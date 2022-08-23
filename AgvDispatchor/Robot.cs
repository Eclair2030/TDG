using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    internal class Robot
    {
        public Robot()
        {
            Paw = null;
        }

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
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public List<RobotPoint> Points { get; set; }

        public Socket Paw;

        public static float LOW_POWER = 15f;
        public static float HIGH_POWER = 95f;
        public static int MAX_DEVICE_INDEX = 7;
        public static int MAX_CARRIER_INDEX = 17;
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

            if (st != RobotStatus.ArmWorking)
            {
                switch (st)
                {
                    case RobotStatus.Moving:
                        int pos = Db.GetRobotTargetPosition(robot.Code, robot.Coordination);
                        if (pos != -1)
                        {
                            if (fms.GetAgvInfo(robot.Code) == AgvState.IDLE.ToString())
                            {
                                int cur = fms.GetAgvStation(robot.Code);
                                if (cur != -1 && cur != pos)
                                {
                                    if (fms.AgvMove(robot.Code, pos) == FmsActionResult.Success)
                                    {
                                    }
                                    else
                                    {
                                        Message("Robot: " + robot.Code + " move to position " + pos + " fail", MessageType.Error);
                                    }
                                }
                                else if (cur == pos)
                                {
                                    if (Db.SetRobotStatus(robot.Code, RobotStatus.Ready))
                                    {
                                    }
                                    else
                                    {
                                        Message("Robot: " + robot.Code + " status set to ready, process set to 0 fail", MessageType.Error);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Message("Robot: " + robot.Code + " get target position fail", MessageType.Error);
                        }
                        break;
                    case RobotStatus.Ready:
                        int status = Db.GetCarrierStatusByRobot(robot.Code, robot.Coordination);
                        if (status == (int)CarrierStatus.Unloading)
                        {
                            if (fms.GetAgvInfo(robot.Code) == AgvState.IDLE.ToString())
                            {
                                if (Db.SetRobotIndexs(robot.Code, 0, 0))
                                {
                                    if (Db.SetRobotStatus(robot.Code, RobotStatus.ArmWorking))
                                    {
                                    }
                                    else
                                    {
                                        Message("Robot: " + robot.Code + " status set to SnapDevice, process set to Device2Buffer fail", MessageType.Error);
                                    }
                                }
                                else
                                {
                                    Message("Robot: " + robot.Code + " set DeviceIndex and CarrierIndex to 0 fail", MessageType.Error);
                                }
                            }
                        }
                        break;
                    case RobotStatus.Idle:
                        break;
                    case RobotStatus.Standby:
                        break;
                    case RobotStatus.Charge:
                        break;
                    case RobotStatus.Charging:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (robot.Arm)
                {
                    case 0: //无料
                        int tarCode, tarArea, tarIndex;
                        string carrCode;
                        int carrIndex = Db.GetFirstMaterialOnCarrier(robot.Code, out carrCode, out tarCode, out tarArea, out tarIndex);
                        if (carrIndex != -1 && carrCode != null)
                        {
                            int staff = Db.GetDeviceStaffState(tarCode, tarArea, tarIndex);
                            switch (staff)
                            {
                                case 0:     //抓取Carrier物料

                                    if (Db.SetRobotArmStatus(robot.Code, 2))
                                    {
                                    }
                                    else
                                    {
                                        Message("Set robot: " + robot.Code + " arm status to 2 fail", MessageType.Error);
                                    }
                                    break;
                                case 1:     //抓取设备空料

                                    if (Db.SetRobotArmStatus(robot.Code, 1))
                                    {
                                    }
                                    else
                                    {
                                        Message("Set robot: " + robot.Code + " arm status to 2 fail", MessageType.Error);
                                    }
                                    break;
                                case 2:     //信息冲突，修改Carrier物料目标位置
                                    if (Db.ReassignMaterials(tarCode, tarArea, tarIndex, carrCode, carrIndex))
                                    {
                                    }
                                    else
                                    {
                                        Message("Reassign material target position fail", MessageType.Error);
                                    }
                                    break;
                                default:    //报错
                                    Message("Set robot: " + robot.Code + " get target device staff status error: " + staff, MessageType.Error);
                                    break;
                            }
                        }
                        else
                        {
                            //回收Buffer后结束
                        }
                        break;
                    case 1: //空料
                        break;
                    case 2: //满料
                        break;
                    default:
                        break;
                }
            }


            if (rp == RobotProcess.None)
            {
            }
            else
            {
                switch (robot.Arm)
                {
                    case 0:     //无料
                        switch (st)
                        {
                            case RobotStatus.DeviceWork:
                                if (robot.MovetoBasePosition(RobotBasePosition.DeviceSnap))
                                {
                                    if (robot.MoveOffset(RobotAxis.Z, robot.DeviceIndex % Material.TOTAL_MATERIAL_ONE_POSITION))
                                    {
                                        if (true)//拍照成功
                                        {
                                            if (true)//视觉检测有料
                                            {
                                                if (robot.MovetoBasePosition(RobotBasePosition.DeviceWork)) //带视觉补正数据的位置
                                                {
                                                    if (true)   //卡爪抓取产品
                                                    {
                                                        if (Db.SetRobotStatus(robot.Code, RobotStatus.DeviceFinish))
                                                        {
                                                        }
                                                        else
                                                        {
                                                            Message("robot: " + robot.Code + " status set to DeviceFinish fail", MessageType.Error);
                                                        }
                                                        if (robot.MovetoBasePosition(RobotBasePosition.DeviceWait))
                                                        {
                                                            if (Db.SetRobotStatus(robot.Code, RobotStatus.BufferWork))
                                                            {
                                                            }
                                                            else
                                                            {
                                                                Message("robot: " + robot.Code + " status set to BufferWork fail", MessageType.Error);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Message("robot: " + robot.Code + " catch empty on device fail", MessageType.Error);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //空杆
                                                if (Db.SetRobotStatusAndProcess(robot.Code, RobotStatus.CarrierWork, (int)RobotProcess.Carrier2Device))
                                                {
                                                }
                                                else
                                                {
                                                    Message("robot: " + robot.Code + " status set to SnapCarrier fail", MessageType.Error);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Message("robot: " + robot.Code + " camera snap fail", MessageType.Error);
                                        }
                                    }
                                }
                                break;
                            case RobotStatus.DeviceFinish:
                                if (robot.DeviceIndex == Material.TOTAL_MATERIAL_ONE_POSITION - 1)
                                {

                                }
                                else
                                {

                                }
                                break;
                            case RobotStatus.CarrierWork:
                                break;
                            case RobotStatus.CarrierFinish:
                                break;
                            case RobotStatus.BufferWork:
                                break;
                            case RobotStatus.BufferFinish:
                                break;
                            default:
                                Message("robot: " + robot.Code + " process code error when null catch", MessageType.Error);
                                break;
                        }
                        break;
                    case 1:     //空料
                        switch (rp)
                        {
                            case RobotProcess.Device2Carrier:
                            case RobotProcess.Buffer2Carrier:
                                break;
                            case RobotProcess.Device2Buffer:
                                break;
                            default:
                                Message("robot: " + robot.Code + " process code error when empty catch", MessageType.Error);
                                break;
                        }
                        break;
                    case 2:     //满料
                        if (rp == RobotProcess.Carrier2Device)
                        {

                        }
                        else
                        {
                            Message("robot: " + robot.Code + " process code error when full catch", MessageType.Error);
                        }
                        break;
                    default:
                        break;
                }
            }            
        }

        /// <summary>
        /// 机械手上电+使能
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            bool result = false;
            try
            {
                IPAddress ip = IPAddress.Parse(IpAddress);
                IPEndPoint ep = new IPEndPoint(ip, Port);
                if (Paw == null)
                {
                    Paw = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                if (!Paw.Connected)
                {
                    Paw.Connect(ep);
                }
                if (Paw != null && Paw.Connected)
                {
                    string str = ReadCurFSM();
                    if (ReadCurFSM() == ((int)CurFsmState.Blackout_48V).ToString())
                    {
                        if (!Electrify())
                        {
                            result = false;
                        }
                    }
                    else if (ReadCurFSM() == ((int)CurFsmState.StandBy).ToString() || ReadCurFSM() == ((int)CurFsmState.Disable).ToString())
                    {
                        Message("Paw on robot: " + Code + " has Electrify already", MessageType.Default);
                    }
                    else
                    {
                        Message("Paw on robot happen error when Electrify", MessageType.Error);
                    }

                    if (ReadCurFSM() == ((int)CurFsmState.StandBy).ToString())
                    {
                        Message("Paw on robot: " + Code + " has PowerOn already", MessageType.Default);
                        result = true;
                    }
                    else
                    {
                        while (ReadCurFSM() != ((int)CurFsmState.Disable).ToString())
                        {
                            Thread.Sleep(500);
                        }
                        if (Powon())
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                }
                else
                {
                    Message("Paw on robot: " + Code + " socket status error", MessageType.Error);
                    result = false;
                }
            }
            catch (Exception)
            {
                Message("Paw on robot: " + Code + " init exception", MessageType.Exception);
                result = false;
            }
            return result;
        }

        private bool MovetoBasePosition(RobotBasePosition pos)
        {
            bool result = false;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    RobotPoint rp = new RobotPoint();
                    for (int i = 0; i < Points.Count; i++)
                    {
                        if (Points[i].PtName == pos.ToString())
                        {
                            rp = Points[i];
                            break;
                        }
                    }
                    string msg = "MoveJ,0," + rp.J1 + ", " + rp.J2 + "," + rp.J3 + "," + rp.J4 + "," + rp.J5 + "," + rp.J6 + ",;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    if (Encoding.ASCII.GetString(data).Contains("OK"))
                    {
                        msg = "ReadRobotState,0,;";
                        string state;
                        do
                        {
                            Paw.Send(Encoding.ASCII.GetBytes(msg));
                            data = new byte[Paw.ReceiveBufferSize];
                            Paw.Receive(data);
                            state = Encoding.ASCII.GetString(data);
                            state = state.Substring(state.IndexOf("movingState")+12, 1);
                            Thread.Sleep(300);
                        } while (state == "1");
                        Message("Robot: " + Code + " reach the base point " + pos, MessageType.Result);
                        result = true;
                    }
                    else
                    {
                        Message("Robot: " + Code + " move to base point " + pos + " fail", MessageType.Error);
                    }
                }
                else
                {
                    Init();
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        private bool MoveOffset(RobotAxis axis, double distance)
        {
            bool result = false;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "ReadActPos,0,;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    string[] msgArry = Encoding.ASCII.GetString(data).Split(',');
                    if (msgArry[1] == "OK")
                    {
                        double x = Convert.ToDouble(msgArry[8]);
                        double y = Convert.ToDouble(msgArry[9]);
                        double z = Convert.ToDouble(msgArry[10]);
                        double rx = Convert.ToDouble(msgArry[11]);
                        double ry = Convert.ToDouble(msgArry[12]);
                        double rz = Convert.ToDouble(msgArry[13]);
                        switch (axis)
                        {
                            case RobotAxis.X:
                                x += distance;
                                break;
                            case RobotAxis.Y:
                                y += distance;
                                break;
                            case RobotAxis.Z:
                                z += distance;
                                break;
                            case RobotAxis.RX:
                            case RobotAxis.RY:
                            case RobotAxis.RZ:
                            default:
                                Message("Robot: " + Code + " Paw move offset axis parameter error", MessageType.Error);
                                break;
                        }
                        msg = "MoveL,rbtID," + x + "," + y + "," + z + "," + rx + "," + ry + "," + rz + ",;";
                        Paw.Send(Encoding.ASCII.GetBytes(msg));
                        data = new byte[Paw.ReceiveBufferSize];
                        Paw.Receive(data);
                        if (Encoding.ASCII.GetString(data).Contains("OK"))
                        {
                            msg = "ReadRobotState,0,;";
                            string state;
                            do
                            {
                                Paw.Send(Encoding.ASCII.GetBytes(msg));
                                data = new byte[Paw.ReceiveBufferSize];
                                Paw.Receive(data);
                                state = Encoding.ASCII.GetString(data);
                                state = state.Substring(state.IndexOf("movingState") + 12, 1);
                                Thread.Sleep(300);
                            } while (state == "1");
                            Message("Robot: " + Code + " axis: " + axis + " move offset " + distance, MessageType.Result);
                            result = true;
                        }
                        else
                        {
                            Message("Robot: " + Code + " Paw MoveL fail", MessageType.Error);
                        }
                    }
                    else
                    {
                        Message("Robot: " + Code + " Paw ReadActPos fail", MessageType.Error);
                    }
                }
                else
                {
                    Init();
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public bool MoveL(double x, double y, double z, double rx, double ry, double rz)
        {
            bool result = false;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "MoveL,0," + x + "," + y + "," + z + "," + rx + "," + ry + "," + rz + ",;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    string[] msgArry = Encoding.ASCII.GetString(data).Split(',');
                    if (msgArry[1] == "OK")
                    {
                        Message("Robot: " + Code + " Paw MoveL OK", MessageType.Result);
                    }
                    else
                    {
                        Message("Robot: " + Code + " Paw MoveL fail", MessageType.Error);
                    }
                }
                else
                {
                    Init();
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public bool MoveJ(double j1, double j2, double j3, double j4, double j5, double j6)
        {
            bool result = false;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "MoveJ,0," + j1 + "," + j2 + "," + j3 + "," + j4 + "," + j5 + "," + j6 + ",;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    string[] msgArry = Encoding.ASCII.GetString(data).Split(',');
                    if (msgArry[1] == "OK")
                    {
                        Message("Robot: " + Code + " Paw MoveJ OK", MessageType.Result);
                    }
                    else
                    {
                        Message("Robot: " + Code + " Paw MoveJ fail", MessageType.Error);
                    }
                }
                else
                {
                    Init();
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public string ReadRobotState(PawStatus state)
        {
            string result = null;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "ReadRobotState,0,;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    //Message(Encoding.ASCII.GetString(data), MessageType.Default);
                    string[] ary = Encoding.ASCII.GetString(data).Replace(";", "").Split(',');
                    result = ary[(int)state];
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public string ReadCurFSM()
        {
            string result = null;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "ReadCurFSM,0,;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    string[] ary = Encoding.ASCII.GetString(data).Replace(";", "").Split(',');
                    result = ary[2];
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public RobotPoint ReadActPos()
        {
            RobotPoint pos = null;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "ReadActPos,0,;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    string[] ary = Encoding.ASCII.GetString(data).Replace(";", "").Split(',');
                    pos = new RobotPoint();
                    pos.J1 = double.Parse(ary[2]);
                    pos.J2 = double.Parse(ary[3]);
                    pos.J3 = double.Parse(ary[4]);
                    pos.J4 = double.Parse(ary[5]);
                    pos.J5 = double.Parse(ary[6]);
                    pos.J6 = double.Parse(ary[7]);
                    pos.X = double.Parse(ary[8]);
                    pos.Y = double.Parse(ary[9]);
                    pos.Z = double.Parse(ary[10]);
                    pos.RX = double.Parse(ary[11]);
                    pos.RY = double.Parse(ary[12]);
                    pos.RZ = double.Parse(ary[13]);
                }
            }
            catch (Exception)
            {
                pos = null;
            }
            return pos;
        }

        public bool Electrify()
        {
            bool result = false;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "Electrify,;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    if (Encoding.ASCII.GetString(data).Contains("OK"))
                    {
                        Message("Robot: " + Code + " Electrify success", MessageType.Result);
                        result = true;
                    }
                    else
                    {
                        Message("Robot: " + Code + " Electrify fail", MessageType.Error);
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool Powon()
        {
            bool result = false;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "GrpPowerOn,0,;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    if (Encoding.ASCII.GetString(data).Contains("OK"))
                    {
                        while (ReadCurFSM() != ((int)CurFsmState.StandBy).ToString())
                        {
                            Thread.Sleep(200);
                        }
                        Message("Robot: " + Code + " PowerOn success", MessageType.Result);
                        result = true;
                    }
                    else
                    {
                        Message("Robot: " + Code + " PowerOn fail", MessageType.Error);
                    }
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public string Reset()
        {
            string result = null;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "GrpReset,0,;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    if (Encoding.ASCII.GetString(data).Contains("OK"))
                    {
                        Message("GrpReset success", MessageType.Result);
                    }
                    else
                    {
                        Message("GrpReset fail", MessageType.Error);
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public string ReadOverride()
        {
            string result = null;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "ReadOverride,0,;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    if (Encoding.ASCII.GetString(data).Contains("OK"))
                    {
                        result = Encoding.ASCII.GetString(data).Replace(";", "").Split(',')[2];
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public bool SetOverride(double value)
        {
            bool result = false;
            if (value < 0.1 || value > 1)
            {
                Message("SetOverride speed value: " + value + " out of range", MessageType.Error);
                return false;
            }
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "SetOverride,0," + value + ",;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    if (Encoding.ASCII.GetString(data).Contains("OK"))
                    {
                        Message("Robot: " + Code + " SetOverride: " + value, MessageType.Result);
                        result = true;
                    }
                    else
                    {
                        Message("Robot: " + Code + " SetOverride: " + value + " fail", MessageType.Error);
                        result = false;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public bool SetTCPByName(string name)
        {
            bool result = false;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "SetTCPByName,0," + name + ",;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    if (Encoding.ASCII.GetString(data).Contains("OK"))
                    {
                        Message("Robot: " + Code + " SetTCPByName by " + name + " success", MessageType.Result);
                        result = true;
                    }
                    else
                    {
                        Message("Robot: " + Code + " SetTCPByName by " + name + " fail", MessageType.Error);
                        result = false;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public bool ReadTCPByName(string name)
        {
            bool result = false;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "ReadPointList,Point_" + name + ",;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    string str = Encoding.ASCII.GetString(data);
                    if (Encoding.ASCII.GetString(data).Contains("OK"))
                    {
                        Message("Robot: " + Code + " ReadTCPByName by " + name + " success", MessageType.Result);
                        Message(str, MessageType.Default);
                        result = true;
                    }
                    else
                    {
                        Message("Robot: " + Code + " ReadTCPByName by " + name + " fail", MessageType.Error);
                        result = false;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public string PCS2ACS(double x, double y, double z, double rx, double ry, double rz)
        {
            string result = null;
            try
            {
                if (Paw != null && Paw.Connected)
                {
                    string msg = "PCS2ACS,0," + x + "," + y + "," + z + "," + rx + "," + ry + "," + rz + ",0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,;";
                    Paw.Send(Encoding.ASCII.GetBytes(msg));
                    byte[] data = new byte[Paw.ReceiveBufferSize];
                    Paw.Receive(data);
                    if (Encoding.ASCII.GetString(data).Contains("OK"))
                    {
                        string[] ary = Encoding.ASCII.GetString(data).Replace(";", "").Split(',');
                        Message(ary[2] + ", " + ary[3] + ", " + ary[4] + ", " + ary[5] + ", " + ary[6] + ", " + ary[7], MessageType.Result);
                    }
                    else
                    {
                        Message("GrpReset fail", MessageType.Error);
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
    }

    public enum RobotStatus
    {
        Moving = 0,                         //去设备上料途中
        Ready = 1,                            //到达目标上料位置
        ArmWorking = 2,                 //手臂工作中
        DeviceWork = 2,                 //相机对设备的料位拍照 + 对位 + 机械手取料/放料
        DeviceFinish = 3,                //机械手对设备 抓住料/放下料 完成
        CarrierWork = 4,                //相机对Carrier的料位拍照 + 对位 + 机械手取料/放料
        CarrierFinish = 5,               //机械手对Carrier 抓住料/放下料 完成
        BufferWork = 6,                 //相机对作业车的Buffer拍照 + 对位 + 机械手取料/放料
        BufferFinish = 7,                //机械手对Buffer 抓住料/放下料 完成
        Idle = 8,                               //空闲，由充电队列模块置位到Standby或者Charge状态
        Standby = 9,                      //就绪，可以应答上料请求
        Charge = 10,                     //在充电队列中
        Charging = 11,                  //正在充电
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

    public enum PawStatus 
    { 
        RBTid = 0,                      //调用方法名
        Result = 1,                     //结果
        MovingState = 2,        //0未运动，1运动中
        PowerState = 3,             //0去使能状态，1使能状态
        ErrorState = 4,                 //0没有报错，1报错
        ErrorCode = 5,                  //错误码
        ErrorAxisID = 6,                //报错的轴ID
        BrakingState = 7,               //0没有抱闸，1抱闸工作中
        HoldingState = 8,               //
        EmergencyStop = 9,          //0没有急停，1急停
        SaftyGuard = 10,                 //0没有安全光幕，1安全光幕
        Electrify = 11,                     //0没上电，1上电
        IsConnectToBox = 12,        //0没连接电箱，1连接电箱
        BlendingDone = 13,            //waypoint中，0blending运动未完成，1blending运动完成
        Intpos = 14,                        //0,未到位(运动中或报错)，1到位
    }

    public enum CurFsmState
    {
        UnInitialize = 0,                                   //未初始化
        Initialize = 1,                                         //初始化
        ElectricBoxDisconnect = 2,              //与电箱控制板断开
        ElectricBoxConnecting = 3,              //连接电箱控制板
        EmergencyStopHandling = 4,          //急停处理中
        EmergencyStop = 5,                          //急停
        Blackouting48V = 6,                         //正在切断本体供电
        Blackout_48V = 7,                           //本体供电已切断
        Electrifying48V = 8,                        //正在准备给本体供电
        SaftyGuardErrorHandling = 9,        //安全光幕错误处理中
        SaftyGuardError = 10,                       //安全光幕错误
        SafetyGuardHandling = 11,           //安全光幕处理中
        SaftyGuard = 12,                                //安全光幕
        ControllerDisconnecting = 13,       //正在反初始化控制器
        ControllerDisconnect = 14,              //控制器已处理于未初始化状态
        ControllerConnecting = 15,              //正在初始化控制器
        ControllerVersionError = 16,            //控制器版本过低错误
        EtherCATError = 17,                             //EtherCAT 错误
        ControllerChecking = 18,                    //控制器初始化后检查
        Reseting = 19,                                      //正在复位机器人
        RobotOutofSafeSpace = 20,               //机器人超出安全空间
        RobotCollisionStop = 21,                    //机器人安全碰撞停车
        Error = 22,                                             //机器人错误
        RobotEnabling = 23,                         //机器人使能中
        Disable = 24,                                       //机器人去使能
        Moveing = 25,                                   //机器人运动中
        LongJogMoveing = 26,                    //机器人长点动运动中
        RobotStopping = 27,                         //机器人停止运动中
        RobotDisabling = 28,                        //机器人去使能中
        RobotOpeningFreeDriver = 29,        //机器人正在开启零力示教
        RobotClosingFreeDriver =30,         //机器人正在关闭零力示教
        FreeDriver = 31,                                    //机器人处于零力示教
        RobotHolding = 32,                          //机器人暂停
        StandBy =33,                                        //机器人就绪
        ScriptRunning = 34,                         //脚本运行中
        ScriptHoldHandling = 35,                //脚本暂停处理中
        ScriptHolding = 36,                         //脚本暂停
        ScriptStopping = 37,                        //脚本停止中
        ScriptStopped = 38,                         //脚本已停止
        HRAppDisconnected = 39,             //HRApp 部件断开
        HRAppError = 40,                              //HRApp 部件错误
    }
}
