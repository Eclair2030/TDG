using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    public class Battery
    {
        public Battery()
        { }

        public string Code { get; set; }
        public int Status { get; set; }
        public int Position { get; set; }
        public int QueuePosition { get; set; }
    }

    public enum BatteryStatus
    {
        Idle = 0,                   //空闲
        Busy = 1,                  //被占用
    }
}
