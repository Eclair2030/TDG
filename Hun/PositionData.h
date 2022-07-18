#pragma once
#include "General.h"
#include "KVC.h"

class PositionData
{
public:
	PositionData();
	PositionData(long b, long u, long s, long bt, long ut, long st);

	int Read(KVC* com);
	int Write(KVC* com, UI_MESSAGE Message);
	long GetFirstAddr();
	int SaveToFile();
	int Compare(PositionData& pd, string dt, string& msg, UI_MESSAGE Message);
	

private:
	string float_tostring(float f);

	long baseAddr, baseType;
	long userAddr, userType;
	long speedAddr, speedType;
	long baseArray[POS_TABLE_SIZE];
	long useArray[POS_TABLE_SIZE];
	long speedArray[POS_TABLE_SIZE];
	map<int, string> _map;
};

