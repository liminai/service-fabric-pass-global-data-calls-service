using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace Service.Common
{
    public class CustomServiceRemotingDispatcher : ServiceRemotingDispatcher
    {
        public CustomServiceRemotingDispatcher(System.Fabric.ServiceContext serviceContext, IService service) : base(serviceContext, service)
        {
        }

        public override Task<byte[]> RequestResponseAsync(IServiceRemotingRequestContext requestContext, ServiceRemotingMessageHeaders messageHeaders, byte[] requestBody)
        {
            var contextDto = messageHeaders.GetContextDto();

            CustomServiceContext.SetContext(contextDto);

            return ServiceRequestContext.RunInRequestContext(async () =>
                await base.RequestResponseAsync(
                    requestContext,
                    messageHeaders,
                    requestBody),
                    contextDto);
        }
    }
}
