#include "pch.h"
#include "UpSystem.h"

UpSystem::UpSystem()
{
    IS_CONNECTED = false;
}

UpSystem::UpSystem(int usType, const char* serverIpAddress, int serverPort)
{
    US_TYPE = usType;
    IS_CONNECTED = false;
    in_addr ad;
    if (inet_pton(AF_INET, serverIpAddress, &ad) == 1)
    {
        ADDR.sin_family = AF_INET;
        ADDR.sin_addr = ad;
        ADDR.sin_port = htons(serverPort);
    }
    else
    {//fail
    }
}

UpSystem::~UpSystem()
{
}

int UpSystem::Connect(MESSAGE_SHOW Message)
{
    int result = UNKNOW;
    WORD r = MAKEWORD(2, 2);
    WSADATA wsa;
    int err = WSAStartup(r, &wsa);
    if (err == 0)
    {
        Message("socket init success.");
    }
    else
    {
        result = FAIL;
        Message("socket init fail.");
        WSACleanup();
        return result;
    }    

    UPSYS_CONNECTION = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    int timeout = 2000; //设定超时时间2秒
    int ret = setsockopt(UPSYS_CONNECTION, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(timeout));
    if (connect(UPSYS_CONNECTION, (sockaddr*)&ADDR, sizeof(sockaddr)) == SOCKET_ERROR)
    {
        result = FAIL;
        WSACleanup();
        return result;
    }
    else
    {
        IS_CONNECTED = true;
    }
	return COMPLETE;
}

int UpSystem::Close(MESSAGE_SHOW Message)
{
    if (IS_CONNECTED)
    {
        closesocket(UPSYS_CONNECTION);
        WSACleanup();
    }
	return COMPLETE;
}

int UpSystem::HeartBeat(MESSAGE_SHOW Message)
{
    int send = HEARTBEAT_LOCAL;
    char sendData[2];
    sendData[0] = send & 0x0F;
    sendData[1] = (send >> 8) & 0x0F;
    int result = _WriteToUpSystem(UNIT_D, 101, 1, sendData, Message);

    char recvData[2];
    memset(recvData, 0, sizeof(recvData));
    result = _ReadFromUpSystem(UNIT_D, 101, 1, recvData, Message);
    if (result == COMPLETE)
    {
        int data = recvData[0] | (recvData[1] << 8);
        if (data == HEARTBEAT_UPSYS)
        {
            result = _WriteToUpSystem(UNIT_D, 101, 1, sendData, Message);
            if (result == COMPLETE)
            {
                //Message("Heart beat signal check ok.");
            }
            else if (result == DISCONNECT)
            {
                Message("disconnect with up system at write response.");
            }
            else
            {
                Message("write unknow error.");
            }
        }
        else
        {
            result = TIMEOUT;
            Message("up system heart beat data error.");
        }
    }
    else if (result == DISCONNECT)
    {
        Message("disconnect with up system at read response.");
    }
    else
    {
        Message("read unknow error.");
    }
    return result;
}

