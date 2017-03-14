using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common
{
    [Serializable]
    public class CustomContextDataDto : ILogicalThreadAffinative
    {
        public string ID { get; set; }

        public string Name { get; set; }
    }
}
