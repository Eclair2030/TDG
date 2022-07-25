using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class Request
    {
        public Request()
        { }

        public string DeviceCode { get; set; }
        public string DeviceIndex { get; set; }
        public string RequestCode { get; set; }
        public string LastResponseTime { get; set; }
    }

    public enum RequestCodeType
    {
        None = 0,
        Oncall = 1,
        WaitingForStore = 2,
        Transporting = 3,
    }
}
