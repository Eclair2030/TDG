using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    public class LifterMotors
    {
        public LifterMotors()
        {
            Sensors = new LifterSensors();
            Sensor_State = string.Empty;
            Current_Motor_Position = LifterMotorPosition.Default;
        }

        public bool InitMethods(string leftUM, string leftDM, string rightUM, string rightDM, string leftPushM, string leftPullM, string rightPushM, string rightPullM)
        {
            bool result = false;
            Type t = typeof(DLL);
            LeftUp = t.GetMethod(leftUM);
            LeftDown = t.GetMethod(leftDM);
            RightUp = t.GetMethod(rightUM);
            RightDown = t.GetMethod(rightDM);
            LeftPush = t.GetMethod(leftPushM);
            LeftPull = t.GetMethod(leftPullM);
            RightPush = t.GetMethod(rightPushM);
            RightPull = t.GetMethod(rightPullM);
            if (LeftUp != null && LeftDown != null && RightUp != null && RightDown != null &&
                LeftPush != null && LeftPull != null && RightPush != null && RightPull != null)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 电机运动到目标位置
        /// </summary>
        /// <param name="position">目标位置</param>
        /// <returns></returns>
        public bool MoveToPosition(LifterMotorPosition position)
        {
            bool result = false;
            if (RefreshCurrentPosition())
            {
                if ((int)Current_Motor_Position < (int)LifterMotorPosition.UpPosition)
                {
                    if (Sensor_State[LifterSensors.LdownLimit] == LifterSensors.SensorOff)
                    {
                        if (Move(LifterMoveType.Down, LiterMove.Move))
                        {
                        }
                    }
                    else
                    {
                        if (Move(LifterMoveType.Down, LiterMove.Stop) && Move(LifterMoveType.Up, LiterMove.Move))
                        {
                        }
                    }
                    if (Sensor_State[MotorPositionToSensorIndex(position)] == LifterSensors.SensorOn)
                    {
                        if (Move(LifterMoveType.Down, LiterMove.Stop) && Move(LifterMoveType.Up, LiterMove.Stop))
                        {
                            Current_Motor_Position = position;
                            result = true;
                        }
                    }
                }
                else if ((int)Current_Motor_Position > (int)LifterMotorPosition.UpPosition)
                {
                    if (Sensor_State[LifterSensors.LupLimit] == LifterSensors.SensorOff)
                    {
                        if (Move(LifterMoveType.Up, LiterMove.Move))
                        {
                        }
                    }
                    else
                    {
                        if (Move(LifterMoveType.Up, LiterMove.Stop) && Move(LifterMoveType.Down, LiterMove.Move))
                        {
                        }
                    }
                    if (Sensor_State[MotorPositionToSensorIndex(position)] == LifterSensors.SensorOn)
                    {
                        if (Move(LifterMoveType.Down, LiterMove.Stop) && Move(LifterMoveType.Up, LiterMove.Stop))
                        {
                            Current_Motor_Position = position;
                            result = true;
                        }
                    }
                }
                else
                {
                    Message("lifter motor in " + position + " right now", MessageType.Default);
                }
            }
            else
            {
                Message("lifter refresh sensor states fail", MessageType.Error);
            }
            return result;
        }

        public LifterMotorStatus ChainMoving(LifterMoveType type)
        {
            LifterMotorStatus result = LifterMotorStatus.Default;
            if (GetSensorStates())
            {
                switch (type)
                {
                    case LifterMoveType.Push:
                        if (Sensor_State[LifterSensors.MaterialHas] == LifterSensors.SensorOn)
                        {
                            if (Move(LifterMoveType.Push, LiterMove.Move))
                            {
                                result = LifterMotorStatus.Moving;
                            }
                        }
                        else
                        {
                            if (Move(LifterMoveType.Push, LiterMove.Stop))
                            {
                                result = LifterMotorStatus.Stopped;
                            }
                        }
                        break;
                    case LifterMoveType.Pull:
                        if (Sensor_State[LifterSensors.MaterialArrive] == LifterSensors.SensorOff)
                        {
                            if (Move(LifterMoveType.Pull, LiterMove.Move))
                            {
                                result = LifterMotorStatus.Moving;
                            }
                        }
                        else
                        {
                            if (Move(LifterMoveType.Pull, LiterMove.Stop))
                            {
                                result = LifterMotorStatus.Stopped;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        public bool ManualMove(LifterMoveType type, LiterMove power)
        {
            bool result = false;
            object[] para = new object[2] { DLL.Device_IOCard, (int)power };
            switch (type)
            {
                case LifterMoveType.Up:
                    if (LeftUp != null && RightUp != null)
                    {
                        LeftUp.Invoke(null, para);
                        RightUp.Invoke(null, para);
                        result = true;
                    }
                    else
                    {
                        Message("Lifter motor move / stop up fail", MessageType.Error);
                    }
                    break;
                case LifterMoveType.Down:
                    if (LeftDown != null && RightDown != null)
                    {
                        LeftDown.Invoke(null, para);
                        RightDown.Invoke(null, para);
                        result = true;
                    }
                    else
                    {
                        Message("Lifter motor move / stop down fail", MessageType.Error);
                    }
                    break;
                case LifterMoveType.Push:
                    if (LeftPush != null && RightPush != null)
                    {
                        LeftPush.Invoke(null, para);
                        RightPush.Invoke(null, para);
                        result = true;
                    }
                    else
                    {
                        Message("Lifter motor push / stop fail", MessageType.Error);
                    }
                    break;
                case LifterMoveType.Pull:
                    if (LeftPull != null && RightPull != null)
                    {
                        LeftPull.Invoke(null, para);
                        RightPull.Invoke(null, para);
                        result = true;
                    }
                    else
                    {
                        Message("Lifter motor pull / stop fail", MessageType.Error);
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// 电机运动
        /// </summary>
        /// <param name="type">0Up，1Down，2Push，3Pull</param>
        /// <param name="power">0运动，1停止</param>
        /// <returns></returns>
        private bool Move(LifterMoveType type, LiterMove power)
        {
            bool result = false;
            object[] para = new object[2] { DLL.Device_IOCard, (int)power };
            switch (type)
            {
                case LifterMoveType.Up:
                    if (Sensor_State[LifterSensors.LupLimit] != LifterSensors.SensorOn)
                    {
                        if (LeftUp != null && RightUp != null)
                        {
                            LeftUp.Invoke(null, para);
                            RightUp.Invoke(null, para);
                            result = true;
                        }
                        else
                        {
                            Message("Lifter motor move / stop up fail", MessageType.Error);
                        }
                    }
                    else
                    {
                        Message("Lifter motor has reached up limit", MessageType.Error);
                    }
                    break;
                case LifterMoveType.Down:
                    if (LeftDown != null && RightDown != null)
                    {
                        LeftDown.Invoke(null, para);
                        RightDown.Invoke(null, para);
                        result = true;
                    }
                    else
                    {
                        Message("Lifter motor move / stop down fail", MessageType.Error);
                    }
                    break;
                case LifterMoveType.Push:
                    if (LeftPush != null && RightPush != null)
                    {
                        LeftPush.Invoke(null, para);
                        RightPush.Invoke(null, para);
                        result = true;
                    }
                    else
                    {
                        Message("Lifter motor push / stop fail", MessageType.Error);
                    }
                    break;
                case LifterMoveType.Pull:
                    if (LeftPull != null && RightPull != null)
                    {
                        LeftPull.Invoke(null, para);
                        RightPull.Invoke(null, para);
                        result = true;
                    }
                    else
                    {
                        Message("Lifter motor pull / stop fail", MessageType.Error);
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        public bool GetSensorStates()
        {
            bool result = false;
            long sensor = 0;
            if (Sensors.ReadSensors(ref sensor))
            {
                Sensor_State = Convert.ToString(sensor, 2).PadLeft(16, '0');
                result = true;
            }
            else
            {
                Message("lifter read sensors fail", MessageType.Error);
            }
            return result;
        }

        private bool RefreshCurrentPosition()
        {
            bool result = GetSensorStates();
            if (result)
            {
                if (Sensor_State[LifterSensors.LupLimit] == LifterSensors.SensorOn)
                {
                    Current_Motor_Position = LifterMotorPosition.UpLimit;
                }
                else if (Sensor_State[LifterSensors.LupPos] == LifterSensors.SensorOn)
                {
                    Current_Motor_Position = LifterMotorPosition.UpPosition;
                }
                else if (Sensor_State[LifterSensors.LdownPos] == LifterSensors.SensorOn)
                {
                    Current_Motor_Position = LifterMotorPosition.DownPosition;
                }
                else if (Sensor_State[LifterSensors.LdownLimit] == LifterSensors.SensorOn)
                {
                    Current_Motor_Position = LifterMotorPosition.DownLimit;
                }
            }
            return result;
        }

        private int MotorPositionToSensorIndex(LifterMotorPosition position)
        {
            int index = -1;
            switch (position)
            {
                case LifterMotorPosition.UpLimit:
                    index = LifterSensors.LupLimit;
                    break;
                case LifterMotorPosition.UpPosition:
                    index = LifterSensors.LupPos;
                    break;
                case LifterMotorPosition.DownPosition:
                    index = LifterSensors.LdownPos;
                    break;
                case LifterMotorPosition.DownLimit:
                    index = LifterSensors.LdownLimit;
                    break;
                default:
                    break;
            }
            return index;
        }

        public MethodInfo LeftUp, LeftDown;
        public MethodInfo RightUp, RightDown;
        public MethodInfo LeftPush, LeftPull;
        public MethodInfo RightPush, RightPull;
        public SendMessage Message;
        private LifterSensors Sensors;
        private LifterMotorPosition Current_Motor_Position;
        private string Sensor_State;
    }

    public enum LifterMoveType
    {
        Up = 0,
        Down = 1,
        Push = 2,
        Pull = 3,
    }

    public enum LiterMove
    {
        Move = 0,
        Stop = 1,
    }

    public enum LifterMotorPosition
    {
        UpLimit = 1,                                                //上限位置
        ULimitToUPosition = 2,                              //上限位与抬升为之间的位置
        UpPosition = 3,                                            //抬升位置
        UPositionToDPosition = 4,                       //抬升位置与下降位置之间的位置
        Default = 5,                                                 //默认位置，不知道位置时，当作UPositionToDPosition处理
        DownPosition = 6,                                       //下降位置
        DPositionToDLimit = 7,                              //下降位置与下限位之间的位置
        DownLimit = 8,                                          //下限位置
    }

    public enum LifterMotorStatus
    {
        Default = 0,
        Moving = 1,
        Stopped = 2,
    }
}
