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
                string lifterCode;
                float currbattery = 0f;
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
                            lifterCode = Db.GetLiftersByCarrier(Code);
                            for (int i = 0; i < MainWindow.LIFTERS.Count; i++)
                            {
                                if (MainWindow.LIFTERS[i].Code == lifterCode)
                                {
                                    if (fms.AgvMove(Code, MainWindow.LIFTERS[i].BufferPosition) == FmsActionResult.Success)
                                    {
                                        if (Db.SetCarrierStatus(Code, CarrierStatus.SupplyBuffer))
                                        {
                                        }
                                        else
                                        {
                                            Message("Carrier: " + Code + " set status to SupplyBuffer fail", MessageType.Error);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    case CarrierStatus.Transport:
                        int pos = Db.GetCarrierTargetPosition(Code);    //获取上料位置时，加入是否已经上料完成的判断，获取不到位置作为上料完成信号
                        if (pos == -1)
                        {
                            Message("Carrier: " + Code + " get target position fail", MessageType.Error);
                            break;
                        }
                        if (fms.AgvMove(Code, pos + (int)FmsCarrierPosition.Dev0) == FmsActionResult.Success)
                        {
                            if (carrier.Robot_1 == null || carrier.Robot_1 == string.Empty)
                            {
                                string robCode;
                                if (Db.SendRobotForCarrier(Code, 1, out robCode))
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
                                    Message("Send robot_1 for carrier: " + Code + "  fail", MessageType.Error);
                                }
                            }
                        }
                        else      //没有料了，去回收升降机
                        {
                            if (Db.SetCarrierStatus(Code, CarrierStatus.Complete))
                            {
                            }
                            else
                            {
                                Message("Empty Carrier: " + Code + " set status to Complete fail", MessageType.Error);
                            }
                        }
                        break;
                    case CarrierStatus.Complete:
                        if (Db.AddCarrierToShortestQueue(LifterType.Retrive, Code, out lifterCode) && lifterCode != null && lifterCode != string.Empty)
                        {
                            //HttpWebRequest派去队列排队位置
                            for (int i = 0; i < MainWindow.LIFTERS.Count; i++)
                            {
                                if (MainWindow.LIFTERS[i].Code == lifterCode)
                                {
                                    if (fms.AgvMove(Code, MainWindow.LIFTERS[i].QueuePosition) == FmsActionResult.Success)
                                    {
                                        if (Db.SetCarrierStatus(Code, CarrierStatus.Retrieve))
                                        {
                                        }
                                        else
                                        {
                                            Message("Empty Carrier: " + Code + " set status to retrive fail", MessageType.Error);
                                        }
                                    }
                                    else
                                    {
                                        Message("Carrier: " + Code + " move to shortest lifter queue fail", MessageType.Error);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Message("Add Carrier: " + Code + " to shortest lifter queue fail", MessageType.Error);
                        }
                        break;
                    case CarrierStatus.Idle:
                        currbattery = fms.GetAgvBattery(Code);
                        Message("Carrier: " + Code + " current battery " + currbattery, MessageType.Default);
                        if (LOW_POWER >= currbattery)
                        {
                            if (MainWindow.BATTERYS.Count > 0)
                            {
                                if (fms.AgvMove(Code, MainWindow.BATTERYS[0].QueuePosition) == FmsActionResult.Success)
                                {
                                    if (Db.SetCarrierStatus(Code, CarrierStatus.Charge))
                                    {
                                        if (Db.AddAGVToChargeQueue(Code))
                                        {
                                        }
                                        else
                                        {
                                            Message("Carrier: " + Code + " add to charge queue fail", MessageType.Error);
                                        }
                                    }
                                    else
                                    {
                                        Message("Carrier: " + Code + " set status to Charge fail", MessageType.Error);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (fms.AgvMove(Code, (int)FmsCarrierPosition.Idle) == FmsActionResult.Success)
                            {
                                if (Db.SetCarrierStatus(Code, CarrierStatus.Ready))
                                {
                                }
                                else
                                {
                                    Message("Carrier: " + Code + " set status to Ready fail", MessageType.Error);
                                }
                            }
                        }
                        break;
                    case CarrierStatus.Ready:
                        if (Db.AddCarrierToShortestQueue(LifterType.Supply, Code, out lifterCode) && lifterCode != null && lifterCode != string.Empty)
                        {
                            //HttpWebRequest派去队列排队位置
                            for (int i = 0; i < MainWindow.LIFTERS.Count; i++)
                            {
                                if (MainWindow.LIFTERS[i].Code == lifterCode)
                                {
                                    if (fms.AgvMove(Code, MainWindow.LIFTERS[i].QueuePosition) == FmsActionResult.Success)
                                    {
                                        if (Db.SetCarrierStatus(Code, CarrierStatus.Init))
                                        {
                                        }
                                        else
                                        {
                                            Message("Carrier: " + Code + " set status to init fail", MessageType.Error);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (lifterCode != null && lifterCode != string.Empty)
                            {
                                //Message("Add Carrier: " + Code + " is already in queue", MessageType.Default);
                                //HttpWebRequest派去队列排队位置
                                for (int i = 0; i < MainWindow.LIFTERS.Count; i++)
                                {
                                    if (MainWindow.LIFTERS[i].Code == lifterCode)
                                    {
                                        if (fms.AgvMove(Code, MainWindow.LIFTERS[i].QueuePosition) == FmsActionResult.Success)
                                        {
                                            if (Db.SetCarrierStatus(Code, CarrierStatus.Init))
                                            {
                                            }
                                            else
                                            {
                                                Message("Carrier: " + Code + " set status to init fail", MessageType.Error);
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            else if (lifterCode != string.Empty)
                            {
                                Message("Add Carrier: " + Code + " is already in queue, but query lifter code fail", MessageType.Default);
                            }
                            else
                            {
                                Message("Add Carrier: " + Code + " to shortest lifter queue fail", MessageType.Error);
                            }
                        }
                        break;
                    case CarrierStatus.Charging:
                        currbattery = fms.GetAgvBattery(Code);
                        if (HIGH_POWER <= currbattery)
                        {
                            if (Db.SetCarrierStatus(Code, CarrierStatus.Idle))
                            {
                                if (fms.GetAgvInfo(Code) == AgvState.IDLE.ToString())
                                {
                                    if (fms.AgvMove(Code, (int)FmsCarrierPosition.Idle) == FmsActionResult.Success)
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
                        break;
                    case CarrierStatus.SupplyBuffer:
                        //送料去设备
                        lifterCode = Db.GetLiftersByCarrier(Code);
                        if (Db.SetSupplyLifterStatus(SupplyLifterStatus.Loading, lifterCode))
                        {
                            if (Db.SetCarrierStatus(Code, CarrierStatus.Transport))
                            {
                                lifterCode = Db.GetLiftersByCarrier(Code);
                                Db.LifterQueueDeleteZero(lifterCode);
                            }
                            else
                            {
                                Message("Carrier: " + Code + " set status to Transport fail", MessageType.Error);
                            }
                        }
                        else
                        {
                            Message("Set lifter: " + lifterCode + " status to Loading fail", MessageType.Error);
                        }
                        break;
                    case CarrierStatus.Empty:
                        lifterCode = Db.GetLiftersByCarrier(Code);
                        for (int i = 0; i < MainWindow.LIFTERS.Count; i++)
                        {
                            if (MainWindow.LIFTERS[i].Code == lifterCode)
                            {
                                if (fms.AgvMove(Code, MainWindow.LIFTERS[i].BufferPosition) == FmsActionResult.Success)
                                {
                                    if (Db.SetCarrierStatus(Code, CarrierStatus.RetrievBuffer))
                                    {
                                    }
                                    else
                                    {
                                        Message("Carrier: " + Code + " set status to RetrievBuffer fail", MessageType.Error);
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    case CarrierStatus.RetrievBuffer:
                        lifterCode = Db.GetLiftersByCarrier(Code);
                        if (Db.SetRetriveLifterStatus(RetriveLifterStatus.Leave, lifterCode))
                        {
                            if (Db.SetCarrierStatus(Code, CarrierStatus.Idle))
                            {
                                lifterCode = Db.GetLiftersByCarrier(Code);
                                Db.LifterQueueDeleteZero(lifterCode);
                            }
                            else
                            {
                                Message("Carrier: " + Code + " set status to Idle fail", MessageType.Error);
                            }
                        }
                        else
                        {
                            Message("Set lifter: " + lifterCode + " status to Loading fail", MessageType.Error);
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
        SupplyBuffer = 12,         //从来料升降机接到满料，前往进/出的Buffer位置
        RetrievBuffer = 13,         //从回收升降机放下空料，前往进/出的Buffer位置
        Empty = 14,                     //回收升降机抬起空料车，Carrier处于空车状态
    }
}