// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "UpSystem.h"
#include "PBD_Tool.h"
#include "PBD_Stage.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

Mat img;

/// <summary>
/// 上位机连接对象
/// </summary>
UpSystem UP_SYS;

/// <summary>
/// 预压压头工位对象
/// </summary>
PBD_Tool PRE_BOND_TOOL;

/// <summary>
/// 预压平台工位对象
/// </summary>
PBD_Stage PRE_BOND_STAGE;


extern "C" _declspec(dllexport) uchar* _stdcall ImgTransport(int* rows, int* cols)
{
    img = imread("D:\\cjh.jpg");
    *rows = img.rows;
    *cols = img.cols;
    return img.data;
}

/// <summary>
/// 初始化Recipe信息
/// </summary>
/// <param name="Message"></param>
/// <returns></returns>
extern "C" _declspec(dllexport) int _stdcall InitRecipe(MESSAGE_SHOW Message)
{
    return COMPLETE;
}

/// <summary>
/// 连接上位机
/// </summary>
/// <param name="usType"></param>
/// <returns></returns>
extern "C" _declspec(dllexport) int _stdcall ConnectToUpSystem(int usType, char* serverIpAddress, int serverPort, MESSAGE_SHOW Message)
{
    int result = UNKNOW;
    try
    {
        UP_SYS = UpSystem(usType, serverIpAddress, serverPort);
        result = UP_SYS.Connect(Message);
        if (result == COMPLETE)
        {
            Message("Connect up system success.");
        }
        else
        {
            Message("Connect up system fail.");
        }
    }
    catch (const std::exception& ex)
    {
        result = OTHEREXCEPTION;
        Message(ex.what());
    }
    
    return result;
}

/// <summary>
/// 切断上位机连接
/// </summary>
/// <returns></returns>
extern "C" _declspec(dllexport) int _stdcall DisconnectFromUpSystem(MESSAGE_SHOW Message)
{
    int result = UNKNOW;
    UP_SYS.Close(Message);
    result = COMPLETE;
    return result;
}

/// <summary>
/// 接收和发送单个心跳脉搏
/// </summary>
/// <param name="Message"></param>
/// <returns></returns>
extern "C" _declspec(dllexport) int _stdcall HeartBeatOnce(MESSAGE_SHOW Message)
{
    int result = UP_SYS.HeartBeat(Message);
    return result;
}

/// <summary>
/// 从界面层获取Recipe，设置到底层视觉算法中
/// </summary>
/// <param name="Message"></param>
/// <returns></returns>
extern "C" _declspec(dllexport) int _stdcall SetToolParameter(int MODEL_RECIPE_CODE, 
                                                                                                    double DELAY, 
                                                                                                    double OFFSET_X,  double OFFSET_Y, double OFFSET_T,
                                                                                                    double SPEC_X,  double SPEC_Y, double SPEC_T,
                                                                                                    double Y_MAP,  bool USE_Y_MAP, 
                                                                                                    int* ENABLE1, MarkCenter* CENTER1,
                                                                                                    const char* Path1_1, const char* Path1_2, const char* Path1_3,
                                                                                                    double RotateCenter_X1, double RotateCenter_Y1, 
                                                                                                    double Resolution_X1, double Resolution_Y1, 
                                                                                                    int* ENABLE2, MarkCenter* CENTER2,
                                                                                                    const char* Path2_1, const char* Path2_2, const char* Path2_3,
                                                                                                    double RotateCenter_X2, double RotateCenter_Y2,
                                                                                                    double Resolution_X2, double Resolution_Y2,
                                                                                                    MESSAGE_SHOW Message)
{
    Recipe_Model rm;
    rm.MODEL_RECIPE_CODE = MODEL_RECIPE_CODE;
    rm.DELAY = DELAY;
    rm.OFFSET_X = OFFSET_X;
    rm.OFFSET_Y = OFFSET_Y;
    rm.OFFSET_T = OFFSET_T;
    rm.SPEC_X = SPEC_X;
    rm.SPEC_Y = SPEC_Y;
    rm.SPEC_T = SPEC_T;
    rm.Y_MAP = Y_MAP;
    rm.USE_Y_MAP = USE_Y_MAP;
    Recipe_Camera rc1, rc2;
    for (size_t i = 0; i < 3; i++, ENABLE1++, CENTER1++)
    {
        rc1.ENABLE[i] = *ENABLE1;
        rc1.MarkPoint[i] = *CENTER1;
    }
    rc1.RotateCenter_X = RotateCenter_X1;
    rc1.RotateCenter_Y = RotateCenter_Y1;
    rc1.Resolution_X = Resolution_X1;
    rc1.Resolution_Y = Resolution_Y1;
    for (size_t i = 0; i < 3; i++, ENABLE2++, CENTER2++)
    {
        rc2.ENABLE[i] = *ENABLE2;
        rc2.MarkPoint[i] = *CENTER2;
    }
    rc2.RotateCenter_X = RotateCenter_X2;
    rc2.RotateCenter_Y = RotateCenter_Y2;
    rc2.Resolution_X = Resolution_X2;
    rc2.Resolution_Y = Resolution_Y2;
    int result = PRE_BOND_TOOL.SetRecipe(rm, rc1, rc2, Path1_1, Path1_2, Path1_3, Path2_1, Path2_2, Path2_3, Message);
    if (result == COMPLETE)
    {
        Message("Vision system PBD - Tool recipe init complete.");
    }

    return result;
}