int UpSystem::GetSignal(MESSAGE_SHOW Message, int* Command, int* SubCommand, int* Code)
{
    *Command = 0;
    *SubCommand = 0;
    *Code = 0;
    char recvData[2];
    memset(recvData, 0, sizeof(recvData));
    int result = _ReadFromUpSystem(UNIT_D, SIGNAL_DEVICE, 1, recvData, Message);
    if (result == COMPLETE)
    {
        *Command = recvData[1];
        *SubCommand = recvData[0];
        if (*Command == 0 && *SubCommand == 0)
        {
            return COMPLETE;
        }

        char resetData[2];
        memset(resetData, 0, sizeof(resetData));
        result = _WriteToUpSystem(UNIT_D, SIGNAL_DEVICE, 1, resetData, Message);
        if (result == COMPLETE)
        {
            Message("up system signal reseted.");
        }

        char resultData[14];
        memset(resultData, 0, sizeof(resultData));
        int alignDataX = 1001023451;
        int alignDataY = 2871;
        int alignDataT = 15;
        switch (*Command)
        {
        case 1:
            switch (*SubCommand)
            {
            case 1:
                Message("signal: tool step1 shot.");
                resultData[12] = 0;
                resultData[13] = 3;
                break;
            case 2:
                Message("signal: tool step2 shot.");
                resultData[12] = 0;
                resultData[13] = 1;
                break;
            case 3:
                Message("signal: stage step1 shot.");
                resultData[12] = 0;
                resultData[13] = 3;
                break;
            case 4:
                Message("signal: stage step2 shot.");
                resultData[12] = 0;
                resultData[13] = 1;
                break;
            case 5:
                Message("signal: tool both shot.");
                resultData[12] = 0;
                resultData[13] = 1;
                break;
            case 6:
                Message("signal: stage both shot.");
                
                resultData[0] = alignDataX & 0x000000FF;
                resultData[1] = (alignDataX >> 8) & 0x000000FF;
                resultData[2] = (alignDataX >> 16) & 0x000000FF;
                resultData[3] = (alignDataX >> 24) & 0x000000FF;
                
                resultData[4] = alignDataY & 0x000000FF;
                resultData[5] = (alignDataY >> 8) & 0x000000FF;
                resultData[6] = (alignDataY >> 16) & 0x000000FF;
                resultData[7] = (alignDataY >> 24) & 0x000000FF;

                resultData[8] = alignDataT & 0x000000FF;
                resultData[9] = (alignDataT >> 8) & 0x000000FF;
                resultData[10] = (alignDataT >> 16) & 0x000000FF;
                resultData[11] = (alignDataT >> 24) & 0x000000FF;

                resultData[12] = 0;
                resultData[13] = 1;
                break;
            default:
                Message("signal: error shot.");
                resultData[12] = 0;
                resultData[13] = 5;
                break;
            }
            break;
        case 2:
            switch (*SubCommand)
            {
            case 1:
                Message("signal: tool calibration step1.");
                resultData[0] = 8;
                resultData[1] = 0;
                break;
            case 2:
                Message("signal: tool calibration step2.");
                resultData[0] = 8;
                resultData[1] = 0;
                break;
            case 3:
                Message("signal: tool calibration step3.");
                resultData[0] = 8;
                resultData[1] = 0;
                break;
            case 4:
                Message("signal: tool calibration step4.");
                resultData[0] = 8;
                resultData[1] = 0;
                break;
            case 5:
                Message("signal: tool calibration step5.");
                resultData[0] = 8;
                resultData[1] = 0;
                break;
            case 6:
                Message("signal: tool calibration step6.");
                resultData[0] = 8;
                resultData[1] = 0;
                break;
            case 7:
                Message("signal: tool calibration step7.");
                resultData[0] = 8;
                resultData[1] = 0;
                break;
            case 8:
                Message("signal: tool calibration step8.");
                resultData[0] = 4;
                resultData[1] = 0;
                break;
            default:
                Message("signal: error tool calibration.");
                resultData[0] = 12;
                resultData[1] = 0;
                break;
            }
            break;
        case 3:
            switch (*SubCommand)
            {
            case 1:
                Message("signal: stage calibration step1.");
                resultData[0] = 64;
                resultData[1] = 0;
                break;
            case 2:
                Message("signal: stage calibration step2.");
                resultData[0] = 64;
                resultData[1] = 0;
                break;
            case 3:
                Message("signal: stage calibration step3.");
                resultData[0] = 64;
                resultData[1] = 0;
                break;
            case 4:
                Message("signal: stage calibration step4.");
                resultData[0] = 64;
                resultData[1] = 0;
                break;
            case 5:
                Message("signal: stage calibration step5.");
                resultData[0] = 64;
                resultData[1] = 0;
                break;
            case 6:
                Message("signal: stage calibration step6.");
                resultData[0] = 64;
                resultData[1] = 0;
                break;
            case 7:
                Message("signal: stage calibration step7.");
                resultData[0] = 64;
                resultData[1] = 0;
                break;
            case 8:
                Message("signal: stage calibration step8.");
                resultData[0] = 32;
                resultData[1] = 0;
                break;
            default:
                Message("signal: error stage calibration.");
                resultData[0] = 96;
                resultData[1] = 0;
                break;
            }
            break;
        case 4:
            char recipeData[24];
            memset(recipeData, 0, sizeof(recipeData));
            result = _ReadFromUpSystem(UNIT_D, SIGNAL_DEVICE + 1, 12, recipeData, Message);
            if (result == COMPLETE)
            {
                int recipeID = recipeData[2] & 0x000000FF | (recipeData[3] << 8 & 0x0000FF00) | (recipeData[4] << 16 & 0x00FF0000) | (recipeData[5] << 24 & 0xFF000000);
                *Code = recipeID;
                std::string msg = "get recipe id: " + std::to_string(recipeID);
                Message(msg.c_str());
            }
            switch (*SubCommand)
            {
            case 1:
                if (result == COMPLETE)
                {
                    char recipeName[20];
                    memset(recipeName, 0, sizeof(recipeName));
                    for (size_t i = 0; i < sizeof(recipeName); i++)
                    {
                        recipeName[i] = recipeData[i + 5];
                    }
                    Message(recipeName);
                    resultData[12] = 1;
                    resultData[13] = 0;
                }
                break;
            case 2:
                resultData[12] = 1;
                resultData[13] = 0;
                break;
            case 3:
                resultData[12] = 1;
                resultData[13] = 0;
                break;
            case 4:
                resultData[12] = 1;
                resultData[13] = 0;
                break;
            default:
                resultData[12] = 2;
                resultData[13] = 0;
                break;
            }
            break;
        default:
            resultData[12] = 3;
            resultData[13] = 0;
            Message("error signal.");
            break;
        }
        result = _WriteToUpSystem(UNIT_D, SIGNAL_RESULT_DEVICE, 7, resultData, Message);
        if (result == COMPLETE)
        {
            Message("signal result send.");
        }
        
    }
    else
    {
        Message("read signal from up system fail.");
    }

    return result;
}

