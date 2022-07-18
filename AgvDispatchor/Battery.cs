using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class Battery
    {
        public Battery()
        { }

        public string Code { get; set; }
        public string Status { get; set; }
    }

    public enum BatteryStatus
    {
        Idle = 0,                   //空闲
        Busy = 1,                  //被占用
    }
}
