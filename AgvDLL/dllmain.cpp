// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "Staff.h"

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

extern "C" _declspec(dllexport) int _stdcall FindStaff(MESSAGE_SHOW Message, int* coordX, int* coordY, int* radius, int srcWidth, int srcHeight, void* srcData)
{
    int result = Unknow;
    try
    {
        Mat src(srcHeight, srcWidth, CV_8UC1, srcData);
        Staff stf;
        if (stf.FindEmptyStaff(&src, coordX, coordY, radius))
        {
            result = Success;
        }
    }
    catch (const std::exception& ex)
    {
        result = Fail;
    }
    return result;
}
