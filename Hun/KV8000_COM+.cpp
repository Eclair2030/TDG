// KV8000_COM+.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include<fstream>
#include<ACPathManager2.h>
#include<DataBuilder.h>
#include<DBPlcDef.h>
#include<time.h>
#include <Windows.h>

using namespace std;

const int Read_NUM = 5000;
DBHCONNECT hConnect;
DBERROR err=1;
DBDevInfo DevInfo_Read;
DBDevInfo DevInfo_Write;
DBDevInfo aDevInfo_Read_Area[Read_NUM];


//typedef 



#pragma comment(lib, "ws2_32.lib")
void OpenSocket();



int main()
{
    //time(0);
    err=DBConnectA("192.168.0.20:8500", DBPLC_DKV8K,&hConnect);
    if (err != DB_NOERROR)
    {

    }

    DevInfo_Read.wKind = DKV8K_DM;
    DevInfo_Read.dwNo = 0;

    DevInfo_Write.wKind = DKV8K_DM;
    DevInfo_Write.dwNo = 20000;
    DevInfo_Write.lValue = 500;

    err=DBWrite(hConnect, &DevInfo_Write);

    
    while (1) {
        //DBRead(hConnect, &DevInfo_Read);
        err=DBReadLong(hConnect, DKV8K_DM, 0, Read_NUM, aDevInfo_Read_Area);
        cout << err << endl;
       // int a = DevInfo_Read.lValue;
        //cout << a << endl;
        for (int i = 0; i < Read_NUM; i++) {
            //cout << aDevInfo_Read_Area[i].lValue << endl;
            Sleep(0);
            int a= aDevInfo_Read_Area[i].lValue;
            ofstream Excel;
            Excel.open("C://Users//tdgmt2021//www.csv", ios::app);
            Excel << "DM";
            Excel << i;
            Excel << " ,";
            Excel << aDevInfo_Read_Area[i].lValue;
            Excel << " ,";
            Excel << aDevInfo_Read_Area[i].lValue;
            Excel << endl;
            Excel.close();
            
        }
        break;
        //Sleep(100000);
    }

   
    //Excel.open("C://Users//tdgmt2021//www.CSV",ios::app);
    ////Excel.seekp(0,ios::end);
    //Excel << "软元,件a,g,m\r";
    //Excel.close();

	//OpenSocket();

    system("pause");
}

