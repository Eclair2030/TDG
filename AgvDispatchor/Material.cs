using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class Material
    {
        public Material()
        { }

        public string Code { get; set; }
        public string LifterCode { get; set; }
        public string CarrierCode { get; set; }
        public string CarrierIndex { get; set; }
        public int TargetDeviceCode { get; set; }
        public int TargetDeviceArea { get; set; }
        public int TargetDeviceIndex { get; set; }
        public string RobotCode { get; set; }
        public string Status { get; set; }

        public static int TOTAL_MATERIAL_ONE_DEVICE = 800;      //一台设备两个通道的上料位总数
        public static int TOTAL_MATERIAL_ONE_AREA = 40;             //一台设备的一个区域上料位总数
        public static int TOTAL_MATERIAL_ONE_POSITION = 8;      //搬送车与作业车在一个位置上(不移动)可以上的料总数
        public static int TOTAL_MATERIAL_ONE_CAR = 36;              //一辆料车上的料总数
        public static int TOTAL_AGVS_ONE_POSITION = 3;              //每个上料位对应agv数量，1个carrier，2个robot，第1个位置为carrier，第2个位置是1协作robot，第3个位置是2协作robot
    }

    public enum MaterialStatus
    {
        Lifter = 0,                         //在升降机上
        Carrier = 1,                    //在搬送车上
        Robot = 2,                      //在作业车的手臂上
        Buffer = 3,                     //在作业车的Buffer上
        Complete = 4,               //到达目标设备的目标位置，完成
        Abnormal = 5,               //异常
    }
}
