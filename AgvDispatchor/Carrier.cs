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
        public string Order { get; set; }                               //FMS系统中当前正在执行或者之前最后一个执行的订单号

        public static float LOW_POWER = 15f;
        public static float HIGH_POWER = 95f;
        public static int RESOURCE_ONE_SIDE = 16;       //料车的一侧的物料总数
        public SendMessage Message;
        public void CarrierAction(object carr)
        {
            Carrier carrier = carr as Carrier;
            DbAccess Db = new DbAccess();
            FmsAction fms = new FmsAction(Message);
            Db.Open();
            if (carrier == null)
            {
                Message("Carrier: " + Code + " get informations error", MessageType.Error);
            }
            else
            {
                CarrierStatus st = (CarrierStatus)Convert.ToInt32(carrier.Status);
                List<Material> materials = Db.GetMaterialsByCarrierCode(Code);
                switch (st)
                {
                    case CarrierStatus.Retrieve:
                    case CarrierStatus.Init:
                    case CarrierStatus.Unloading:
                    case CarrierStatus.Retrieving:
                    case CarrierStatus.Initing:
                    case CarrierStatus.Charge:
                        break;
                    case CarrierStatus.Full:
                        if (materials == null)
                        {
                            Message("Query materials on Carrier: " + Code + " error", MessageType.Error);
                        }
                        else if (materials.Count == 0)
                        {
                            Message("No materials on Carrier: " + Code, MessageType.Exception);
                            if (Db.SetCarrierStatus(Code, CarrierStatus.Init))
                            {
                                string lifterCode;
                                if (Db.AddCarrierToShortestQueue(LifterType.Supply, Code, out lifterCode) && lifterCode != null && lifterCode != string.Empty)
                                {
                                    //HttpWebRequest派去队列排队位置
                                    if (fms.GetAgvInfo(Code) == AgvState.IDLE.ToString())
                                    {
                                        int liftPos = -1;
                                        for (int i = 0; i < MainWindow.LIFTERS.Count; i++)
                                        {
                                            if (MainWindow.LIFTERS[i].Code == lifterCode)
                                            {
                                                liftPos = MainWindow.LIFTERS[i].QueuePosition;
                                            }
                                        }
                                        if (liftPos == -1)
                                        {
                                            break;
                                        }
                                        if (fms.AgvMove(Code, liftPos) == FmsActionResult.Success)
                                        {
                                        }
                                        else
                                        {
                                            Message("Carrier: " + Code + " move to shortest lifter queue fail", MessageType.Error);
                                        }
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                    Message("Add Carrier: " + Code + " to shortest lifter queue fail", MessageType.Error);
                                }
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
                                if (fms.GetAgvInfo(Code) == AgvState.IDLE.ToString())
                                {
                                    int pos = Db.GetCarrierTargetPosition(Code);
                                    if (pos == -1)
                                    {
                                        Message("Carrier: " + Code + " get target position fail", MessageType.Error);
                                        break;
                                    }

                                    if (fms.AgvMove(Code, pos + (int)FmsCarrierPosition.Dev1) == FmsActionResult.Success)
                                    {
                                    }
                                    else
                                    {
                                        Message("Carrier: " + Code + " transport to " + pos + " fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                Message("Empty Carrier: " + Code + " set status to Transport fail", MessageType.Error);
                            }
                        }
                        break;
                    case CarrierStatus.Transport:
                        if (fms.GetAgvInfo(Code) == string.Empty)//need modify
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
                            if (fms.GetAgvInfo(Code) == AgvState.IDLE.ToString())
                            {
                                int pos = Db.GetCarrierTargetPosition(Code);
                                if (pos == -1)
                                {
                                    Message("Carrier: " + Code + " get target position fail", MessageType.Error);
                                    break;
                                }

                                if (fms.AgvMove(Code, pos + (int)FmsCarrierPosition.Dev1) == FmsActionResult.Success)
                                {
                                }
                                else
                                {
                                    Message("Carrier: " + Code + " transport to " + FmsCarrierPosition.Dev1 + " fail", MessageType.Error);
                                }
                            }
                        }
                        break;
                    case CarrierStatus.Complete:
                        if (Db.SetCarrierStatus(Code, CarrierStatus.Retrieve))
                        {
                            string lifterCode;
                            if (Db.AddCarrierToShortestQueue(LifterType.Retrive, Code, out lifterCode) && lifterCode != null && lifterCode != string.Empty)
                            {
                                //HttpWebRequest派去队列排队位置
                                int liftPos = 1;
                                for (int i = 0; i < MainWindow.LIFTERS.Count; i++)
                                {
                                    if (MainWindow.LIFTERS[i].Code == lifterCode)
                                    {
                                        liftPos = MainWindow.LIFTERS[i].QueuePosition;
                                    }
                                }
                                if (liftPos == -1)
                                {
                                    break;
                                }
                                if (fms.GetAgvInfo(Code) == AgvState.IDLE.ToString())
                                {
                                    if (fms.AgvMove(Code, liftPos) == FmsActionResult.Success)
                                    {
                                    }
                                    else
                                    {
                                        Message("Carrier: " + Code + " move to shortest lifter queue fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                Message("Add Carrier: " + Code + " to shortest lifter queue fail", MessageType.Error);
                            }
                        }
                        else
                        {
                            Message("Empty Carrier: " + Code + " set status to retrive fail", MessageType.Error);
                        }
                        break;
                    case CarrierStatus.Idle:
                        float battery;
                        if (float.TryParse(carrier.Battery, out battery))
                        {
                            if (LOW_POWER >= battery)
                            {
                                if (Db.AddAGVToChargeQueue(Code))
                                {
                                    if (Db.SetCarrierStatus(Code, CarrierStatus.Charge))
                                    {
                                        if (fms.GetAgvInfo(Code) == AgvState.IDLE.ToString())
                                        {
                                            int pos = -1;
                                            if (MainWindow.BATTERYS.Count > 0)
                                            {
                                                pos = MainWindow.BATTERYS[0].QueuePosition;
                                            }
                                            if (pos == -1)
                                            {
                                                break;
                                            }
                                            if (fms.AgvMove(Code, pos) == FmsActionResult.Success)
                                            {
                                            }
                                            else
                                            {
                                                Message("Carrier: " + Code + " move to charge queue fail", MessageType.Error);
                                            }
                                        }
                                        else
                                        {
                                        }
                                    }
                                    else
                                    {
                                        Message("Carrier: " + Code + " set status to Charge fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                    Message("Carrier: " + Code + " add to charge queue fail", MessageType.Error);
                                }
                            }
                            else
                            {
                                if (Db.SetCarrierStatus(Code, CarrierStatus.Ready))
                                {
                                    if (fms.GetAgvInfo(Code) == AgvState.IDLE.ToString())
                                    {
                                        if (fms.AgvMove(Code, (int)FmsCarrierPosition.Ready) == FmsActionResult.Success)
                                        {
                                        }
                                        else
                                        {
                                            Message("Carrier: " + Code + " move to ready area fail", MessageType.Error);
                                        }
                                    }
                                    else
                                    {
                                    }
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
                    case CarrierStatus.Ready:
                        if (Db.SetCarrierStatus(Code, CarrierStatus.Init))
                        {
                            string lifterCode;
                            if (Db.AddCarrierToShortestQueue(LifterType.Supply, Code, out lifterCode) && lifterCode != null && lifterCode != string.Empty)
                            {
                                //HttpWebRequest派去队列排队位置
                                int liftPos = 1;
                                for (int i = 0; i < MainWindow.LIFTERS.Count; i++)
                                {
                                    if (MainWindow.LIFTERS[i].Code == lifterCode)
                                    {
                                        liftPos = MainWindow.LIFTERS[i].QueuePosition;
                                    }
                                }
                                if (liftPos == -1)
                                {
                                    break;
                                }
                                if (fms.GetAgvInfo(Code) == AgvState.IDLE.ToString())
                                {
                                    if (fms.AgvMove(Code, liftPos) == FmsActionResult.Success)
                                    {
                                    }
                                    else
                                    {
                                        Message("Carrier: " + Code + " move to shortest lifter queue fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                Message("Add Carrier: " + Code + " to shortest lifter queue fail", MessageType.Error);
                            }
                        }
                        else
                        {
                            Message("Empty Carrier: " + Code + " set status to init fail", MessageType.Error);
                        }
                        break;
                    case CarrierStatus.Charging:
                        float currbattery;
                        if (float.TryParse(carrier.Battery, out currbattery))
                        {
                            if (HIGH_POWER <= currbattery)
                            {
                                if (Db.SetCarrierStatus(Code, CarrierStatus.Idle))
                                {
                                    if (fms.GetAgvInfo(Code) == AgvState.IDLE.ToString())
                                    {
                                        if (fms.AgvMove(Code, (int)FmsCarrierPosition.Ready) == FmsActionResult.Success)
                                        {
                                        }
                                        else
                                        {
                                            Message("Carrier: " + Code + " move to ready area fail", MessageType.Error);
                                        }
                                    }
                                    else
                                    {
                                    }
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