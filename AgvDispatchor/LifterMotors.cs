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
        { }

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
        /// 电机运动
        /// </summary>
        /// <param name="type">0Up，1Down，2Push，3Pull</param>
        /// <param name="power">0运动，1停止</param>
        /// <returns></returns>
        public bool Move(LifterMoveType type, LiterMove power)
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

        public MethodInfo LeftUp, LeftDown;
        public MethodInfo RightUp, RightDown;
        public MethodInfo LeftPush, LeftPull;
        public MethodInfo RightPush, RightPull;
        public SendMessage Message;
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
}
