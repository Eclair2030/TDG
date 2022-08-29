// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"

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

extern "C" _declspec(dllexport) int _stdcall FindStaff(MESSAGE_SHOW Message, int* coordX, int* coordY, int srcWidth, int srcHeight, void* srcData)
{
    int result = Unknow;
    try
    {
        Mat src(srcHeight, srcWidth, CV_8UC1, srcData);
        imwrite("D:\\AGV1.bmp", src);
        //result = PRE_BOND_STAGE.CAM_LEFT.Snap(&src, stepCode, markWidth, markHeight, centerX, centerY, Message);
        result = Success;
    }
    catch (const std::exception& ex)
    {
        result = Fail;
        Message(ex.what());
    }
    return result;
}
