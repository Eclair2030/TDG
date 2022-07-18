#pragma once
class KVC
{
public:
	KVC();

	DBERROR Connect();
	DBERROR DisConnect();
	//DBERROR Read(DBDevInfo *lpDevInfo);
	DBERROR ReadFast(WORD kind, DWORD firstNum, LONG count, DBDevInfo* aDevInfo);
	DBERROR ReadEx(WORD kind, DWORD firstNum, LONG count, DBDevInfo* aDevInfo);
	DBERROR WriteEx(WORD kind, DWORD firstNum, LONG count, DBDevInfo* aDevInfo);

	DBERROR TriggerSet();
	DBERROR TriggerReset(Result& r);
	DBERROR TriggerOn();
	DBERROR TriggerOff();
	DBERROR Edit(int value);

	
private:
	DBHCONNECT _conn;
};