void OpenSocket()
{
	//定义长度变量
	int send_len = 0;
	int recv_len = 0;
	//定义发送缓冲区和接受缓冲区
	char send_buf[100];
	char recv_buf[100];
	//定义服务端套接字，接受请求套接字
	SOCKET s_server;
	//服务端地址客户端地址
	SOCKADDR_IN server_addr;

	WORD w_req = MAKEWORD(2, 2);//版本号
	WSADATA wsadata;
	int err;
	err = WSAStartup(w_req, &wsadata);
	if (err != 0) {
		cout << "初始化套接字库失败！" << endl;
	}
	else {
		cout << "初始化套接字库成功！" << endl;
	}
	//检测版本号
	if (LOBYTE(wsadata.wVersion) != 2 || HIBYTE(wsadata.wHighVersion) != 2) {
		cout << "套接字库版本号不符！" << endl;
		WSACleanup();
	}
	else {
		cout << "套接字库版本正确！" << endl;
	}
	//填充服务端地址信息
	// 
	//填充服务端信息
	server_addr.sin_family = AF_INET;
	server_addr.sin_addr.S_un.S_addr = inet_addr("192.168.0.200");
	server_addr.sin_port = htons(1030);
	//创建套接字
	s_server = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	int result = connect(s_server, (SOCKADDR*)&server_addr, sizeof(SOCKADDR));
	if (result == SOCKET_ERROR) {
		cout << "服务器连接失败！" << endl;
		WSACleanup();
	}
	else {
		cout << "服务器连接成功！" << endl;
	}

	//发送,接收数据
	//while (1) {
		/*cout << "请输入发送信息:";*/
		
	char read_buff_[1024]{ 0 };
	read_buff_[0] = 0x50; // 副头部
	read_buff_[1] = 0x00; // 副头部
	read_buff_[2] = 0x00; // 网络编号
	read_buff_[3] = (char)0xff; // 可编程控制器编号
	read_buff_[4] = (char)0xff; // 请求目标模块I/O编号L
	read_buff_[5] = 0x03; // 请求目标模块I/O编号H
	read_buff_[6] = 0x00; // 请求目标模块站号

	read_buff_[7] = 0x0c; //请求数据长度L
	read_buff_[8] = 0x00; //请求数据长度H

	//CPU 监视定时器 0:无限等待 0001～FFFF H (1～65535): 等待时间(单位 250ms) @《三菱Q_L系列通信协议参考》3.1.3 P73
	read_buff_[9] = 0x01;  // CPU监视定时器L
	read_buff_[10] = 0x00; // CPU监视定时器H

	// 0401  T寄存器读取 @《三菱Q_L系列通信协议参考》3.3.6 P150
	// WARNING: M寄存器读取 send_buff_[18] = 0x90 待确认
	read_buff_[11] = 0x01; // 指令L
	read_buff_[12] = 0x04; // 指令H
	read_buff_[13] = 0x00; // 子指令L
	read_buff_[14] = 0x00; // 子指令H

	// 字单位的批量写入
	read_buff_[15] = 0x00; // 起始软元件
	read_buff_[16] = 0x00; // 起始软元件
	read_buff_[17] = 0x00; // 起始软元件
	read_buff_[18] = 0x00; // 软元件代码
	read_buff_[19] = 0x00; // 软元件点数
	read_buff_[20] = 0x00; // 软元件点数

	
	// 字单位的批量写入
	int start_addr = 0;
	int plc_code = 0xA8;
	int len = 100;
	//////------------------------read
	read_buff_[15] = start_addr & 0xff;         // 起始软元件
	read_buff_[16] = (start_addr >> 8) & 0xff;  // 起始软元件
	read_buff_[17] = (start_addr >> 16) & 0xff; // 起始软元件
	read_buff_[18] = plc_code;  // 软元件代码
	read_buff_[19] = len & 0xff;         // 软元件点数
	read_buff_[20] = (len & 0xff00) >> 8;// 软元件点数
	//////------------------------read



	//////-----------------------write
	char write_buff_[2048]{ 0 };
	write_buff_[0] = 0x50; // 副头部
	write_buff_[1] = 0x00; // 副头部
	write_buff_[2] = 0x00; // 网络编号
	write_buff_[3] = (char)0xff; // 可编程控制器编号
	write_buff_[4] = (char)0xff; // 请求目标模块I/O编号L
	write_buff_[5] = 0x03; // 请求目标模块I/O编号H
	write_buff_[6] = 0x00; // 请求目标模块站号

	// 长度从CPU监视定时器L到指令结束
	write_buff_[7] = 0x00; //请求数据长度L
	write_buff_[8] = 0x00; //请求数据长度H

	//CPU 监视定时器 0:无限等待 0001～FFFF H (1～65535): 等待时间(单位 250ms) @《三菱Q_L系列通信协议参考》3.1.3 P73
	write_buff_[9] = 0x01;  // CPU监视定时器L
	write_buff_[10] = 0x00; // CPU监视定时器H

	// 指令1401 @《三菱Q_L系列通信协议参考》3.3.7 P154
	write_buff_[11] = 0x01; // 指令L
	write_buff_[12] = 0x14; // 指令H
	write_buff_[13] = 0x00; // 子指令L
	write_buff_[14] = 0x00; // 子指令H

	// 字单位的批量写入
	write_buff_[15] = 0x00; // 起始软元件
	write_buff_[16] = 0x00; // 起始软元件
	write_buff_[17] = 0x00; // 起始软元件
	write_buff_[18] = 0x00; // 软元件代码
	write_buff_[19] = 0x00; // 软元件点数
	write_buff_[20] = 0x00; // 软元件点数

	write_buff_[7] = (len * 2 + 12) & 0xff;          //请求数据长度L
	write_buff_[8] = ((len * 2 + 12) & 0xff00) >> 8; //请求数据长度H

	// 字单位的批量写入
	write_buff_[15] = start_addr & 0xff;         // 起始软元件
	write_buff_[16] = (start_addr >> 8) & 0xff;  // 起始软元件
	write_buff_[17] = (start_addr >> 16) & 0xff; // 起始软元件
	write_buff_[18] = plc_code;  // 软元件代码
	write_buff_[19] = len & 0xff;         // 软元件点数
	write_buff_[20] = (len & 0xff00) >> 8;// 软元件点数


	for (int i = 0; i < len; i++)
	{
		write_buff_[21 + i * 2] = 1000& 0xff;
		write_buff_[21 + i * 2 + 1] = (1000 & 0xff00) >> 8;
	}
	//////-----------------------write

	do{

		
	send_len = send(s_server, write_buff_, 21 + len * 2, 0);
	if (send_len < 0) {
		cout << "发送失败！" << endl;
	}

	const int kBlockSize = 512;
	char data[kBlockSize]{ 0 };
	recv_len = recv(s_server, data, kBlockSize, 0);
	if (recv_len < 0) {
		cout << "接受失败！" << endl;
	}
	else {
		cout << "服务端信息:" << (int)data[11]*1+ (int)data[12]*256 << endl;
	}
	
	//}
	//关闭套接字
	//closesocket(s_server);
	Sleep(100);
	} while (1);
}

// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门使用技巧: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件
