using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace Service.Common
{
    public class CustomFabricTransportServiceRemotingProviderAttribute : FabricTransportServiceRemotingProviderAttribute
    {
        public override IServiceRemotingListener CreateServiceRemotingListener(System.Fabric.ServiceContext serviceContext, IService serviceImplementation)
        {
            var messageHandler = new CustomServiceRemotingDispatcher(
                serviceContext, serviceImplementation);

            return new FabricTransportServiceRemotingListener(serviceContext: serviceContext, messageHandler: messageHandler);
        }

        public override IServiceRemotingClientFactory CreateServiceRemotingClientFactory(IServiceRemotingCallbackClient callbackClient)
        {
            return new FabricTransportServiceRemotingClientFactory(callbackClient: callbackClient, servicePartitionResolver: null, traceId: null);
        }
    }
}
