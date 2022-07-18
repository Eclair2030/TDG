using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TDG_Vision
{
    internal class VisionDll
    {
        public VisionDll() { }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void SHOWMESSAGE(IntPtr p);

        //public delegate void DrawMarkResult(Canvas can, Point img_lt, Point rect_lt, double ratio, double width, double height);

        [DllImport("VisionAlgorithm.dll", EntryPoint = "ConnectToUpSystem", CallingConvention = CallingConvention.StdCall)]
        public extern static int ConnectToUpSystem(int usType, IntPtr ipAddr, int serverPort, SHOWMESSAGE Message);

        [DllImport("VisionAlgorithm.dll", EntryPoint = "DisconnectFromUpSystem", CallingConvention = CallingConvention.StdCall)]
        public extern static int DisconnectFromUpSystem(SHOWMESSAGE Message);

        [DllImport("VisionAlgorithm.dll", EntryPoint = "HeartBeatOnce", CallingConvention = CallingConvention.StdCall)]
        public extern static int HeartBeatOnce(SHOWMESSAGE Message);

        [DllImport("VisionAlgorithm.dll", EntryPoint = "SetToolParameter", CallingConvention = CallingConvention.StdCall)]
        public extern static int SetToolParameter(int MODEL_RECIPE_CODE, double DELAY, double OFFSET_X, double OFFSET_Y, double OFFSET_T, double SPEC_X, double SPEC_Y, double SPEC_T,
            double Y_MAP, bool USE_Y_MAP, int[] ENABLE1, MarkCenter[] CENTER1, IntPtr Path1_1, IntPtr Path1_2, IntPtr Path1_3, 
            double RotateCenter_X1, double RotateCenter_Y1,
            double Resolution_X1, double Resolution_Y1, int[] ENABLE2, MarkCenter[] CENTER2, IntPtr Path2_1, IntPtr Path2_2, IntPtr Path2_3, double RotateCenter_X2, 
            double RotateCenter_Y2, double Resolution_X2, double Resolution_Y2, SHOWMESSAGE Message);

        [DllImport("VisionAlgorithm.dll", EntryPoint = "SetStageParameter", CallingConvention = CallingConvention.StdCall)]
        public extern static int SetStageParameter(int MODEL_RECIPE_CODE, double DELAY, double OFFSET_X, double OFFSET_Y, double OFFSET_T, double SPEC_X, double SPEC_Y, double SPEC_T,
            double Y_MAP, bool USE_Y_MAP, int[] ENABLE1, MarkCenter[] CENTER1, IntPtr Path1_1, IntPtr Path1_2, IntPtr Path1_3, 
            double RotateCenter_X1, double RotateCenter_Y1,
            double Resolution_X1, double Resolution_Y1, int[] ENABLE2, MarkCenter[] CENTER2, IntPtr Path2_1, IntPtr Path2_2, IntPtr Path2_3, double RotateCenter_X2,
            double RotateCenter_Y2, double Resolution_X2, double Resolution_Y2, SHOWMESSAGE Message);

        [DllImport("VisionAlgorithm.dll", EntryPoint = "GetSignalFromUpSystem", CallingConvention = CallingConvention.StdCall)]
        public extern static int GetSignalFromUpSystem(SHOWMESSAGE Message, out int Command, out int SubCommand, out int Code);

        [DllImport("VisionAlgorithm.dll", EntryPoint = "SequenceShot", CallingConvention = CallingConvention.StdCall)]
        public extern static int SequenceShot(int stepCode, SHOWMESSAGE Message, out int markWidth, out int markHeight, 
                                                                            out int centerX, out int centerY, int srcWidth, int srcHeight, IntPtr srcData);

        [DllImport("VisionAlgorithm.dll", EntryPoint = "TestPtr", CallingConvention = CallingConvention.StdCall)]
        public extern static int TestPtr(SHOWMESSAGE Message, out int ptr);
    }
}