/// <summary>
/// 从界面层获取Recipe，设置到底层视觉算法中
/// </summary>
/// <param name="Message"></param>
/// <returns></returns>
extern "C" _declspec(dllexport) int _stdcall SetStageParameter(int MODEL_RECIPE_CODE,
                                                                                                                    double DELAY,
                                                                                                                    double OFFSET_X, double OFFSET_Y, double OFFSET_T,
                                                                                                                    double SPEC_X, double SPEC_Y, double SPEC_T,
                                                                                                                    double Y_MAP, bool USE_Y_MAP,
                                                                                                                    int* ENABLE1, MarkCenter* CENTER1,
                                                                                                                    const char* Path1_1, const char* Path1_2, const char* Path1_3,
                                                                                                                    double RotateCenter_X1, double RotateCenter_Y1,
                                                                                                                    double Resolution_X1, double Resolution_Y1,
                                                                                                                    int* ENABLE2, MarkCenter* CENTER2,
                                                                                                                    const char* Path2_1, const char* Path2_2, const char* Path2_3,
                                                                                                                    double RotateCenter_X2, double RotateCenter_Y2,
                                                                                                                    double Resolution_X2, double Resolution_Y2,
                                                                                                                    MESSAGE_SHOW Message)
{
    Recipe_Model rm;
    rm.MODEL_RECIPE_CODE = MODEL_RECIPE_CODE;
    rm.DELAY = DELAY;
    rm.OFFSET_X = OFFSET_X;
    rm.OFFSET_Y = OFFSET_Y;
    rm.OFFSET_T = OFFSET_T;
    rm.SPEC_X = SPEC_X;
    rm.SPEC_Y = SPEC_Y;
    rm.SPEC_T = SPEC_T;
    rm.Y_MAP = Y_MAP;
    rm.USE_Y_MAP = USE_Y_MAP;
    Recipe_Camera rc1, rc2;
    for (size_t i = 0; i < 3; i++, ENABLE1++, CENTER1++)
    {
        rc1.ENABLE[i] = *ENABLE1;
        rc1.MarkPoint[i] = *CENTER1;
    }
    rc1.RotateCenter_X = RotateCenter_X1;
    rc1.RotateCenter_Y = RotateCenter_Y1;
    rc1.Resolution_X = Resolution_X1;
    rc1.Resolution_Y = Resolution_Y1;
    for (size_t i = 0; i < 3; i++, ENABLE2++, CENTER2++)
    {
        rc2.ENABLE[i] = *ENABLE2;
        rc2.MarkPoint[i] = *CENTER2;
    }
    rc2.RotateCenter_X = RotateCenter_X2;
    rc2.RotateCenter_Y = RotateCenter_Y2;
    rc2.Resolution_X = Resolution_X2;
    rc2.Resolution_Y = Resolution_Y2;
    int result = PRE_BOND_STAGE.SetRecipe(rm, rc1, rc2, Path1_1, Path1_2, Path1_3, Path2_1, Path2_2, Path2_3, Message);
    if (result == COMPLETE)
    {
        Message("Vision system PBD - Stage recipe init complete.");
    }

    return result;
}

/// <summary>
/// 获取上位机信号
/// </summary>
/// <param name="Message"></param>
/// <returns></returns>
extern "C" _declspec(dllexport) int _stdcall GetSignalFromUpSystem(MESSAGE_SHOW Message, int* Command, int* SubCommand, int* Code)
{
    int result = UP_SYS.GetSignal(Message, Command, SubCommand, Code);
    return result;
}

extern "C" _declspec(dllexport) int _stdcall SequenceShot(int stepCode, MESSAGE_SHOW Message, int* markWidth, int* markHeight, 
                                                                                        int* centerX, int* centerY, int srcWidth, int srcHeight, void* srcData)
{
    int result = UNKNOW;
    try
    {
        Mat src(srcHeight, srcWidth, CV_8UC1, srcData);
        switch (stepCode)
        {
        case 1:		//Tool step 1
        case 2:		//Tool step 2
            result = PRE_BOND_TOOL.SeqShot(stepCode, &src, markWidth, markHeight, centerX, centerY, Message);
            break;
        case 3:		//Stage step 1
        case 4:		//Stage step 2
            PRE_BOND_STAGE.SeqShot(stepCode, &src, markWidth, markHeight, centerX, centerY, Message);
            result = COMPLETE;
            break;
        case 5:		//Tool both
        case 6:		//Stage both
        default:
            result = FAIL;
            break;
        }
        //result = PRE_BOND_STAGE.CAM_LEFT.Snap(&src, stepCode, markWidth, markHeight, centerX, centerY, Message);
    }
    catch (const std::exception& ex)
    {
        result = OTHEREXCEPTION;
        Message(ex.what());
    }
    return result;
}

extern "C" _declspec(dllexport) int _stdcall TestPtr(MESSAGE_SHOW Message, int* ptr)
{
    *ptr = 489;
    char chx[16];
    _itoa_s(*ptr, chx, 10);
    Message(chx);
    return 0;
}