using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class Carrier
    {
        public Carrier()
        {
        }
        public string Code { get; set; }
        public string Status { get; set; }
        public string Battery { get; set; }
        public string Robot_1 { get; set; }
        public string Robot_2 { get; set; }

        public static float LOW_POWER = 15f;
        public static float HIGH_POWER = 95f;
        public delegate void SendMessage(string msg, MessageType mt);
        public SendMessage Message;
        public void CarrierAction(object carr)
        {
            Carrier carrier = carr as Carrier;
            DbAccess Db = new DbAccess();
            Db.Open();
            if (carrier == null)
            {
                Message("Carrier: " + Code + " get informations error", MessageType.Error);
            }
            else
            {
                CarrierStatus st = (CarrierStatus)Convert.ToInt32(carrier.Status);
                switch (st)
                {
                    case CarrierStatus.Retrieve:
                    case CarrierStatus.Init:
                    case CarrierStatus.Unloading:
                    case CarrierStatus.Retrieving:
                    case CarrierStatus.Initing:
                    case CarrierStatus.Ready:
                    case CarrierStatus.Charge:
                        break;
                    case CarrierStatus.Full:
                        List<Material> materials = Db.GetMaterialsByCarrierCode(Code);
                        if (materials == null)
                        {
                            Message("Query materials on Carrier: " + Code + " error", MessageType.Error);
                        }
                        else if (materials.Count == 0)
                        {
                            Message("No materials on Carrier: " + Code, MessageType.Exception);
                            if (Db.SetCarrierStatus(Code, CarrierStatus.Init))
                            {
                                //HttpWebRequest 回去升降机上料
                            }
                            else
                            {
                                Message("Empty Carrier: " + Code + " set status to init fail", MessageType.Error);
                            }
                        }
                        else
                        {
                            //HttpWebRequest 送料去设备
                            if (Db.SetCarrierStatus(Code, CarrierStatus.Transport))
                            {
                            }
                            else
                            {
                                Message("Empty Carrier: " + Code + " set status to Transport fail", MessageType.Error);
                            }
                        }
                        break;
                    case CarrierStatus.Transport:
                        //HttpWebRequest
                        if (true)
                        {
                            if (Db.SetCarrierStatus(Code, CarrierStatus.Unloading))
                            {
                            }
                            else
                            {
                                Message("Empty Carrier: " + Code + " set status to Unloading fail", MessageType.Error);
                            }
                        }
                        else
                        {
                        }
                        break;
                    case CarrierStatus.Complete:
                        //HttpWebRequest去往卸料等待区
                        break;
                    case CarrierStatus.Idle:
                        float battery;
                        if (float.TryParse(carrier.Battery, out battery))
                        {
                            if (LOW_POWER >= battery)
                            {
                                //HttpWebRequest 去充电队列
                                if (Db.SetCarrierStatus(Code, CarrierStatus.Charge))
                                {
                                }
                                else
                                {
                                    Message("Carrier: " + Code + " set status to Charge fail", MessageType.Error);
                                }
                            }
                            else
                            {
                                //HttpWebRequest 去空闲区
                                if (Db.SetCarrierStatus(Code, CarrierStatus.Ready))
                                {
                                }
                                else
                                {
                                    Message("Carrier: " + Code + " set status to Ready fail", MessageType.Error);
                                }
                            }
                        }
                        else
                        {
                            Message("Carrier: " + Code + " battery data error", MessageType.Error);
                        }
                        break;
                    case CarrierStatus.Charging:
                        float currbattery;
                        if (float.TryParse(carrier.Battery, out currbattery))
                        {
                            if (HIGH_POWER <= currbattery)
                            {
                                //HttpWebRequest
                                if (Db.SetCarrierStatus(Code, CarrierStatus.Idle))
                                {
                                }
                                else
                                {
                                    Message("Empty Carrier: " + Code + " set status to Idle fail", MessageType.Error);
                                }
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            Message("Carrier: " + Code + " battery data error", MessageType.Error);
                        }
                        break;
                    default:
                        Message("Carrier: " + Code + " status data error", MessageType.Error);
                        break;
                }
            }
            Db.Close();
        }
    }

    public enum CarrierStatus
    {
        Retrieve = 0,                   //在回收升降机队列中，由队列控制模块置位到其他状态
        Init = 1,                            //在来料升降机队列中，由队列控制模块置位到其他状态
        Full = 2,                           //从升降机接到满料
        Transport = 3,                //去设备上料途中
        Complete = 4,               //上料完成，由回收升降机队列置位到Retrive状态
        Unloading = 5,              //在达目标设备位置，由作业车置位到其他状态
        Retrieving = 6,             //在回收升降机位置，由升降机置位到其他状态
        Initing = 7,                    //在出料升降机位置，由升降机置位到其他状态
        Idle = 8,                           //空闲，由充电队列模块置位到Ready或者Charge状态
        Ready = 9,                      //就绪状态，可以被来料升降机队列调用，由来料升降机队列置位到Init状态
        Charge = 10,                  //在充电队列中，由队列控制模块置位到其他状态
        Charging = 11,              //正在充电
    }
}