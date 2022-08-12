using System;
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
        public int Status { get; set; }
        public string Battery { get; set; }
        public int Coordination { get; set; }
        public int Process { get; set; }
        public int DeviceIndex { get; set; }
        public int CarrierIndex { get; set; }
        public int Buffer1 { get; set; }
        public int Buffer2 { get; set; }
        public int Arm { get; set; }

        public static float LOW_POWER = 15f;
        public static float HIGH_POWER = 95f;
        public SendMessage Message;

        public void RobotAction(object obj)
        {
            if (obj == null)
            {
                return;
            }
            Robot robot = obj as Robot;
            DbAccess Db = new DbAccess();
            FmsAction fms = new FmsAction(Message);
            Db.Open();
            RobotProcess rp = (RobotProcess)robot.Process;
            RobotStatus st = (RobotStatus)robot.Status;
            switch (rp)
            {
                case RobotProcess.None:
                    break;
                case RobotProcess.Device2Carrier:
                    switch (st)
                    {
                        case RobotStatus.SnapDevice:
                            break;
                        case RobotStatus.PopDevice:
                            break;
                        case RobotStatus.SnapCarrier:
                            break;
                        case RobotStatus.PushCarrier:
                            break;
                        default:
                            break;
                    }
                    break;
                case RobotProcess.Carrier2Device:
                    switch (st)
                    {
                        case RobotStatus.SnapDevice:
                            break;
                        case RobotStatus.PushDevice:
                            break;
                        case RobotStatus.SnapCarrier:
                            break;
                        case RobotStatus.PopCarrier:
                            break;
                        default:
                            break;
                    }
                    break;
                case RobotProcess.Device2Buffer:
                    switch (st)
                    {
                        case RobotStatus.SnapDevice:
                            break;
                        case RobotStatus.PopDevice:
                            break;
                        case RobotStatus.SnapBuffer:
                            break;
                        case RobotStatus.PushBuffer:
                            break;
                        default:
                            break;
                    }
                    break;
                case RobotProcess.Buffer2Device:
                    switch (st)
                    {
                        case RobotStatus.SnapDevice:
                            break;
                        case RobotStatus.PushDevice:
                            break;
                        case RobotStatus.SnapBuffer:
                            break;
                        case RobotStatus.PopBuffer:
                            break;
                        default:
                            break;
                    }
                    break;
                case RobotProcess.Carrier2Buffer:
                    switch (st)
                    {
                        case RobotStatus.SnapCarrier:
                            break;
                        case RobotStatus.PopCarrier:
                            break;
                        case RobotStatus.SnapBuffer:
                            break;
                        case RobotStatus.PushBuffer:
                            break;
                        default:
                            break;
                    }
                    break;
                case RobotProcess.Buffer2Carrier:
                    switch (st)
                    {
                        case RobotStatus.SnapCarrier:
                            break;
                        case RobotStatus.PushCarrier:
                            break;
                        case RobotStatus.SnapBuffer:
                            break;
                        case RobotStatus.PopBuffer:
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            
        }
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
        Idle = 11,                          //空闲，由充电队列模块置位到Standby或者Charge状态
        Standby = 12,                      //就绪，可以应答上料请求
        Charge = 13,                    //在充电队列中
        Charging = 14,                  //正在充电
    }

    public enum RobotProcess
    {
        None = 0,                           //机械手无作业
        Device2Carrier = 1,         //设备到搬送车作业
        Carrier2Device = 2,         //搬送车到设备作业
        Device2Buffer = 3,          //设备到作业车Buffer作业
        Buffer2Device = 4,          //作业车Buffer到设备作业
        Carrier2Buffer = 5,         //搬送车到Buffer作业
        Buffer2Carrier = 6,         //Buffer到搬送车作业
    }
}
