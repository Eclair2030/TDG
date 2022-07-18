// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "pch.h"
#include "KVC.h"
#include "PositionData.h"
#include "FileManager.h"
#include "BaseRecipe.h"


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

KVC* kUi = 0;
KVC* kAuto = 0;
vector<PositionData> vec;
BaseRecipe recipe;
string _posBuffer, _baseBuffer;
mutex mut;

void CalAddress(long firstAddr, long addr[])
{
	long x = (firstAddr - 60) / 100;
	addr[0] = firstAddr;
	addr[1] = 80000 + 40 * (x - 1);
	addr[2] = firstAddr - 40;
}



extern "C" _declspec(dllexport) DBERROR _stdcall InitAuto(UI_MESSAGE Message)
{
	if (kAuto == 0)
	{
		kAuto = new KVC();
	}
	
	vec.clear();
	Message("begin to connect...", MessageType::Default);
	DBERROR result = kAuto->Connect();
	string msg = "Connect Result: " + to_string(result);
	Message(msg.c_str(), MessageType::FunctionResult);
	_posBuffer = string("");
	_baseBuffer = string("");
	
	
	return result;
}

extern "C" _declspec(dllexport) DBERROR _stdcall InitUi(UI_MESSAGE Message)
{
	if (kUi == 0)
	{
		kUi = new KVC();
	}
	DBERROR result = kUi->Connect();
	string msg = "Ui Connect Result: " + to_string(result);
	Message(msg.c_str(), MessageType::FunctionResult);

	return result;
}

extern "C" _declspec(dllexport) int _stdcall FinishAuto()
{
	DBERROR err = kAuto->DisConnect();
	if (kAuto != 0)
	{
		delete kAuto;
	}
	return err;
}

extern "C" _declspec(dllexport) int _stdcall FinishUi()
{
	DBERROR err = kUi->DisConnect();
	if (kUi != 0)
	{
		delete kUi;
	}
	return err;
}

extern "C" _declspec(dllexport) LONG _stdcall Trigger(UI_MESSAGE Message, Flush_Buffer FlushBuffer)
{
	DBDevInfo info[TRIG_DEVCOUNT];
	DBERROR result = kAuto->ReadEx(TRIG_DEVTYPE, TRIG_1STDEVNUM, TRIG_DEVCOUNT, info);
	
	if (result == DB_NOERROR)
	{
		if (info[0].lValue == Trigger_Signal::On)
		{
			LONG command = info[10].lValue;
			string msg = "command signal: " + to_string(command);
			Message(msg.c_str(), MessageType::Default);
			time_t t = time(0);
			tm ttm;
			localtime_s(&ttm, &t);
			int year = ttm.tm_year + 1900;
			int month = ttm.tm_mon + 1;
			int day = ttm.tm_mday;
			switch (command)
			{
			case CommandCode::Create:
			case CommandCode::Copy:
			case CommandCode::Delete:
				break;
			case CommandCode::Edit:
				if (info[11].lValue == 0)
				{
					//Recipe base data edit.
					string strSave("");
					if (recipe.Compare(Message, strSave, kAuto, to_string(ttm.tm_hour) + ":" + to_string(ttm.tm_min) + ":" + to_string(ttm.tm_sec)) == Result::DIFF)
					{
						FileManager fmBase(ttm);
						try
						{
							Result r = fmBase.BaseWrite(strSave);
							if (r == Result::SUC)
							{
								Message("Base recipe data modified.", MessageType::FunctionResult);
							}
							else
							{
								Message(("Base recipe data modify save error: " + to_string(r) + " close csv file.").c_str(), MessageType::Error);
								//mut.lock();
								_baseBuffer += strSave;
								//mut.unlock();
								FlushBuffer(1);
								//th = thread{ &FileManager::AddtoBuffer, &fmBase, Message, &strSave, true };
								//th.detach();
							}
						}
						catch (const std::exception& e)
						{
							Message(e.what(), MessageType::Error);
						}
					}
				}
				else
				{
					//position data edit.
					long addr[3];
					CalAddress(info[11].lValue, addr);
					msg = "start address: " + to_string(info[11].lValue);
					Message(msg.c_str(), MessageType::Default);
					long bt = 0, ut = 0, st = 0;
					for (map<vector<int>, map<int, string>>::iterator itr = double_map.begin(); itr != double_map.end(); itr++)
					{
						if (itr->first[Map_Index::StartAddress] == addr[0])
						{
							bt = itr->first[Map_Index::BaseType];
							ut = itr->first[Map_Index::UserType];
							st = itr->first[Map_Index::SpeedType];
							break;
						}
					}
					PositionData pd(addr[0], addr[1], addr[2], bt, ut, st);
					pd.Read(kAuto);
					string msg("");
					FileManager fm(ttm);
					int compareResult = Result::SAME;
					for (vector<PositionData>::iterator ita = vec.begin() ; ita != vec.end(); ita++)
					{
						if (ita->GetFirstAddr() == pd.GetFirstAddr())
						{
							compareResult = ita->Compare(pd, to_string(ttm.tm_hour) + ":" + to_string(ttm.tm_min) + ":" + to_string(ttm.tm_sec), msg, Message);
						}
					}
					try
					{
						fm.Write(msg);
					}
					catch (const std::exception& e)
					{
						Message(e.what(), MessageType::Error);
						//fm.AddtoBuffer(Message, msg, false, ttm);
					}
				}
				break;
			default:
				break;
			}
			result = kAuto->TriggerSet();
			msg = "Trigger End Code: " + to_string(result);
			Message(msg.c_str(), MessageType::Default);
			while (result == 0)
			{
				Result r0 = Result::UNKNOW;
				result = kAuto->TriggerReset(r0);
				if (result == 0 || r0 == Result::SUC)
				{
					msg = "Trigger Reset Code: " + to_string(result);
					Message(msg.c_str(), MessageType::Default);
					break;
				}
				Sleep(100);
			}
		}
		else
		{//trigger signals off.
		}
	}
	return result;
}

