using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carputer.Phone.UWP.SMAC
{
    public class ServiceMessageEnvelope
    {
        public string FromMachineID { get; set; }
        public string ToMachineID { get; set; }
        public string FromServiceID { get; set; }
        public string ToServiceID { get; set; }

        public string DataTypeName { get; set; }
        public string Data { get; set; }

        public ServiceMessageEnvelope()
        {
            
        }
    }
}
