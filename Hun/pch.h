// pch.h: 这是预编译标头文件。
// 下方列出的文件仅编译一次，提高了将来生成的生成性能。
// 这还将影响 IntelliSense 性能，包括代码完成和许多代码浏览功能。
// 但是，如果此处列出的文件中的任何一个在生成之间有更新，它们全部都将被重新编译。
// 请勿在此处添加要频繁更新的文件，这将使得性能优势无效。

#ifndef PCH_H
#define PCH_H

// 添加要在此处预编译的标头
#include "framework.h"

#include "General.h"
using namespace std;
#include <vector>
#include <map>
#include <string>
#include <fstream>
#include <sstream>
#include <time.h>
#include <thread>
#include <mutex>

#include <DataBuilder.h>

const int TRIG_1STDEVNUM = 0;
const int TRIG_DEVTYPE = DKV8K_ZF;
const int TRIG_DEVCOUNT = 16;
const float TIMES_POSITION = 10000;
const float TIMES_SPEED = 100;
const int HIGH_BIT = 65535;
const int POS_TABLE_SIZE = 40;
const int RESET_DEVNUM = 50;
const int RESET_DEVTYPE = DKV8K_ZF;
const string DEFAULT_FILENAME = "UNKNOW";
const string FILE_PATH = "C://Users//10273//";
const string TABLE_HEAD = "axis,position,before,after,time\r";
const string BASE_FILE_PATH = "C://Users//10273//";
const string BASE_TABLE_HEAD = "name,before,after,time\r";
static const wchar_t* PLC_ADDR = L"192.168.0.20:8500";
const DBPlcId PLC_ID = DBPlcId::DBPLC_DKV8K;


static map<vector<int>, map<int, string>> double_map =
{
	{
		{
			60000,										//首地址
			DKV8K_ZF,									//基准寄存器类型
			DKV8K_ZF,									//user offset寄存器类型
			DKV8K_ZF									//速度寄存器类型
		},
		{
			{0, "Plasma_X_Standby"},		//teaching表行号索引，行名称（首行索引必须是0，而且依次累加）
			{1, "Plasma_X_Load"},
			{2, "Plasma_X_Vision"},
			{3, "Plasma_X_下料位"},
		}
	},

	{
		{
			30000,
			DKV8K_ZF,
			DKV8K_ZF,
			DKV8K_ZF
		},
		{
			{0, "Plasma_Y_Standby"},
			{1, "Plasma_Y_Load"},
			{2, "Plasma_Y_Vision"},
			{3, "Plasma_Y_Unload"},
		}
	},
};

static map<string, vector<long>> brecipe_map =
{
	{
		"Plasma_xxxx",
		{
			10000,									//寄存器地址
			DKV8K_ZF,								//寄存器类型
			2,												//所占寄存器数量
		}
	},

	{
		"Prebond_yyyy",
		{
			15000,
			DKV8K_ZF,	
			2,
		}
	},
};

static string ConvertString(float Num)
{
	ostringstream oss;
	oss << Num;
	string str(oss.str());
	return str;
}

#endif //PCH_H
