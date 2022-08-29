using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AgvDispatchor
{
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void ShowMessage(IntPtr p, MessageType t);
    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public delegate void BufferTask(int iStart);

    internal class DLL
    {
        public DLL()
        { }

        [DllImport("AgvDLL.dll", EntryPoint = "FindStaff", CallingConvention = CallingConvention.StdCall)]
        public static extern int FindStaff(ShowMessage sm, out int x, out int y, int w, int h, IntPtr data);
    }
}