extern "C" _declspec(dllexport) int _stdcall GetAllPosTable(UI_MESSAGE Message)
{
	long addr[3];
	int result = DB_NOERROR;
	for (map<vector<int>, map<int, string>>::iterator itr = double_map.begin(); itr != double_map.end(); itr++)
	{
		CalAddress(itr->first[Map_Index::StartAddress], addr);
		string msg = to_string(addr[0]) + ", " + to_string(addr[1]) + ", " + to_string(addr[2]);
		Message(msg.c_str(), MessageType::FunctionResult);
		PositionData pd(addr[0], addr[1], addr[2], itr->first[Map_Index::BaseType], itr->first[Map_Index::UserType], itr->first[Map_Index::SpeedType]);
		msg = to_string(itr->first[Map_Index::BaseType]) + ", " + to_string(itr->first[Map_Index::UserType]) + ", " + to_string(itr->first[Map_Index::SpeedType]);
		Message(msg.c_str(), MessageType::FunctionResult);
		result = pd.Read(kUi);
		if (result == DB_NOERROR)
		{
			vec.push_back(pd);
			msg = "Address " + to_string(addr[0]) + " readed, total size:" + to_string(vec.size());
			Message(msg.c_str(), MessageType::FunctionResult);
		}
		else
		{
			msg = "error code:" + to_string(result);
			Message(msg.c_str(), MessageType::Error);
		}
	}
	Result r = Result::UNKNOW;
	result = recipe.ReadBaseRecipedatas(Message, kUi, r);
	if (r == Result::SUC)
	{
		Message("Read all base recipe data success.", MessageType::FunctionResult);
	}
	else
	{
		Message("Error occur in read all base recipe data.", MessageType::Error);
	}
	
	return result;
}

extern "C" _declspec(dllexport) Result _stdcall FlushRecordBuffer(UI_MESSAGE Message, Flush_Buffer FlushBuffer)
{
	Result result = Result::UNKNOW;
	if (_baseBuffer.empty() && _posBuffer.empty())
	{
		FlushBuffer(0);
		result = Result::EMPTY;
	}
	else
	{
		time_t t = time(0);
		tm ttm;
		localtime_s(&ttm, &t);
		FileManager fm(ttm);
		if (fm.FlushBuffer(Message, _posBuffer, _baseBuffer))
		{
			FlushBuffer(0);
			result = Result::SUC;
		}
		else
		{
			result = Result::FAIL;
		}
	}
	return result;
}




extern "C" _declspec(dllexport) int _stdcall SetRecipe_ForTest(UI_MESSAGE Message)
{
	long addr[3];
	CalAddress(60000, addr);
	string msg = to_string(addr[0]) + ", " + to_string(addr[1]) + ", " + to_string(addr[2]);
	Message(msg.c_str(), MessageType::FunctionResult);
	PositionData pd(addr[0], addr[1], addr[2], DKV8K_ZF, DKV8K_ZF, DKV8K_ZF);
	int result = pd.Write(kUi, Message);
	if (result == DB_NOERROR)
	{
		Message("Write complete", MessageType::FunctionResult);
	}
	else
	{
		string msg = "Data write error : " + to_string(result);
		Message(msg.c_str(), MessageType::Error);
	}

	return result;
}

extern "C" _declspec(dllexport) int _stdcall Edit_ForTest(UI_MESSAGE Message, int value)
{
	int result = kUi->Edit(value);

	if (result == DB_NOERROR)
	{
		Message("Data modified", MessageType::FunctionResult);
	}
	else
	{
		string msg = "Data modify error : " + to_string(result);
		Message(msg.c_str(), MessageType::Error);
	}

	return result;
}

extern "C" _declspec(dllexport) int _stdcall Trigger_ForTest(UI_MESSAGE Message)
{
	int result = kUi->TriggerOn();

	if (result == DB_NOERROR)
	{
		Message("Trigger sended", MessageType::FunctionResult);
	}

	return result;
}

extern "C" _declspec(dllexport) int _stdcall TriggerReset_ForTest(UI_MESSAGE Message)
{
	int result = kUi->TriggerOff();

	if (result == DB_NOERROR)
	{
		Message("Trigger off", MessageType::FunctionResult);
	}


	return result;
}