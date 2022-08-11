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

        public int DeviceCode { get; set; }
        public int DeviceArea { get; set; }
        public int DeviceIndex { get; set; }
        public int RequestCode { get; set; }
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
