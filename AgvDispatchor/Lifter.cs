using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class Lifter
    {
        public Lifter()
        { }

        public string Code { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Parking { get; set; } //弃用

        public delegate void SendMessage(string msg, MessageType mt);
        public SendMessage Message;
        public void RetriveLifterAction(object obj)
        {
            Lifter lifter = obj as Lifter;
            if (lifter != null && lifter.Type == ((int)LifterType.Retrive).ToString())
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
                                    //让搬送车离开
                                    carrierCode = db.GetFirstCarrierInLifterQueuebyCode(lifter.Code, (LifterType)Convert.ToInt32(lifter.Type));
                                    db.SetCarrierStatus(carrierCode, CarrierStatus.Idle);
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
                                //向仓库供料系统发出下料请求
                                if (db.SetRetriveLifterStatus(RetriveLifterStatus.Wait, lifter.Code))
                                {
                                }
                                else
                                {
                                    Message("Retrive lifter: " + lifter.Code + " set status to Wait fail", MessageType.Error);
                                }
                                break;
                            case RetriveLifterStatus.Wait:
                                if (true)   //收到供料系统回应
                                {
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Response, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Response fail", MessageType.Error);
                                    }
                                }
                                break;
                            case RetriveLifterStatus.Response:
                                //转动链条，把料车送往仓库供料系统
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
                                    if (db.SetRetriveLifterStatus(RetriveLifterStatus.Pushout, lifter.Code))
                                    {
                                    }
                                    else
                                    {
                                        Message("Retrive lifter: " + lifter.Code + " set status to Pushout fail", MessageType.Error);
                                    }
                                }
                                break;
                            case RetriveLifterStatus.Pushout:
                                //链条停止
                                if (db.SetRetriveLifterStatus(RetriveLifterStatus.Loading, lifter.Code))
                                {
                                }
                                else
                                {
                                    Message("Retrive lifter: " + lifter.Code + " set status to Loading fail", MessageType.Error);
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
            if (lifter != null && lifter.Type == ((int)LifterType.Supply).ToString())
            {

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
        Load = 1,                         //到达接料位(空闲，可发请求)
        Wait = 2,                           //等待供料系统响应
        Response = 3,                   //接到系统响应
        ChainRoll = 4,                  //链条转动
        Pullin = 5,                         //料车到达升降机(链条停止)
        Avoiding = 6,                   //升降机上升到避让位
        Avoid = 7,                          //到达避让位(等搬送车)，此状态搬送车可进入
        Arrive = 8,                         //搬送车到位
        Unloading = 9,                  //升降机下降到下料位
        Unload = 10,                    //到达下料位(等搬送车)
        Leave = 11,                     //搬送车已离开
    }

    public enum RetriveLifterStatus
    {
        Loading = 0,                           //升降机下降到接料位
        Load = 1,                               //到达接料位(空闲，等搬送车)，此状态搬送车可进入
        Arrive = 2,                           //搬送车到达
        Avoiding = 3,                       //升降机上升到避让位
        Avoid = 4,                           //到达避让位(等搬送车)
        Leave = 5,                              //搬送车已离开
        Unloading = 6,                      //升降机下降到下料位
        Unload = 7,                         //到达下料位(发请求)
        Wait = 8,                               //等待供料系统响应
        Response = 9,                       //接到系统响应
        ChainRoll = 10,                     //链条转动
        Pushout = 11,                       //料车已离开升降机(链条停止)
    }

    public enum LifterType
    {
        None = 0,
        Retrive = 1,                    //回收升降机
        Supply = 2,                     //来料升降机
    }
}
