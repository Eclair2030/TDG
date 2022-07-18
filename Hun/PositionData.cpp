#include "pch.h"
#include "PositionData.h"

PositionData::PositionData()
{
	_map = double_map.begin()->second;
}

PositionData::PositionData(long b, long u, long s, long bt, long ut, long st)
{
	baseAddr = b;
	userAddr = u;
	speedAddr = s;
	baseType = bt;
	userType = ut;
	speedType = st;
	for (map<vector<int>, map<int, string>>::iterator itr = double_map.begin(); itr != double_map.end(); itr++)
	{
		if (itr->first[Map_Index::StartAddress] == baseAddr)
		{
			_map = itr->second;
		}
	}
}

int PositionData::Read(KVC* com)
{
	DBDevInfo baseInfo[POS_TABLE_SIZE];
	DBDevInfo userInfo[POS_TABLE_SIZE];
	DBDevInfo speedInfo[POS_TABLE_SIZE];
	DBERROR err = com->ReadEx(baseType, baseAddr, POS_TABLE_SIZE, baseInfo);
	err = com->ReadEx(userType, userAddr, POS_TABLE_SIZE, userInfo);
	err = com->ReadEx(speedType, speedAddr, POS_TABLE_SIZE, speedInfo);
	for (size_t i = 0; i < POS_TABLE_SIZE; i++)
	{
		baseArray[i] = baseInfo[i].lValue;
		useArray[i] = userInfo[i].lValue;
		speedArray[i] = speedInfo[i].lValue;
	}
	return err;
}

int PositionData::Write(KVC* com, UI_MESSAGE Message)
{
	DBDevInfo baseInfo[POS_TABLE_SIZE];
	DBDevInfo userInfo[POS_TABLE_SIZE];
	DBDevInfo speedInfo[POS_TABLE_SIZE];
	for (size_t i = 0; i < POS_TABLE_SIZE; i++)
	{
		baseInfo[i].lValue = i * 100;
		userInfo[i].lValue = i * 10 + 1;
		speedInfo[i].lValue = i + 5;
	}
	DBERROR err = com->WriteEx(baseType, baseAddr, POS_TABLE_SIZE, baseInfo);
	string msg = "base write: " + to_string(err);
	Message(msg.c_str(), MessageType::FunctionResult);
	err = com->WriteEx(userType, userAddr, POS_TABLE_SIZE, userInfo);
	msg = "user write: " + to_string(err);
	Message(msg.c_str(), MessageType::FunctionResult);
	err = com->WriteEx(speedType, speedAddr, POS_TABLE_SIZE, speedInfo);
	msg = "speed write: " + to_string(err);
	Message(msg.c_str(), MessageType::FunctionResult);
	return err;
}

long PositionData::GetFirstAddr()
{
	return baseAddr;
}

int PositionData::SaveToFile()
{
	return Result::SUC;
}

int PositionData::Compare(PositionData& pd, string dt, string& msg, UI_MESSAGE Message)
{
	int result = Result::SAME;
	string strShow("");
	for (size_t i = 0, j = 0; i < POS_TABLE_SIZE; i+=2,j++)
	{
		if (pd.baseArray[i] != baseArray[i] || pd.baseArray[i+1] != baseArray[i+1])
		{
			result = Result::DIFF;
			msg += _map[j] + ",base," + float_tostring((baseArray[i] + baseArray[i + 1] * HIGH_BIT) / TIMES_POSITION) + +","
				+ float_tostring((pd.baseArray[i] + pd.baseArray[i + 1] * HIGH_BIT) / TIMES_POSITION) + "," + dt + "\r";
			strShow = _map[j] + " - base: " + float_tostring((baseArray[i] + baseArray[i + 1] * HIGH_BIT) / TIMES_POSITION) + +" --> "
				+ float_tostring((pd.baseArray[i] + pd.baseArray[i + 1] * HIGH_BIT) / TIMES_POSITION) + " --time: " + dt;
			Message(strShow.c_str(), MessageType::CppProcess);
			baseArray[i] = pd.baseArray[i];
			baseArray[i+1] = pd.baseArray[i+1];
		}
		if (pd.useArray[i] != useArray[i] || pd.useArray[i + 1] != useArray[i + 1])
		{
			result = Result::DIFF;
			msg += _map[j] + ",useroffset," + float_tostring((useArray[i] + useArray[i + 1] * HIGH_BIT) / TIMES_POSITION) + +","
				+ float_tostring((pd.useArray[i] + pd.useArray[i + 1] * HIGH_BIT) / TIMES_POSITION) + "," + dt + "\r";
			strShow = _map[j] + " - base: " + float_tostring((useArray[i] + useArray[i + 1] * HIGH_BIT) / TIMES_POSITION) + +" --> "
				+ float_tostring((pd.useArray[i] + pd.useArray[i + 1] * HIGH_BIT) / TIMES_POSITION) + " --time: " + dt;
			Message(strShow.c_str(), MessageType::CppProcess);
			useArray[i] = pd.useArray[i];
			useArray[i + 1] = pd.useArray[i + 1];
		}
		if (pd.speedArray[i] != speedArray[i] || pd.speedArray[i + 1] != speedArray[i + 1])
		{
			result = Result::DIFF;
			msg += _map[j] + ",speed," + float_tostring((speedArray[i] + speedArray[i + 1] * HIGH_BIT) / TIMES_SPEED) + +","
				+ float_tostring((pd.speedArray[i] + pd.speedArray[i + 1] * HIGH_BIT) / TIMES_SPEED) + "," + dt + "\r";
			strShow = _map[j] + " - base: " + float_tostring((speedArray[i] + speedArray[i + 1] * HIGH_BIT) / TIMES_POSITION) + +" --> "
				+ float_tostring((pd.speedArray[i] + pd.speedArray[i + 1] * HIGH_BIT) / TIMES_POSITION) + " --time: " + dt;
			Message(strShow.c_str(), MessageType::CppProcess);
			speedArray[i] = pd.speedArray[i];
			speedArray[i + 1] = pd.speedArray[i + 1];
		}
	}
	
	return result;
}

string PositionData::float_tostring(float f)
{
	string num = ConvertString(f);
	string::iterator ita = num.end() - 1;
	for (; *ita == '0' && ita != num.begin(); ita--)
	{
	}
	string res(std::begin(num), ita);
	return res;
}
