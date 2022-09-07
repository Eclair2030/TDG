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

    public struct WY_hDevice { IntPtr hDevice; UInt32 Ar; };
    
    internal class DLL
    {
        public DLL()
        { }

        [DllImport("AgvDLL.dll", EntryPoint = "FindStaff", CallingConvention = CallingConvention.StdCall)]
        public static extern int FindStaff(ShowMessage sm, out int x, out int y, out int r, int w, int h, IntPtr data);


        public static string Test(int a)
        {
            return (a+a).ToString();
        }
        /****************************************************************************
                函数名称: WY_Open
                功能描述: 打开当前板卡，获取当前板卡句柄等DeviceID参数。打开获取DeviceID
                          参数值后，才能对板卡相关操作。在关闭系统前，必须用WY_Close函数关闭。
                          打开成功，所有输出端口16路为关闭状态，5路计数器清零0。
                参数列表:
                  CardNo：板卡编号，对应PCI槽0,1,2,3....
                DeviceID: 反回当前板卡DeviceID参数值;
                  返回值: 表示函数返状态  0:正确    1:板卡打开操作失败
        技术支持联系电话：13510401592
        *****************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_Open", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_Open(int CardNo, ref WY_hDevice DeviceID);

        /****************************************************************************
                函数名称: WY_Close
                功能描述: 关闭注销当前板卡。关闭后，不能对板卡相关操作。它与WY_Open函数对应。
                参数列表:
                DeviceID: 关闭注销当前板卡ID;
                  返回值: 表示函数返状态  0:正确    1:板卡关闭操作失败  
        技术支持联系电话：13510401592
        *****************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_Close", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_Close(WY_hDevice DeviceID);

        /****************************************************************************
                函数名称: WY_GetCardVersion
                功能描述: 获取本开关量控制卡版本号
                参数列表:
	            DeviceID: 操作当前板卡ID;
               VerNumber: 返回版本号，30代表版本3.0;
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败   2:本函数与板卡不相符
        技术支持联系电话：13510401592
        *****************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_GetCardVersion", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_GetCardVersion(WY_hDevice DeviceID, ref long VerNumber);

        /*****************************************************************************
                函数名称: WY_GetLowInPutData
                功能描述: 获取开关量控制卡输入端口低8位数据
                参数列表:
	            DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                 LowData: 输入端口低8位数据，对应关系如下：
                                LowData数据: bit7,bit6,bit5,bit4,bit3,bit2,bit1,bit0
                               对应输入端口: Input7,Input6,Input5,Input4,Input3,Input2,Input1,Input0
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ******************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_GetLowInPutData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_GetLowInPutData(WY_hDevice DeviceID, ref long LowData);

        /******************************************************************************
                函数名称: WY_GetHighInPutData
                功能描述: 获取开关量控制卡输入端口高8位数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                HighData: 输入端口高8位数据，对应关系如下：
                               HighData数据:bit7,bit6,bit5,bit4,bit3,bit2,bit1,bit0
                               对应输入端口:Input15,Input14,Input13,Input12,Input11,Input10,Input9,Input8
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        *******************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_GetHighInPutData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_GetHighInPutData(WY_hDevice DeviceID, ref long HighData);

        /*****************************************************************************
                函数名称: WY_GetLowOutPutData
                功能描述: 获取开关量控制卡输出端口低8位输出数据
                参数列表:
		        DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                 LowData: 输出端口低8位数据，对应关系如下：
                                LowData数据:bit7,bit6,bit5,bit4,bit3,bit2,bit1,bit0
                               对应输入端口:Output7,Onput6,Onput5,Onput4,Onput3,Onput2,Onput1,Onput0
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        *****************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_GetLowOutPutData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_GetLowOutPutData(WY_hDevice DeviceID, ref long LowData);

        /*******************************************************************************
                函数名称: WY_GetHighOutPutData
                功能描述: 获取开关量控制卡输出端口高8位输出数据
                参数列表:
		        DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                HighData: 输出端口高8位数据，对应关系如下：
                               HighData数据:bit7,bit6,bit5,bit4,bit3,bit2,bit1,bit0
                               对应输入端口:Output15,Onput14,Onput13,Onput12,Onput11,Onput10,Onput9,Onput8
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ******************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_GetHighOutPutData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_GetHighOutPutData(WY_hDevice DeviceID, ref long HighData);

        /****************************************************************************
                函数名称: WY_GetOutPutData
                功能描述: 获取开关量控制卡输出端口输出数据
                参数列表:
		        DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
              OutPutData: 输出端口数据
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ****************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_GetOutPutData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_GetOutPutData(WY_hDevice DeviceID, ref long OutPutData);

        /***************************************************************************
                函数名称: WY_GetInPutData
                功能描述: 获取开关量控制卡输入端数据
                参数列表:
		        DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
               InPutData: 输入端口数据，低位在前，高位在后。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ****************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_GetInPutData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_GetInPutData(WY_hDevice DeviceID, ref long InPutData);

        /***************************************************************************
                函数名称: WY_ReadInPutbit0
                功能描述: 获取开关量控制卡输入端口0状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit0", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit0(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit1
                功能描述: 获取开关量控制卡输入端口1状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败 
        技术支持联系电话：13510401592
        *************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit1", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit1(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit2
                功能描述: 获取开关量控制卡输入端口2状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        *************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit2", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit2(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit3
                功能描述: 获取开关量控制卡输入端口3状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        *************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit3", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit3(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit4
                功能描述: 获取开关量控制卡输入端口4状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        *************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit4", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit4(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit5
                功能描述: 获取开关量控制卡输入端口5状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        *************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit5", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit5(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit6
                功能描述: 获取开关量控制卡输入端口6状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败 
        技术支持联系电话：13510401592
        *************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit6", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit6(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit7
                功能描述: 获取开关量控制卡输入端口7状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit7", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit7(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit8
                功能描述: 获取开关量控制卡输入端口8状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit8", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit8(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit9
                功能描述: 获取开关量控制卡输入端口9状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit9", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit9(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit10
                功能描述: 获取开关量控制卡输入端口10状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit10", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit10(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit11
                功能描述: 获取开关量控制卡输入端口11状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit11", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit11(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit12
                功能描述: 获取开关量控制卡输入端口12状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit12", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit12(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit13
                功能描述: 获取开关量控制卡输入端口13状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit13", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit13(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadInPutbit14
                功能描述: 获取开关量控制卡输入端口14状态
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 输入端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败 
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit14", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit14(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
            函数名称: WY_ReadInPutbit15
            功能描述: 获取开关量控制卡输入端口15状态
            参数列表:
            DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                 bit: 输入端口电平值（0：表示低电平，1：表示高电平）
              返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadInPutbit15", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadInPutbit15(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_SetLowOutPutData
                功能描述: 设置开关量控制卡输出端口低8位数据
                参数列表:
		        DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                 LowData: 输出端口低8位数据，对应关系如下：
                                 LowData数据:bit7,bit6,bit5,bit4,bit3,bit2,bit1,bit0
                                对应输入端口:Output7,Onput6,Onput5,Onput4,Onput3,Onput2,Onput1,Onput0
                  返回值: 表示函数返状态   0:正确    1:板卡连接失败  3：输入参数错误,输入数值超出范围0x00~0xff
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_SetLowOutPutData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_SetLowOutPutData(WY_hDevice DeviceID, long LowData);

        /************************************************************************
                函数名称: WY_SetHighOutPutData
                功能描述: 设置开关量控制卡输出端口高8位数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                HighData: 输出端口高8位数据，对应关系如下：
                                HighData数据:bit 15,bit14,bit13,bit12,bit11,bit10,bit9,bit8,
                                对应输入端口:Output7,Onput6,Onput5,Onput4,Onput3,Onput2,Onput1,Onput0
                  返回值: 表示函数返状态   0:正确    1:板卡连接失败  
                                           3：输入参数错误,输入数值超出范围0x00~0xff
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_SetHighOutPutData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_SetHighOutPutData(WY_hDevice DeviceID, long HighData);

        /************************************************************************
                函数名称: WY_SetOutPutData
                功能描述: 设置开关量控制卡输出端口16位数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
              OutPutData: 输出端口数据。
                  返回值: 表示函数返状态   0:正确    1:板卡连接失败   
                                           3：输入参数错误,OutPutData输入数值超出范围0x0000~0xffff
        技术支持联系电话：13510401592
        **************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_SetOutPutData", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_SetOutPutData(WY_hDevice DeviceID, long OutPutData);

        /************************************************************************
                函数名称: WY_WriteOutPutBit0
                功能描述: 向开关量控制卡输出端口0写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit0", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit0(WY_hDevice DeviceID, int bit);

        /***********************************************************************
                函数名称: WY_WriteOutPutBit1
                功能描述: 向开关量控制卡输出端口1写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ***********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit1", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit1(WY_hDevice DeviceID, int bit);

        /**********************************************************************
             '* 函数名称: WY_WriteOutPutBit2
             '* 功能描述: 向开关量控制卡输出端口2写入数据
             '* 参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
               '* 返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        **********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit2", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit2(WY_hDevice DeviceID, int bit);

        /*********************************************************************
                函数名称: WY_WriteOutPutBit3
                功能描述: 向开关量控制卡输出端口3写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ***********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit3", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit3(WY_hDevice DeviceID, int bit);

        /*********************************************************************
                函数名称: WY_WriteOutPutBit4
                功能描述: 向开关量控制卡输出端口4写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        **********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit4", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit4(WY_hDevice DeviceID, int bit);

        /*********************************************************************
                函数名称: WY_WriteOutPutBit5
                功能描述: 向开关量控制卡输出端口5写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        *********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit5", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit5(WY_hDevice DeviceID, int bit);

        /********************************************************************
                函数名称: WY_WriteOutPutBit6
                功能描述: 向开关量控制卡输出端口6写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        *********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit6", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit6(WY_hDevice DeviceID, int bit);

        /************************************************************************
                函数名称: WY_WriteOutPutBit7
                功能描述: 向开关量控制卡输出端口7写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit7", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit7(WY_hDevice DeviceID, int bit);

        /************************************************************************
                函数名称: WY_WriteOutPutBit8
                功能描述: 向开关量控制卡输出端口8写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit8", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit8(WY_hDevice DeviceID, int bit);

        /************************************************************************
                函数名称: WY_WriteOutPutBit9
                功能描述: 向开关量控制卡输出端口9写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit9", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit9(WY_hDevice DeviceID, int bit);

        /************************************************************************
                函数名称: WY_WriteOutPutBit10
                功能描述: 向开关量控制卡输出端口10写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit10", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit10(WY_hDevice DeviceID, int bit);

        /************************************************************************
                函数名称: WY_WriteOutPutBit11
                功能描述: 向开关量控制卡输出端口11写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit11", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit11(WY_hDevice DeviceID, int bit);

        /************************************************************************
                函数名称: WY_WriteOutPutBit12
                功能描述: 向开关量控制卡输出端口12写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit12", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit12(WY_hDevice DeviceID, int bit);

        /************************************************************************
                函数名称: WY_WriteOutPutBit13
                功能描述: 向开关量控制卡输出端口13写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        *************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit13", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit13(WY_hDevice DeviceID, int bit);

        /************************************************************************
                函数名称: WY_WriteOutPutBit14
                功能描述: 向开关量控制卡输出端口14写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit14", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit14(WY_hDevice DeviceID, int bit);

        /*************************************************************************
                函数名称: WY_WriteOutPutBit15
                功能描述: 向开关量控制卡输出端口15写入数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                   value: 表示端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败  3:输入参数错误
        技术支持联系电话：13510401592
        ***************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_WriteOutPutBit15", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_WriteOutPutBit15(WY_hDevice DeviceID, int value);

        /************************************************************************
                函数名称: WY_ReadOutPutBit0
                功能描述: 向开关量控制卡输出端口0回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败 
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit0", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit0(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit1
                功能描述: 向开关量控制卡输出端口1回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit1", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit1(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit2
                功能描述: 向开关量控制卡输出端口2回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败 
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit2", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit2(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit3
                功能描述: 向开关量控制卡输出端口3回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit3", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit3(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit4
                功能描述: 向开关量控制卡输出端口4回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit4", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit4(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit5
                功能描述: 向开关量控制卡输出端口5回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败 
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit5", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit5(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit6
                功能描述: 向开关量控制卡输出端口6回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit6", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit6(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit7
                功能描述: 向开关量控制卡输出端口7回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit7", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit7(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit8
                功能描述: 向开关量控制卡输出端口8回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败 
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit8", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit8(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit9
                功能描述: 向开关量控制卡输出端口9回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit9", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit9(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit10
                功能描述: 向开关量控制卡输出端口10回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit10", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit10(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit11
                功能描述: 向开关量控制卡输出端口11回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit11", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit11(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit12
                功能描述: 向开关量控制卡输出端口12回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit12", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit12(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit13
                功能描述: 向开关量控制卡输出端口13回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit13", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit13(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit14
                功能描述: 向开关量控制卡输出端口14回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit14", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit14(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ReadOutPutBit15
                功能描述: 向开关量控制卡输出端口15回读数据
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                     bit: 回读输出端口电平值（0：表示低电平，1：表示高电平）
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadOutPutBit15", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadOutPutBit15(WY_hDevice DeviceID, ref int bit);

        /************************************************************************
                函数名称: WY_ResetCounter0
                功能描述: 复位计数器0。复位后，计数器0中的计数寄存器清除复位为0。
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        *************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ResetCounter0", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ResetCounter0(WY_hDevice DeviceID);

        /************************************************************************
                函数名称: WY_ResetCounter1
                功能描述: 复位计数器1。复位后，计数器1中的计数寄存器清除复位为0。
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败 
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ResetCounter1", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ResetCounter1(WY_hDevice DeviceID);

        /************************************************************************
                函数名称: WY_ResetCounter2
                功能描述: 复位计数器2。复位后，计数器2中的计数寄存器清除复位为0。
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ResetCounter2", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ResetCounter2(WY_hDevice DeviceID);

        /************************************************************************
                函数名称: WY_ResetCounter3
                功能描述: 复位计数器3。复位后，计数器3中的计数寄存器清除复位为0。
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ResetCounter3", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ResetCounter3(WY_hDevice DeviceID);

        /**********************************************************************
                函数名称: WY_ResetCounter4
                功能描述: 复位计数器4。复位后，计数器4中的计数寄存器清除复位为0。
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ************************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ResetCounter4", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ResetCounter4(WY_hDevice DeviceID);

        /***********************************************************************
                函数名称: WY_ReadCounter0
                功能描述: 读取计数器0中的计数值。
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                   value: 计数器0中的计数值。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ***********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadCounter0", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadCounter0(WY_hDevice DeviceID, ref int value);

        /***********************************************************************
                函数名称: WY_ReadCounter1
                功能描述: 读取计数器1中的计数值。
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                   value: 计数器1中的计数值。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ***********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadCounter1", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadCounter1(WY_hDevice DeviceID, ref int value);

        /**********************************************************************
                函数名称: WY_ReadCounter2
                功能描述: 读取计数器2中的计数值。
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                   value: 计数器2中的计数值。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ***********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadCounter2", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadCounter2(WY_hDevice DeviceID, ref int value);

        /**********************************************************************
                函数名称: WY_ReadCounter3
                功能描述: 读取计数器3中的计数值。
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                   value: 计数器3中的计数值。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ***********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadCounter3", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadCounter3(WY_hDevice DeviceID, ref int value);

        /**********************************************************************
                函数名称: WY_ReadCounter4
                功能描述: 读取计数器4中的计数值。
                参数列表:
                DeviceID: 当前板卡DeviceID参数值。（从WY_Open函数获取）。
                   value: 计数器4中的计数值。
                  返回值: 表示函数返状态  0:正确    1:板卡连接失败
        技术支持联系电话：13510401592
        ***********************************************************************/
        [DllImport("WENYU_PIO32P.dll", EntryPoint = "WY_ReadCounter4", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int WY_ReadCounter4(WY_hDevice DeviceID, ref int value);

        public static WY_hDevice Device_IOCard;
    }
}
