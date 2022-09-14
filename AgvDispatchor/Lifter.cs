using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    public class Lifter
    {
        public Lifter()
        {
            Motors = new LifterMotors();
            Sensors = new LifterSensors();
        }

        public string Code { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string Parking { get; set; } //弃用
        public int QueuePosition { get; set; }
        public int Position { get; set; }
        public int BufferPosition { get; set; }

        public delegate void SendMessage(string msg, MessageType mt);
        public SendMessage Message;
        public FmsAction Fms;
        public LifterMotors Motors;
        public LifterSensors Sensors;

        public void RetriveLifterAction(object obj)
        {
            Lifter lifter = obj as Lifter;
            if (lifter != null && lifter.Type == (int)LifterType.Retrive)
            {
                try
                {
                    RetriveLifterStatus status = (RetriveLifterStatus)Convert.ToInt32(lifter.Status);
                    DbAccess db = new DbAccess();
                    string carrierCode = string.Empty;
                    long sensors = 0;
                    string strSensor = string.Empty;
                    if (Sensors.ReadSensors(ref sensors))
                    {
                        strSensor = Convert.ToString(sensors, 2).PadLeft(16, '0');
                    }
                    else
                    {
                        Message("Retrive lifter: " + lifter.Code + " read sensors fail", MessageType.Error);
                    }
                    if (db.Open())
                    {
                        switch (status)
                        {
                            case RetriveLifterStatus.Loading:
                                if (Motors.MoveToPosition(LifterMotorPosition.DownPosition))     //电机去接料位
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Load, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Load fail", MessageType.Error);
                                    }
                                }
                                break;
                            case RetriveLifterStatus.Load:
                                carrierCode = db.GetFirstCarrierInLifterQueuebyCode(lifter.Code, (LifterType)lifter.Type);
                                if (Fms.GetAgvInfo(carrierCode) == AgvState.IDLE.ToString() &&
                                    Fms.GetAgvStation(carrierCode) == lifter.Position)                      //用FMS系统判断搬送车是否已经到位
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Arrive, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Arrive fail", MessageType.Error);
                                    }
                                }
                                break;
                            case RetriveLifterStatus.Arrive:
                                if (Motors.MoveToPosition(LifterMotorPosition.UpPosition))          //升降机上升到抬高位
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Avoiding, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Avoiding fail", MessageType.Error);
                                    }
                                }
                                break;
                            case RetriveLifterStatus.Avoiding:
                                if (Motors.MoveToPosition(LifterMotorPosition.UpPosition))          //升降机上升到抬高位
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Avoid, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Avoid fail", MessageType.Error);
                                    }
                                }
                                break;
                            case RetriveLifterStatus.Avoid:
                                //空车Carrier离开时，Carrier会将Lifter的状态设置为Leave
                                carrierCode = db.GetFirstCarrierInLifterQueuebyCode(lifter.Code, (LifterType)lifter.Type);
                                if (carrierCode != null && carrierCode != string.Empty)
                                {
                                    db.SetCarrierStatus(carrierCode, CarrierStatus.Empty);
                                }
                                else
                                {
                                    Message("Retrive lifter: " + lifter.Code + " get carrier code fail at avoid position", MessageType.Error);
                                }
                                break;
                            case RetriveLifterStatus.Leave:
                                //升降机Z轴去下料位
                                if (db.SetRetriveLifterStatus(RetriveLifterStatus.Unloading, lifter.Code))
                                {
                                }
                                else
                                {
                                    Message("Retrive lifter: " + lifter.Code + " set status to Unloading fail", MessageType.Error);
                                }
                                break;
                            case RetriveLifterStatus.Unloading:
                                if (Motors.MoveToPosition(LifterMotorPosition.DownPosition))                        //升降机下降到下料位
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Unload, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Unload fail", MessageType.Error);
                                    }
                                }
                                break;
                            case RetriveLifterStatus.Unload:
                                if (strSensor[LifterSensors.MaterialCustom] == LifterSensors.SensorOff)         //判断升降机客户Buffer上是否不存在料车
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Ready, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Ready fail", MessageType.Error);
                                    }
                                }
                                break;
                            case RetriveLifterStatus.Ready:
                                if (Motors.ChainMoving(LifterMoveType.Push) != LifterMotorStatus.Default)       //转动链条，把料车送往Buffer
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.ChainRoll, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to ChainRoll fail", MessageType.Error);
                                    }
                                }
                                break;
                            case RetriveLifterStatus.ChainRoll:
                                if (Motors.ChainMoving(LifterMoveType.Push) != LifterMotorStatus.Stopped)       //用Sensor判断料车已经离开升降机
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Loading, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Loading fail", MessageType.Error);
                                    }
                                }
                                break;
                            default:
                                Message("Retrive lifter: " + lifter.Code + " set status error", MessageType.Error);
                                break;
                        }
                    }
                    else
                    {
                        Message("Retrive lifter: " + lifter.Code + " database access error", MessageType.Error);
                    }
                    db.Close();
                }
                catch (Exception)
                {
                    Message("Retrive lifter: " +lifter.Code + " action fail", MessageType.Error);
                }
                
            }
            else
            {
                Message("Retrive lifter data or type error", MessageType.Error);
            }
        }
        public void SupplyLifterAction(object obj)
        {
            Lifter lifter = obj as Lifter;
            if (lifter != null && lifter.Type == (int)LifterType.Supply)
            {
                try
                {
                    SupplyLifterStatus status = (SupplyLifterStatus)Convert.ToInt32(lifter.Status);
                    DbAccess db = new DbAccess();
                    string carrierCode = string.Empty;
                    long sensors = 0;
                    string strSensor = string.Empty;
                    if (Sensors.ReadSensors(ref sensors))
                    {
                        strSensor = Convert.ToString(sensors, 2).PadLeft(16, '0');
                    }
                    else
                    {
                        Message("Supply lifter: " + lifter.Code + " read sensors fail", MessageType.Error);
                    }
                    if (db.Open())
                    {
                        switch (status)
                        {
                            case SupplyLifterStatus.Loading:
                                if (Motors.MoveToPosition(LifterMotorPosition.DownPosition))     //电机去接料位
                                {
                                    if (db.SetSupplyLifterStatus(SupplyLifterStatus.Load, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " set status to Load fail", MessageType.Error);
                                    }
                                }
                                break;
                            case SupplyLifterStatus.Load:
                                if (strSensor[LifterSensors.MaterialCustom] == LifterSensors.SensorOn)   //判断客户Buffer上是否有料车
                                {
                                    if (db.SetSupplyLifterStatus(SupplyLifterStatus.Ready, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " set status to Ready fail", MessageType.Error);
                                    }
                                }
                                break;
                            case SupplyLifterStatus.Ready:
                                if (Motors.ChainMoving(LifterMoveType.Pull) != LifterMotorStatus.Default)
                                {
                                    if (db.SetSupplyLifterStatus(SupplyLifterStatus.ChainRoll, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " set status to ChainRoll fail", MessageType.Error);
                                    }
                                }
                                break;
                            case SupplyLifterStatus.ChainRoll:
                                if (Motors.ChainMoving(LifterMoveType.Pull) == LifterMotorStatus.Stopped)
                                {
                                    if (db.AssignMaterialsToLifter(lifter.Code))    //物料指派给升降机
                                    {
                                        if (db.SetSupplyLifterStatus(SupplyLifterStatus.Pullin, lifter.Code))
                                        {
                                        }
                                        else
                                        {
                                            Message("Supply lifter: " + lifter.Code + " set status to Pullin fail", MessageType.Error);
                                        }
                                    }
                                    else
                                    {
                                        Message("Assign Materials to " + lifter.Code + " fail", MessageType.Error);
                                    }
                                }
                                break;
                            case SupplyLifterStatus.Pullin:
                                if (Motors.MoveToPosition(LifterMotorPosition.UpPosition))
                                {
                                    if (db.SetSupplyLifterStatus(SupplyLifterStatus.Avoiding, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " set status to Avoiding fail", MessageType.Error);
                                    }
                                }
                                break;
                            case SupplyLifterStatus.Avoiding:
                                if (Motors.MoveToPosition(LifterMotorPosition.UpPosition))
                                {
                                    if (db.SetSupplyLifterStatus(SupplyLifterStatus.Avoid, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " set status to Avoid fail", MessageType.Error);
                                    }
                                }
                                break;
                            case SupplyLifterStatus.Avoid:
                                if (db.ExistMaterialRequests() > 0)
                                {
                                    carrierCode = db.GetFirstCarrierInLifterQueuebyCode(lifter.Code, (LifterType)lifter.Type);
                                    if (Fms.GetAgvInfo(carrierCode) == AgvState.IDLE.ToString() && 
                                        Fms.GetAgvStation(carrierCode) == lifter.Position)   //用FMS系统判断搬送车是否已经到位
                                    {
                                        if (db.SetSupplyLifterStatus(SupplyLifterStatus.Arrive, lifter.Code))
                                        {
                                        }
                                        else
                                        {
                                            Message("Supply lifter: " + lifter.Code + " set status to Arrive fail", MessageType.Error);
                                        }
                                    }
                                }
                                break;
                            case SupplyLifterStatus.Arrive:
                                if (Motors.MoveToPosition(LifterMotorPosition.DownPosition))    //升降机去下料位
                                {
                                    if (db.SetSupplyLifterStatus(SupplyLifterStatus.Unloading, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " set status to Unloading fail", MessageType.Error);
                                    }
                                }
                                break;
                            case SupplyLifterStatus.Unloading:
                                if (Motors.MoveToPosition(LifterMotorPosition.DownPosition))
                                {
                                    carrierCode = db.GetFirstCarrierInLifterQueuebyCode(lifter.Code, LifterType.Supply);
                                    if (carrierCode != null && carrierCode != string.Empty)
                                    {
                                        if (db.AssignMaterialsToCarrier(lifter.Code, carrierCode))     //物料分配到搬送车
                                        {
                                            int res = db.AssignMaterialsTargetOnCarrier(carrierCode);      //更改物料请求表，分配每个物料的目的地
                                            if (res == 0)
                                            {
                                                if (db.SetSupplyLifterStatus(SupplyLifterStatus.Unload, lifter.Code))
                                                {
                                                }
                                                else
                                                {
                                                    Message("Supply lifter: " + lifter.Code + " set status to Unload fail", MessageType.Error);
                                                }
                                            }
                                            else if (res == -1)
                                            {
                                                Message("Assign materials from carrier: " + carrierCode + " to device fail", MessageType.Error);
                                            }
                                            else if (res == 1)
                                            {
                                                Message("There is no request right now", MessageType.Error);
                                            }
                                        }
                                        else
                                        {
                                            Message("Assign materials from lifter: " + lifter.Code + " to carrier fail", MessageType.Error);
                                        }
                                    }
                                    else
                                    {
                                        Message("Get carrier in lifter queue fail", MessageType.Error);
                                    }
                                }
                                break;
                            case SupplyLifterStatus.Unload:
                                //满载的Carrier离开时，Carrier会将Lifter的状态设置为Loading
                                carrierCode = db.GetFirstCarrierInLifterQueuebyCode(lifter.Code, (LifterType)Convert.ToInt32(lifter.Type));
                                if (carrierCode != null && carrierCode != string.Empty)
                                {
                                    if (db.GetCarrierStatus(carrierCode) == (int)CarrierStatus.Initing)
                                    {
                                        db.SetCarrierStatus(carrierCode, CarrierStatus.Full);
                                    }
                                }
                                break;
                            default:
                                Message("Supply lifter: " + lifter.Code + " set status error", MessageType.Error);
                                break;
                        }
                    }
                    else
                    {
                        Message("Supply lifter: " + lifter.Code + " database access error", MessageType.Error);
                    }
                    db.Close();
                }
                catch (Exception)
                {
                    Message("Supply lifter: " + lifter.Code + " action fail", MessageType.Error);
                }
             }
            else
            {
                Message("Supply lifter data or type error", MessageType.Error);
            }
        }
    }

    public enum SupplyLifterStatus
    {
        Loading = 0,                    //升降机下降到接料位
        Load = 1,                         //到达接料位(空闲)
        //Wait = 2,                           //等待供料系统响应
        Ready = 2,                      //升降机的Buffer上有料车
        ChainRoll = 3,                  //链条转动
        Pullin = 4,                         //料车到达升降机(链条停止)
        Avoiding = 5,                   //升降机上升到避让位
        Avoid = 6,                          //到达避让位(等搬送车)，此状态搬送车可进入
        Arrive = 7,                         //搬送车到位
        Unloading = 8,                  //升降机下降到下料位
        Unload = 9,                    //到达下料位(等搬送车)
    }

    public enum RetriveLifterStatus
    {
        Loading = 0,                           //升降机前往接料位
        Load = 1,                               //到达接料位(空闲)，此状态搬送车可进入
        Arrive = 2,                             //搬送车到达
        Avoiding = 3,                       //升降机上升到避让位
        Avoid = 4,                              //到达避让位(等搬送车)
        Leave = 5,                              //搬送车已离开
        Unloading = 6,                      //升降机下降到下料位
        Unload = 7,                             //到达下料位
        //Wait = 8,                               //等待供料系统响应
        Ready = 8,                              //Buffer上没有料车
        ChainRoll = 9,                      //链条转动
    }

    public enum LifterType
    {
        None = 0,
        Retrive = 1,                    //回收升降机
        Supply = 2,                     //来料升降机
    }
}
