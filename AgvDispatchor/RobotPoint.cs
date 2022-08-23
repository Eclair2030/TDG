using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class RobotPoint
    {
        public RobotPoint() { }

        public string Code { get; set; }
        public string PtName { get; set; }
        public double J1 { get; set; }
        public double J2 { get; set; }
        public double J3 { get; set; }
        public double J4 { get; set; }
        public double J5 { get; set; }
        public double J6 { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double RX { get; set; }
        public double RY { get; set; }
        public double RZ { get; set; }
    }

    public enum RobotAxis
    {
        X = 0,
        Y = 1,
        Z = 2,
        RX = 3,
        RY = 4,
        RZ = 5,
    }

    public enum RobotBasePosition
    {
        None = 0,                       //未定义
        DeviceSnap = 1,             //设备拍照基准位
        CarrierSnap = 2,              //料车拍照基准位
        BufferSnap = 3,               //Robot Buffer拍照基准位
        DeviceWait = 4,             //设备等待基准位
        CarrierWait = 5,               //料车等待基准位
        BufferWait = 6,                 //Robot Buffer等待基准位
        DeviceWork = 7,             //设备抓/取料基准位
        CarrierWork = 8,                //料车抓/取料基准位
        BufferWork = 9,                 //Robot Buffer抓/取料基准位
        Convert = 10,                   //不同工序间的中转位
    }

    public enum PositionNames
    {
        DevWait_0 = 0,              //设备第0个等待位
        DevWait_1 = 1,              //设备第0个等待位
        DevWait_2 = 2,              //设备第0个等待位
        DevWait_3 = 3,              //设备第0个等待位
    }
}
