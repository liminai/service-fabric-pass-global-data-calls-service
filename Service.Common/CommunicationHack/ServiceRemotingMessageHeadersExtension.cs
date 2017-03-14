using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using Newtonsoft.Json;

namespace Service.Common
{
    public static class ServiceRemotingMessageHeadersExtension
    {
        public static bool SetContextDto(this ServiceRemotingMessageHeaders headers,　CustomContextDataDto contextData)
        {
            try
            {
                if(contextData != null)
                {
                    string str = JsonConvert.SerializeObject(contextData);
                    headers.AddHeader("contextdata-header", Encoding.UTF8.GetBytes(str));
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static CustomContextDataDto GetContextDto(this ServiceRemotingMessageHeaders headers)
        {
            byte[] result;
            if (headers.TryGetHeaderValue("contextdata-header", out result))
            {
                string str = Encoding.UTF8.GetString(result);
                if (!string.IsNullOrEmpty(str))
                {
                    CustomContextDataDto dto = JsonConvert.DeserializeObject<CustomContextDataDto>(str);
                    return dto;
                }
                return null;
            }
            else
            {
                return null;
            }
        }
    }
}
