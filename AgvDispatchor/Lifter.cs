using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class Lifter
    {
        public Lifter()
        { }

        public string Code { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Parking { get; set; }
    }

    public enum LifterStatus
    {
        Call = 0,                           //向仓库系统发出请求
        Wait = 1,                         //等待仓库系统响应
        Bite = 2,                           //升降机抓住料车
        Lift = 3,                           //升降机升起
        Fall = 4,                           //升降机降下
        Transport = 5,              //运输料车
        Idle = 6,                           //空闲
    }

    public enum LifterType
    {
        Retrive = 1,
        Supply = 2,
    }
}
