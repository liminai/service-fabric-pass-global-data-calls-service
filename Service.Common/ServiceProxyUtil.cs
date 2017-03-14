using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Client;

namespace Service.Common
{
    public static class ServiceProxyUtil
    {
        public static Task<TResult> InvokeService<TServiceInterface, TResult>(Uri serviceUri, Func<TServiceInterface, Task<TResult>> action, CustomContextDataDto context = null) where TServiceInterface : IService
        {
            if(CustomServiceContext.GetContext() == null && context != null)
            {
                CustomServiceContext.SetContext(context);
            }
            CustomContextDataDto dto = CustomServiceContext.GetContext();
            ServiceProxyFactory factory = new ServiceProxyFactory(CreateServiceRemotingClientFactory);
            ServicePartitionKey key = new ServicePartitionKey(GetLongHashCode(dto == null ? string.Empty : dto.ID));
            return ServiceRequestContext.RunInRequestContext<TResult>(() => {
                var serviceProxy = factory.CreateServiceProxy<TServiceInterface>(serviceUri, key);
                return action(serviceProxy);
            }, dto);
        }

        public static Task InvokeService<TServiceInterface>(Uri serviceUri, Func<TServiceInterface, Task> action, CustomContextDataDto context = null) where TServiceInterface : IService
        {
            if (CustomServiceContext.GetContext() == null && context != null)
            {
                CustomServiceContext.SetContext(context);
            }
            CustomContextDataDto dto = CustomServiceContext.GetContext();
            ServiceProxyFactory factory = new ServiceProxyFactory(CreateServiceRemotingClientFactory);
            ServicePartitionKey key = new ServicePartitionKey(GetLongHashCode(dto == null ? string.Empty : dto.ID));
            return ServiceRequestContext.RunInRequestContext(() => {
                var serviceProxy = factory.CreateServiceProxy<TServiceInterface>(serviceUri, key);
                return action(serviceProxy);
            }, dto);
        }

        private static IServiceRemotingClientFactory CreateServiceRemotingClientFactory(IServiceRemotingCallbackClient callbackClient)
        {
            return new CustomFabricTransportServiceRemotingClientFactory(new FabricTransportServiceRemotingClientFactory(callbackClient: callbackClient));
        }

        public static long GetLongHashCode(string stringInput)
        {
            byte[] byteContents = Encoding.Unicode.GetBytes(stringInput);
            MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider();
            byte[] hashText = hash.ComputeHash(byteContents);
            return BitConverter.ToInt64(hashText, 0) ^ BitConverter.ToInt64(hashText, 7);
        }

        public static Uri GetServiceUri(string serviceTypeName)
        {
            return new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/" + serviceTypeName);
        }
    }
}
