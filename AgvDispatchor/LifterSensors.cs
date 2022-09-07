using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    public class LifterSensors
    {
        public LifterSensors()
        { }

        public bool InitSensors(string MonL, string MinP, string MonC, string Agv, string LupL, string LdownL, string RupL, string RdownL,
            string LupP, string LdownP, string RupP, string RdownP)
        {
            bool result = false;
            Type t = typeof(DLL);
            MaterialOnLifter = t.GetMethod(MonL);
            MaterialInPosition = t.GetMethod(MinP);
            MaterialOnCustomer = t.GetMethod(MonC);
            AgvInLifter = t.GetMethod(Agv);
            LeftUpLimit = t.GetMethod(LupL);
            LeftDownLimit = t.GetMethod(LdownL);
            RightUpLimit = t.GetMethod(RupL);
            RightDownLimit = t.GetMethod(RdownL);
            LeftUpPosition = t.GetMethod(LupP);
            LeftDownPosition = t.GetMethod(LdownP);
            RightUpPosition = t.GetMethod(RupP);
            RightDownPosition = t.GetMethod(RdownP);
            if (MaterialOnLifter != null && MaterialInPosition != null && MaterialOnCustomer != null && AgvInLifter != null && LeftUpLimit != null && LeftDownLimit != null && 
                RightUpLimit != null && RightDownLimit != null && LeftUpPosition != null && LeftDownPosition != null && RightUpPosition != null && RightDownPosition != null)
            {
                result = true;
            }
            return result;
        }

        public bool ReadSensors(ref long data)
        {
            bool result = false;
            if (DLL.WY_GetInPutData(DLL.Device_IOCard, ref data) == 0)
            {
                result = true;
            }
            return result;
        }

        private MethodInfo MaterialOnLifter;                     //料车是否在升降机上的Sensor，位于升降机边缘，料车一进入升降机就会感应到
        private MethodInfo MaterialInPosition;                  //料车在升降机上是否到位(来料升降机) / 回收升降机不用此Sensor
        private MethodInfo MaterialOnCustomer;             //料车是否在客户的升降机上，位于客户输送带中间下方，向上感应
        private MethodInfo AgvInLifter;                               //AGV小车是否在升降机
        private MethodInfo LeftUpLimit;                              //左侧上限位
        private MethodInfo LeftDownLimit;                        //左侧下限位
        private MethodInfo RightUpLimit;                          //右侧上限位
        private MethodInfo RightDownLimit;                     //右侧下限位
        private MethodInfo LeftUpPosition;                       //左侧上升位置(上升位置是避让位)
        private MethodInfo LeftDownPosition;                  //左侧下降位置(下降位置用于把料车放到AGV小车上，以及和客户的输送带做交互)
        private MethodInfo RightUpPosition;                    //右侧上升位置
        private MethodInfo RightDownPosition;               //右侧下降位置

        public static char SensorOn = '0';
        public static char SensorOff = '1';
        public static int MaterialHas = 0;
        public static int MaterialArrive = 1;
        public static int MaterialCustom = 2;
        public static int Agv = 3;
        public static int LupLimit = 4;
        public static int LdownLimit = 5;
        public static int RupLimit = 6;
        public static int RdownLimit = 7;
        public static int LupPos = 8;
        public static int LdownPos = 9;
        public static int RupPos = 10;
        public static int RdownPos = 11;
    }
}