int UpSystem::InitShot(Model* m, MESSAGE_SHOW Message)
{
    char recvData[2 * SHOT_UNIT_COUNT];
    memset(recvData, 0, sizeof(recvData));
    /*int result = _ReadFromUpSystem(m->SHOT_CODE, m->SHOT_ADDR, SHOT_UNIT_COUNT, recvData, Message);
    if (result == COMPLETE)
    {

    }
    else
    {

    }*/
    return 0;
}

UpSystem& UpSystem::operator=(const UpSystem& us)
{
    this->US_TYPE = us.US_TYPE;
    this->UPSYS_CONNECTION = us.UPSYS_CONNECTION;
    this->ADDR = us.ADDR;
    this->IS_CONNECTED = us.IS_CONNECTED;
    return *this;
}

int UpSystem::_WriteToUpSystem(int code, int number, int count, char* data, MESSAGE_SHOW Message)
{
    try
    {
        char sendData[64];
        memset(sendData, 0, sizeof(sendData));
        //3E帧格式设置
        sendData[0] = 0x50;                     //副标题 L
        sendData[1] = 0x00;                     //副标题 H
        sendData[2] = 0x00;                     //网络号
        sendData[3] = 0xFF;                     //PLC编号
        sendData[4] = 0xFF;                     //请求目标模块I/O编号 L
        sendData[5] = 0x03;                     //请求目标模块I/O编号 H
        sendData[6] = 0x00;                     //请求目标模块站编号
        sendData[7] = 12 + count * 2;     //请求数据长度 L diff
        sendData[8] = 0x00;                     //请求数据长度 H
        sendData[9] = 0x10;                     //CPU监视定时器 L
        sendData[10] = 0x00;                    //CPU监视定时器 H

        sendData[11] = 0x01;                    //二进制字单位批量写 命令 L
        sendData[12] = 0x14;                    //二进制字单位批量写 命令 H
        sendData[13] = 0x00;                    //子命令 L
        sendData[14] = 0x00;                    //子命令 H

        sendData[15] = number & 0x00FF;                         //软原件编号L
        sendData[16] = ( number >> 8 ) & 0x00FF;            //软原件编号
        sendData[17] = ( number >> 16 ) & 0x00FF;         //软原件编号H
        sendData[18] = code;     //软原件代码
        sendData[19] = count & 0x00FF;                             //软原件点数L（软原件个数）
        sendData[20] = ( count >> 8 ) & 0x00FF;               //软原件点数H

        for (size_t i = 0; i < count; i++)
        {
            sendData[21 + i * 2] = *(data + i * 2);                   //写入的数据 L
            sendData[22 + i * 2] = *(data + i * 2 + 1);            //写入的数据 H
        }

        if (send(UPSYS_CONNECTION, sendData, sizeof(sendData), 0) == SOCKET_ERROR)
        {
            Message("write to up system request error.");
            return FAIL;
        }
        char recvData[64];
        memset(recvData, 0, sizeof(recvData));
        int len = recv(UPSYS_CONNECTION, recvData, sizeof(recvData), 0);
        if (len > 0)
        {
            if (recvData[7] == 0x0B)
            {
                Message("write to up system command error.");
                return FAIL;
            }
            else if (recvData[7] == 0x02)
            {
                //Message("write to up system success.");
            }
            else
            {
                Message("write to up system command unknow error.");
                return FAIL;
            }
        }
        else if (len == 0)
        {
            IS_CONNECTED = false;
            Message("connection closed.");
            return DISCONNECT;
        }
        else {
            Message("receive heart beat data error.");
            return TIMEOUT;
        }
        
    }
    catch (const std::exception& ex)
    {
        Message(ex.what());
        return OTHEREXCEPTION;
    }
    return COMPLETE;
}

