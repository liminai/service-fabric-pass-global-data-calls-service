using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Service.Weather.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using System.Net.Http;
using Service.Common.Util;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;

namespace Service.Weather
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Weather : StatefulService, IWeatherService
    {
        public Weather(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context)) };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            //var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            //while (true)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    using (var tx = this.StateManager.CreateTransaction())
            //    {
            //        var result = await myDictionary.TryGetValueAsync(tx, "Counter");

            //        ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
            //            result.HasValue ? result.Value.ToString() : "Value does not exist.");

            //        await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

            //        // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
            //        // discarded, and nothing is saved to the secondary replicas.
            //        await tx.CommitAsync();
            //    }

            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //}
            await base.RunAsync(cancellationToken);
        }

        Task<string> IWeatherService.GetCurrentWeather(string cityName)
        {
            string baseUri = FabricConfigUtil.GetConfigValue("WebServiceSettings", "BaseUri");
            using (HttpClientHandler handler = new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.GZip })
            {
                using (HttpClient client = new HttpClient(handler) { BaseAddress = new Uri(baseUri + "/") })
                {
                    //FormUrlEncodedContent content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("byProvinceName", cityName) });
                    //HttpResponseMessage responseMessage = client.PostAsync("getSupportCity", content).Result;
                    //HttpResponseMessage responseMessage = client.GetAsync("getSupportProvince").Result;
                    FormUrlEncodedContent content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("theCityName", cityName) });
                    HttpResponseMessage responseMessage = client.PostAsync("getWeatherbyCityName", content).Result;
                    return responseMessage.Content.ReadAsStringAsync();
                }
            }
        }

        Task<string> IWeatherService.GetSupportCity(string provinceName)
        {
            string baseUri = FabricConfigUtil.GetConfigValue("WebServiceSettings", "BaseUri");
            using (HttpClientHandler handler = new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.GZip })
            {
                using (HttpClient client = new HttpClient(handler) { BaseAddress = new Uri(baseUri + "/") })
                {
                    FormUrlEncodedContent content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("byProvinceName", provinceName) });
                    HttpResponseMessage responseMessage = client.PostAsync("getSupportCity", content).Result;
                    string result = responseMessage.Content.ReadAsStringAsync().Result;
                    return Task.Run(()=> { return result; });
                }
            }
        }
    }
}
