using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class Carrier
    {
        public Carrier()
        { }

        public string Code { get; set; }
        public string Status { get; set; }
        public string Battery { get; set; }
        public string Robot_1 { get; set; }
        public string Robot_2 { get; set; }
    }

    public enum CarrierStatus
    {
        Retrieve = 0,                   //在回收升降机队列中，由队列控制模块置位到其他状态
        Init = 1,                            //在来料升降机队列中，由队列控制模块置位到其他状态
        Full = 2,                           //从升降机接到满料
        Transport = 3,                //去设备上料途中
        Unloading = 4,              //在达目标设备位置，由作业车置位到其他状态
        Retrieving = 5,             //在回收升降机位置，由升降机置位到其他状态
        Initing = 6,                    //在出料升降机位置，由升降机置位到其他状态
        Idle = 7,                           //空闲
        Charge = 8,                     //在充电队列中，由队列控制模块置位到其他状态
        Charging = 9,                   //正在充电
    }
}