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
        { }

        public string Code { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string Parking { get; set; } //弃用
        public int QueuePosition { get; set; }
        public int Position { get; set; }

        public delegate void SendMessage(string msg, MessageType mt);
        public SendMessage Message;

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
                    if (db.Open())
                    {
                        switch (status)
                        {
                            case RetriveLifterStatus.Loading:
                                if (true)   //用Sensor判断升降机已到达接料位
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
                                carrierCode = db.GetFirstCarrierInLifterQueuebyCode(lifter.Code, (LifterType)Convert.ToInt32(lifter.Type));
                                if (carrierCode != null && carrierCode != string.Empty)
                                {
                                    if (true)   //用Sensor判断搬送车到位
                                    {
                                        if (db.SetRetriveLifterStatus(RetriveLifterStatus.Arrive, lifter.Code))
                                        {
                                        }
                                        else
                                        {
                                            Message("Retrive lifter: " + lifter.Code + " set status to Arrive fail", MessageType.Error);
                                        }
                                    }
                                }
                                break;
                            case RetriveLifterStatus.Arrive:
                                //升降机Z轴去避让位
                                if (db.SetRetriveLifterStatus(RetriveLifterStatus.Avoiding, lifter.Code))
                                {
                                }
                                else
                                {
                                    Message("Retrive lifter: " + lifter.Code + " set status to Avoiding fail", MessageType.Error);
                                }
                                break;
                            case RetriveLifterStatus.Avoiding:
                                if (true)   //电机判断是否已到达避让位
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Avoid, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Avoid fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                    //升降机Z轴去避让位
                                }
                                break;
                            case RetriveLifterStatus.Avoid:
                                if (true) //用Sensor判断搬送车是否已经离开
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Leave, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Leave fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                    carrierCode = db.GetFirstCarrierInLifterQueuebyCode(lifter.Code, (LifterType)Convert.ToInt32(lifter.Type));
                                    if (carrierCode != null && carrierCode != string.Empty)
                                    {
                                        db.SetCarrierStatus(carrierCode, CarrierStatus.Idle);
                                        db.LifterQueueDeleteZero(lifter.Code);
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " get carrier code fail at avoid position", MessageType.Error);
                                    }
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
                                if (true)   //电机判断是否已到达下料位
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Unload, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Unload fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                    //升降机Z轴去下料位
                                }
                                break;
                            case RetriveLifterStatus.Unload:
                                if (true)   //判断Buffer上是否不存在料车
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
                                //转动链条，把料车送往Buffer
                                if (db.SetRetriveLifterStatus(RetriveLifterStatus.ChainRoll, lifter.Code))
                                {
                                }
                                else
                                {
                                    Message("Retrive lifter: " + lifter.Code + " set status to ChainRoll fail", MessageType.Error);
                                }
                                break;
                            case RetriveLifterStatus.ChainRoll:
                                if (true) //用Sensor判断料车已经离开升降机
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
                    if (db.Open())
                    {
                        switch (status)
                        {
                            case SupplyLifterStatus.Loading:
                                if (true)   //电机判断是否已到达接料位
                                {
                                    if (db.SetSupplyLifterStatus(SupplyLifterStatus.Load, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " set status to Load fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                    //电机去接料位
                                }
                                break;
                            case SupplyLifterStatus.Load:
                                if (true)   //判断Buffer上是否有料车
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
                                //转动链条
                                if (db.SetSupplyLifterStatus(SupplyLifterStatus.ChainRoll, lifter.Code))
                                {
                                }
                                else
                                {
                                    Message("Supply lifter: " + lifter.Code + " set status to ChainRoll fail", MessageType.Error);
                                }
                                break;
                            case SupplyLifterStatus.ChainRoll:
                                if (true)   //用Sensor判断料车已经到达升降机
                                {
                                    if (db.AssignMaterialsToLifter(lifter.Code))    //物料指派给升降机
                                    {
                                    }
                                    else
                                    {
                                        Message("Assign Materials to " + lifter.Code + " fail", MessageType.Error);
                                    }

                                    if (db.SetSupplyLifterStatus(SupplyLifterStatus.Pullin, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " set status to Pullin fail", MessageType.Error);
                                    }
                                }
                                break;
                            case SupplyLifterStatus.Pullin:
                                //电机去避让位
                                if (db.SetSupplyLifterStatus(SupplyLifterStatus.Avoiding, lifter.Code))
                                {
                                }
                                else
                                {
                                    Message("Supply lifter: " + lifter.Code + " set status to Avoiding fail", MessageType.Error);
                                }
                                break;
                            case SupplyLifterStatus.Avoiding:
                                if (true)   //电机判断是否到达避让位
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
                                    if (true)   //用Sensor判断搬送车是否已经到位
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
                                //升降机去下料位
                                if (db.SetSupplyLifterStatus(SupplyLifterStatus.Unloading, lifter.Code))
                                {
                                }
                                else
                                {
                                    Message("Supply lifter: " + lifter.Code + " set status to Unloading fail", MessageType.Error);
                                }
                                break;
                            case SupplyLifterStatus.Unloading:
                                if (true)   //电机判断是否到达下料位
                                {
                                    if (db.SetSupplyLifterStatus(SupplyLifterStatus.Unload, lifter.Code))
                                    {
                                        string carrCode = db.GetFirstCarrierInLifterQueuebyCode(lifter.Code, LifterType.Supply);
                                        if (carrCode != null && carrCode != string.Empty)
                                        {
                                            if (db.AssignMaterialsToCarrier(lifter.Code, carrCode))     //物料分配到搬送车
                                            {
                                                if (db.AssignMaterialsTargetOnCarrier(carrCode))      //更改物料请求表，分配每个物料的目的地
                                                {
                                                }
                                                else
                                                {
                                                    Message("Assign materials from carrier: " + carrCode + " to device fail", MessageType.Error);
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
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " set status to Unload fail", MessageType.Error);
                                    }
                                }
                                break;
                            case SupplyLifterStatus.Unload:
                                if (true)   //用Sensor判断搬送车是否已离开
                                {
                                    if (db.SetSupplyLifterStatus(SupplyLifterStatus.Loading, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " set status to Loading fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                    carrierCode = db.GetFirstCarrierInLifterQueuebyCode(lifter.Code, (LifterType)Convert.ToInt32(lifter.Type));
                                    if (carrierCode != null && carrierCode != string.Empty)
                                    {
                                        db.SetCarrierStatus(carrierCode, CarrierStatus.Full);
                                        db.LifterQueueDeleteZero(lifter.Code);
                                    }
                                    else
                                    {
                                        Message("Supply lifter: " + lifter.Code + " get carrier code fail at unload position", MessageType.Error);
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
        Loading = 0,                           //升降机下降到接料位
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
