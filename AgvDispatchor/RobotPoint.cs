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

        private static int X_OFFSET = 100;
        private static int DEVZ_OFFSET = 100;

        public bool PointMatch(RobotPoint point)
        {
            bool result = false;
            if (Math.Abs(point.X - X) < 1 && Math.Abs(point.Y - Y) < 1 && Math.Abs(point.Z - Z) < 1 && 
                Math.Abs(point.RX - RX) < 1 && Math.Abs(point.RY - RY) < 1 && Math.Abs(point.RZ - RZ) < 1)
            {
                result = true;
            }
            return result;
        }

        public RobotPoint GetCarrierMaterialPosition(int carrIndex)
        {
            RobotPoint pos = this;
            pos.X = X + X_OFFSET * carrIndex;
            return pos;
        }

        public RobotPoint GetDeviceMaterialPosition(int devIndex)
        {
            RobotPoint pos = this;
            pos.Z -= DEVZ_OFFSET * (devIndex % Material.TOTAL_MATERIAL_ONE_POSITION);
            return pos;
        }
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
    {   //等待位与拍照位共用
        None = -1,                     //不存在的位置，默认值
        Dwait0 = 0,                    //设备第0个等待位
        Dwait1 = 1,                    //设备第1个等待位
        Dwait2 = 2,                    //设备第2个等待位
        Dwait3 = 3,                    //设备第3个等待位，和中间位之间的桥梁
        Staff0 = 4,                     //Carrier第0个等待位
        Staff1 = 5,                     //Carrier第1个等待位
        Staff2 = 6,                     //Carrier第2个等待位
        Staff3 = 7,                     //Carrier第3个等待位
        BufferWait = 8,             //Buffer等待位
        Buffer = 9,                     //Buffer取料位
        Convert = 10,              //中转位置
    }
}
