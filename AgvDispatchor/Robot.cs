﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class Robot
    {
        public Robot()
        { }

        public string Code { get; set; }
        public string Status { get; set; }
        public string Battery { get; set; }
        public string Coordination { get; set; }
        public string Process { get; set; }
        public string DeviceIndex { get; set; }
        public string CarrierIndex { get; set; }
    }

    public enum RobotStatus
    {
        Moving = 0,                         //去设备上料途中
        Ready = 1,                            //到达目标上料位置
        SnapDevice = 2,                 //相机对设备的料位拍照
        PushDevice = 3,                //机械手把当前抓取的料放到设备
        PopDevice = 4,                  //机械手抓取设备上的料
        SnapCarrier = 5,                //相机对搬送车的料位拍照
        PushCarrier = 6,                    //机械手把当前抓取的料放到料车
        PopCarrier = 7,                     //机械手抓取料车上的料
        SnapBuffer = 8,                     //相机对作业车的Buffer拍照
        PushBuffer = 9,                   //机械手把当前抓取的料放入Buffer
        PopBuffer = 10,                 //机械手抓取Buffer上的料
        Idle = 11,                          //空闲
        Charge = 12,                    //在充电队列中
        Charging = 13,                  //正在充电
    }
}
