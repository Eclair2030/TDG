using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class Mary
    {
        public Mary()
        {
        }

        [DllImport("Mdfunc32.dll", EntryPoint = "mdopen", CallingConvention = CallingConvention.StdCall)]
        extern static short mdopen(short chan, short para2, out int ipoint);

        [DllImport("Mdfunc32.dll", EntryPoint = "mdclose", CallingConvention = CallingConvention.StdCall)]
        extern static short mdclose(int ipath);

        [DllImport("Mdfunc32.dll", EntryPoint = "mdsendex", CallingConvention = CallingConvention.StdCall)]
        extern static int mdSendEx(int path, int netNumber, int stationNumber, int deveiceType, int deveiceNumber, out int size, out short data);

        [DllImport("Mdfunc32.dll", EntryPoint = "mdreceiveex", CallingConvention = CallingConvention.StdCall)]
        extern static int mdReceiveEx(int path, int netNumber, int stationNumber, int deveiceType, int deveiceNumber, out int size, out short data);

        public static short MdOpen()
        {
            return mdopen(_channelNumber, _mode, out _path);
        }

        public static short MdClose()
        {
            return mdclose(_path);
        }

        public static int MdSendEx(int deveiceType, int deveiceNumber, out int writeSize, out short data)
        {
            return mdSendEx(_path, _plcNetWorkNumber, _plcStationNumber, deveiceType, deveiceNumber, out writeSize, out data);
        }

        public static int MdReceiveEx(int deveiceType, int deveiceNumber, out int readSize, out short data)
        {
            return mdReceiveEx(_path, _plcNetWorkNumber, _plcStationNumber, deveiceType, deveiceNumber, out readSize, out data);
        }

        public static int BaseAddress(MotorNumber motorNum)
        {
            return (int)motorNum * 100 + 60;
        }

        public static int ModelAddress(MotorNumber motorNum)
        {
            return (int)motorNum * 100 + 30000;
        }

        public static int UserAddress(MotorNumber motorNum)
        {
            return ((int)motorNum - 1) * 40 + 80000;
        }

        public static int SpeedAddress(MotorNumber motorNum)
        {
            return (int)motorNum * 100 + 20;
        }

        static short _channelNumber = 154;
        static short _mode = -1;
        static int _path = 0;
        static int _plcNetWorkNumber = 0x01, _plcStationNumber = 0x01;

        public static int _plcTriggerDeviceNumber = 584594;
        public static int _plcTriggerTimerPeriod = 2000;        //2S scan period
        public static int _plcUnitReadLength = 4;
        public static int _timesFromShortToInt = 2;
        public static float _dataShow = 10000f;
        public static float _speedDataShow = 100f;
        public static int _maxUshortAdd1 = ushort.MaxValue + 1;
        public static DateTime _date;
        public static string _filePath = "D:\\Position\\";
        public static string _fileExtendName = ".csv";
        public static string _csvFileSplit = ",";
        public static string _tableHead = " ,Base,ModelOffset,UserOffset,Speed,AlignSpeed" + Environment.NewLine;
        public static string _posChangeTableHead = "Time,Process,Position,Category,Before,After" + Environment.NewLine;
        public static string _plasmaStage_X_FileName = "PlasmaStage_X";
        public static string _plasmaStage_Y_FileName = "PlasmaStage_Y";
        public static string _plasmaStage_T1_FileName = "PlasmaStage_T1";
        public static string _plasmaStage_T2_FileName = "PlasmaStage_T2";
        public static string _plasmaStage_Ym_FileName = "PlasmaStage_Ym";
        public static string _plasmaHandler_Default_FileName = "PlasmaHandler_Default";
        public static string _acfStage1_X_FileName = "AcfStage1_X";
        public static string _acfStage1_Y_FileName = "AcfStage1_Y";
        public static string _acfStage1_T_FileName = "AcfStage1_T";
        public static string _acfStage2_X_FileName = "AcfStage2_X";
        public static string _acfStage2_Y_FileName = "AcfStage2_Y";
        public static string _acfStage2_T_FileName = "AcfStage2_T";
        public static string _acfTool1_Z_FileName = "AcfTool1_Z";
        public static string _acfTool2_Z_FileName = "AcfTool2_Z";
        public static string _acfReel1_Turn_FileName = "AcfReel1_Turn";
        public static string _acfReel2_Turn_FileName = "AcfReel2_Turn";
        public static string _acfFeed1_Turn_FileName = "AcfFeed1_Turn";
        public static string _acfFeed2_Turn_FileName = "AcfFeed2_Turn";
        public static string _acfSeparator1_Default_FileName = "AcfSeparator1_Default";
        public static string _acfSeparator2_Default_FileName = "AcfSeparator2_Default";
        public static string _acfHandler_Default_FileName = "AcfHandler_Default";
        public static string _prebondStage_X_FileName = "PreBondStage_X";
        public static string _prebondStage_Y_FileName = "PreBondStage_Y";
        public static string _prebondStage_T1_FileName = "PreBondStage_T1";
        public static string _prebondStage_T2_FileName = "PreBondStage_T2";
        public static string _prebondTool1_X_FileName = "PreBondTool1_X";
        public static string _prebondTool1_Z_FileName = "PreBondTool1_Z";
        public static string _prebondTool1_T_FileName = "PreBondTool1_T";
        public static string _prebondTool2_X_FileName = "PreBondTool2_X";
        public static string _prebondTool2_Z_FileName = "PreBondTool2_Z";
        public static string _prebondTool2_T_FileName = "PreBondTool2_T";
        public static string _prebondCarrier1_X_FileName = "PreBondCarrier1_X";
        public static string _prebondCarrier1_Y_FileName = "PreBondCarrier1_Y";
        public static string _prebondCarrier1_Z_FileName = "PreBondCarrier1_Z";
        public static string _prebondCarrier1_T1_FileName = "PreBondCarrier1_T1";
        public static string _prebondCarrier1_T2_FileName = "PreBondCarrier1_T2";
        public static string _prebondCarrier2_X_FileName = "PreBondCarrier2_X";
        public static string _prebondCarrier2_Y_FileName = "PreBondCarrier2_Y";
        public static string _prebondCarrier2_Z_FileName = "PreBondCarrier2_Z";
        public static string _prebondCarrier2_T_FileName = "PreBondCarrier2_T";
        public static string _prebondCamera_X_FileName = "PreBondCamera_X";
        public static string _prebondCamera_Y_FileName = "PreBondCamera_Y";
        public static string _prebondHandler_Default_FileName = "PreBondHandler_Default";
        public static string _prebondUSC_Default_FileName = "PreBondUSC_Default";
        public static string _prebondShuttle1_X_FileName = "PreBondShuttle1_X";
        public static string _prebondShuttle1_Z_FileName = "PreBondShuttle1_Z";
        public static string _prebondShuttle2_X_FileName = "PreBondShuttle2_X";
        public static string _prebondShuttle2_Z_FileName = "PreBondShuttle2_Z";
        public static string _prebondLoadUnload1_Default_FileName = "PreBondLoadUnload1_Default";
        public static string _prebondLoadUnload2_Default_FileName = "PreBondLoadUnload2_Default";
        public static string _prebondIcCamera_Y_FileName = "PreBondIcCamera_Y";
        public static string _prebondIcCamera_T_FileName = "PreBondIcCamera_T";
        public static string _prebondIcBuffer_Default_FileName = "PreBondIcBuffer_Default";
        public static string _mainbondStage1_Y_FileName = "MainBondStage1_Y";
        public static string _mainbondStage2_Y_FileName = "MainBondStage2_Y";
        public static string _mainbondStage3_Y_FileName = "MainBondStage3_Y";
        public static string _mainbondStage4_Y_FileName = "MainBondStage4_Y";
        public static string _mainbondStage1_T_FileName = "MainBondStage1_T";
        public static string _mainbondStage2_T_FileName = "MainBondStage2_T";
        public static string _mainbondStage3_T_FileName = "MainBondStage3_T";
        public static string _mainbondStage4_T_FileName = "MainBondStage4_T";
        public static string _mainbondTool1_Z_FileName = "MainBondTool1_Z";
        public static string _mainbondTool2_Z_FileName = "MainBondTool2_Z";
        public static string _mainbondTool3_Z_FileName = "MainBondTool3_Z";
        public static string _mainbondTool4_Z_FileName = "MainBondTool4_Z";
        public static string _mainbondCamera_X_FileName = "MainBondCamera_X";
        public static string _mainbondHandler_Default_FileName = "MainBondHandler_Default";
        public static string _mainbondSheet1_Turn_FileName = "MainBondSheet1_Turn";
        public static string _mainbondSheet2_Turn_FileName = "MainBondSheet2_Turn";

    }
}
