#include "pch.h"
#include "BaseRecipe.h"
BaseRecipe::BaseRecipe()
{
}

int BaseRecipe::GetRecipeID()
{
	return _id;
}

int BaseRecipe::SetRecipeID(int id)
{
	_id = id;
	return Result::SUC;
}

DBERROR BaseRecipe::ReadBaseRecipedatas(UI_MESSAGE Message, KVC* k, Result& r)
{
	DBERROR result = DB_NOERROR;
	for (map<string, vector<long>>::iterator itr = brecipe_map.begin(); itr != brecipe_map.end(); itr++)
	{
		DBDevInfo* info = new DBDevInfo[itr->second[2]];
		result = k->ReadFast(itr->second[1], itr->second[0], itr->second[2], info);
		if (result == DB_NOERROR)
		{
			long data = info->lValue;
			if (itr->second[2] == 2)
			{
				DBDevInfo* info1 = info + 1;
				data += HIGH_BIT * info1->lValue;
			}
			if (itr->second.size() == 3)
			{
				itr->second.push_back(data);
			}
			else if (itr->second.size() == 4)
			{
				itr->second[3] = data;
			}
			else
			{
				Message("Base recipe address data type error.", MessageType::Error);
			}
			
			r = Result::SUC;
			Message(("Base recipe address " + to_string(itr->second[0]) + " data: " + to_string(itr->second[3])).c_str(), MessageType::Default);
		}
		else
		{
			r = Result::FAIL;
			Message(("Base recipe address " + to_string(itr->second[0]) + " fail.").c_str(), MessageType::Error);
		}
		delete[] info;
	}
	return result;
}

Result BaseRecipe::Compare(UI_MESSAGE Message, string& msg, KVC* k, string dt)
{
	Result r = Result::SAME;
	for (map<string, vector<long>>::iterator itr = brecipe_map.begin(); itr != brecipe_map.end(); itr++)
	{
		if (itr->second.size() == 4)
		{
			long dataMem = itr->second[3];
			DBDevInfo* info = new DBDevInfo[itr->second[2]];
			if (k->ReadFast(itr->second[1], itr->second[0], itr->second[2], info) == DB_NOERROR)
			{
				long dataPlc = info->lValue;
				if (itr->second[2] == 2)
				{
					DBDevInfo* info1 = info + 1;
					dataPlc += HIGH_BIT * info1->lValue;
				}
				if (dataMem != dataPlc)
				{
					r = Result::DIFF;
					itr->second[3] = dataPlc;
					msg += itr->first + "," + to_string(dataMem) + "," + to_string(dataPlc) + "," + dt + "\r";
					Message(("Base recipe " + itr->first + " data: " + to_string(dataMem) + " --> " + to_string(dataPlc) + " -- time: " + dt).c_str(), MessageType::CppProcess);
				}
			}
			else
			{
				r = Result::CONNERR;
				Message(("Base recipe address " + to_string(itr->second[0]) + " fail.").c_str(), MessageType::Error);
			}
			delete[] info;
		}
		else if (itr->second.size() == 3)
		{
			r = Result::FAIL;
			Message("Base recipe data not gain complete, get base recipe from PLC first.", MessageType::Error);
		}
		else
		{
			r = Result::FAIL;
			Message("Base recipe data struct error, repair Code from visual studio.", MessageType::Error);
		}
	}
	return r;
}
