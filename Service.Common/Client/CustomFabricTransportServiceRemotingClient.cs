using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Service.Common
{
    public class CustomFabricTransportServiceRemotingClient : IServiceRemotingClient, ICommunicationClient
    {
        private readonly IServiceRemotingClient _innerClient;

        public CustomFabricTransportServiceRemotingClient(IServiceRemotingClient innerClient)
        {
            _innerClient = innerClient;
        }

        ~CustomFabricTransportServiceRemotingClient()
        {
            if (this._innerClient == null) return;
            var disposable = this._innerClient as IDisposable;
            disposable?.Dispose();
        }

        Task<byte[]> IServiceRemotingClient.RequestResponseAsync(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            messageHeaders.SetContextDto(ServiceRequestContext.Current.ContextDto);
            return this._innerClient.RequestResponseAsync(messageHeaders, requestBody);
        }

        void IServiceRemotingClient.SendOneWay(ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            messageHeaders.SetContextDto(ServiceRequestContext.Current.ContextDto);
            this._innerClient.SendOneWay(messageHeaders, requestBody);
        }

        public ResolvedServicePartition ResolvedServicePartition
        {
            get { return this._innerClient.ResolvedServicePartition; }
            set { this._innerClient.ResolvedServicePartition = value; }
        }

        public string ListenerName
        {
            get { return this._innerClient.ListenerName; }
            set { this._innerClient.ListenerName = value; }
        }
        public ResolvedServiceEndpoint Endpoint
        {
            get { return this._innerClient.Endpoint; }
            set { this._innerClient.Endpoint = value; }
        }
    }
}
