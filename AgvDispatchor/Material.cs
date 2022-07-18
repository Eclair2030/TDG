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
        public string TargetDeviceCode { get; set; }
        public string TargetDeviceIndex { get; set; }
        public string RobotCode { get; set; }
        public string Status { get; set; }
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
