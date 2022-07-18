using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Involution
{
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void ShowMessage(IntPtr p, MessageType t);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void BufferTask(int iStart);

    public class HJF
    {
        public static Dictionary<MessageType, Brush> BRUSH_TYPE = new Dictionary<MessageType, Brush>()
        {
            { MessageType.Default, Brushes.Gray },
            { MessageType.CppProcess, Brushes.Blue },
            { MessageType.FunctionResult, Brushes.Chocolate },
            { MessageType.Error, Brushes.DarkRed },
        };

        public HJF()
        {
        }

        [DllImport("Hun.dll", EntryPoint = "InitAuto", CallingConvention = CallingConvention.StdCall)]
        public static extern int InitAuto(ShowMessage sm);

        [DllImport("Hun.dll", EntryPoint = "FinishAuto", CallingConvention = CallingConvention.StdCall)]
        public static extern int FinishAuto();

        [DllImport("Hun.dll", EntryPoint = "InitUi", CallingConvention = CallingConvention.StdCall)]
        public static extern int InitUi(ShowMessage sm);

        [DllImport("Hun.dll", EntryPoint = "FinishUi", CallingConvention = CallingConvention.StdCall)]
        public static extern int FinishUi();

        [DllImport("Hun.dll", EntryPoint = "Trigger", CallingConvention = CallingConvention.StdCall)]
        public static extern int Trigger(ShowMessage sm, BufferTask bt);

        [DllImport("Hun.dll", EntryPoint = "GetAllPosTable", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAllPosTable(ShowMessage sm);

        [DllImport("Hun.dll", EntryPoint = "FlushRecordBuffer", CallingConvention = CallingConvention.StdCall)]
        public static extern int FlushRecordBuffer(ShowMessage sm, BufferTask bt);


        [DllImport("Hun.dll", EntryPoint = "SetRecipe_ForTest", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetRecipe_ForTest(ShowMessage sm);

        [DllImport("Hun.dll", EntryPoint = "Trigger_ForTest", CallingConvention = CallingConvention.StdCall)]
        public static extern int Trigger_ForTest(ShowMessage sm);

        [DllImport("Hun.dll", EntryPoint = "TriggerReset_ForTest", CallingConvention = CallingConvention.StdCall)]
        public static extern int TriggerReset_ForTest(ShowMessage sm);

        [DllImport("Hun.dll", EntryPoint = "Edit_ForTest", CallingConvention = CallingConvention.StdCall)]
        public static extern int Edit_ForTest(ShowMessage sm, int value);
    }

    public enum UiSignal
    {
        None = 0,
        Button_Read = 1,
        Button_Write = 2,
        Button_Edit = 3,
        Button_PLC_Trigger_On = 4,
        Button_PLC_Trigger_Off = 5,
    }

    public enum MessageType
    {
        Default = 0,
        CppProcess = 1,
        FunctionResult = 2,
        Error = 3,
    }

    

}
