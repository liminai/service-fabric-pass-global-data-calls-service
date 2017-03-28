using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric.Description;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Service.Common.Util;

namespace Service.Web
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class Web : StatelessService
    {
        private static ServiceEventSource logger = ServiceEventSource.Current;

        public Web(StatelessServiceContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            var endpoints = this.Context.CodePackageActivationContext.GetEndpoints()
                                   .Where(endpoint => endpoint.Protocol == EndpointProtocol.Http || endpoint.Protocol == EndpointProtocol.Https)
                                   .Select(endpoint => endpoint.Name);
            string serverType = FabricConfigUtil.GetConfigValue("ServerConfig", "ServerType");
            if ("WebListener".ToLowerInvariant().Equals(serverType))
            {
                //Use Web Listener
                return endpoints.Select(endpoint => new ServiceInstanceListener(
                    serviceContext => new WebListenerCommunicationListener(serviceContext, endpoint, (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting WebListener on {url}");
                        return new WebHostBuilder()
                             .UseWebListener()
                             .UseApplicationInsights()
                             .ConfigureServices(
                                 services => services
                                     .AddSingleton<StatelessServiceContext>(serviceContext))
                             .UseContentRoot(Directory.GetCurrentDirectory())
                             //.UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                             .UseStartup<Startup>()
                             .UseUrls(url)
                             .Build();
                    }), endpoint));
            }
            else
            {
                //Use Kestrel
                return endpoints.Select(endpoint => new ServiceInstanceListener(
                    serviceContext => new KestrelCommunicationListener(serviceContext, endpoint, (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting WebListener on {url}");
                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .UseApplicationInsights()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseStartup<Startup>()
                                    .UseUrls(url)
                                    .Build();
                    }
                    ), endpoint));
            }
        }
    }
}