int UpSystem::_ReadFromUpSystem(int code, int number, int count, char* data, MESSAGE_SHOW Message)
{
    try
    {
        char sendData[64];
        memset(sendData, 0, sizeof(sendData));
        //3E帧格式设置
        sendData[0] = 0x50;     //副标题 L
        sendData[1] = 0x00;     //副标题 H
        sendData[2] = 0x00;     //网络号
        sendData[3] = 0xFF;     //PLC编号
        sendData[4] = 0xFF;     //请求目标模块I/O编号 L
        sendData[5] = 0x03;     //请求目标模块I/O编号 H
        sendData[6] = 0x00;     //请求目标模块站编号
        sendData[7] = 0x0C;     //请求数据长度 L diff
        sendData[8] = 0x00;     //请求数据长度 H
        sendData[9] = 0x10;     //CPU监视定时器 L
        sendData[10] = 0x00;     //CPU监视定时器 H

        sendData[11] = 0x01;     //二进制字单位批量读 命令 L
        sendData[12] = 0x04;     //二进制字单位批量读 命令 H
        sendData[13] = 0x00;     //子命令 L
        sendData[14] = 0x00;     //子命令 H

        sendData[15] = number & 0x00FF;                         //软原件编号L
        sendData[16] = (number >> 8) & 0x00FF;            //软原件编号
        sendData[17] = (number >> 16) & 0x00FF;         //软原件编号H
        sendData[18] = code;     //软原件代码
        sendData[19] = count & 0x00FF;                             //软原件点数L（软原件个数）
        sendData[20] = (count >> 8) & 0x00FF;               //软原件点数H

        if (send(UPSYS_CONNECTION, sendData, sizeof(sendData), 0) == SOCKET_ERROR)
        {
            Message("read from up system request error");
            return FAIL;
        }
        char recvData[64];
        memset(recvData, 0, sizeof(recvData));
        int len = recv(UPSYS_CONNECTION, recvData, sizeof(recvData), 0);
        if (len > 0)
        {
            if (recvData[7] == 0x0B)
            {
                Message("command error.");
                return FAIL;
            }
            else if (recvData[7] - 2 == count * 2)
            {
                for (size_t i = 0; i < recvData[7] - 2; i++)
                {
                    data[i] = recvData[11 + i];
                }
                //Message("read from up system success.");
            }
            else
            {
                Message("read from up system command unknow error.");
                return FAIL;
            }
        }
        else if (len == 0)
        {
            IS_CONNECTED = false;
            Message("connection closed.");
            return DISCONNECT;
        }
        else {
            Message("receive heart beat data error.");
            return TIMEOUT;
        }
    }
    catch (const std::exception& ex)
    {
        Message(ex.what());
        return OTHEREXCEPTION;
    }
    return COMPLETE;
}