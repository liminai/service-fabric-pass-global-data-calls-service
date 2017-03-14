using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Service.Common
{
    public class CustomFabricTransportServiceRemotingClientFactory : IServiceRemotingClientFactory
    {
        private readonly ICommunicationClientFactory<IServiceRemotingClient> _innerClientFactory;

        public CustomFabricTransportServiceRemotingClientFactory(ICommunicationClientFactory<IServiceRemotingClient> innerClientFactory)
        {
            _innerClientFactory = innerClientFactory;
            _innerClientFactory.ClientConnected += OnClientConnected;
            _innerClientFactory.ClientDisconnected += OnClientDisconnected;
        }

        private void OnClientConnected(object sender, CommunicationClientEventArgs<IServiceRemotingClient> e)
        {
            EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> clientConnected = this.ClientConnected;
            if (clientConnected == null) return;
            clientConnected((object)this, new CommunicationClientEventArgs<IServiceRemotingClient>()
            {
                Client = e.Client
            });
        }

        private void OnClientDisconnected(object sender, CommunicationClientEventArgs<IServiceRemotingClient> e)
        {
            EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> clientDisconnected = this.ClientDisconnected;
            if (clientDisconnected == null) return;
            clientDisconnected((object)this, new CommunicationClientEventArgs<IServiceRemotingClient>()
            {
                Client = e.Client
            });
        }

        async Task<IServiceRemotingClient> ICommunicationClientFactory<IServiceRemotingClient>.GetClientAsync(ResolvedServicePartition previousRsp, TargetReplicaSelector targetReplicaSelector, string listenerName, OperationRetrySettings retrySettings, CancellationToken cancellationToken)
        {
            var client = await _innerClientFactory.GetClientAsync(
            previousRsp,
            targetReplicaSelector,
            listenerName,
            retrySettings,
            cancellationToken);
            return new CustomFabricTransportServiceRemotingClient(client);

        }

        async Task<IServiceRemotingClient> ICommunicationClientFactory<IServiceRemotingClient>.GetClientAsync(Uri serviceUri, ServicePartitionKey partitionKey, TargetReplicaSelector targetReplicaSelector, string listenerName, OperationRetrySettings retrySettings, CancellationToken cancellationToken)
        {
            var client = await _innerClientFactory.GetClientAsync(
            serviceUri,
            partitionKey,
            targetReplicaSelector,
            listenerName,
            retrySettings,
            cancellationToken);
            return new CustomFabricTransportServiceRemotingClient(client);

        }

        Task<OperationRetryControl> ICommunicationClientFactory<IServiceRemotingClient>.ReportOperationExceptionAsync(IServiceRemotingClient client, ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, CancellationToken cancellationToken)
        {
            return _innerClientFactory.ReportOperationExceptionAsync(
            client,
            exceptionInformation,
            retrySettings,
            cancellationToken);
        }

        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientConnected;
        public event EventHandler<CommunicationClientEventArgs<IServiceRemotingClient>> ClientDisconnected;

    }
}
