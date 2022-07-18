#include "pch.h"
#include "KVC.h"
KVC::KVC()
{
}

DBERROR KVC::Connect()
{
	return DBConnectW(PLC_ADDR, PLC_ID, &_conn);
}

DBERROR KVC::DisConnect()
{
	return DBDisconnect(&_conn);
}

//DBERROR KVC::Read(DBDevInfo *lpDevInfo)
//{
//	return DBRead(_conn, lpDevInfo);
//}

DBERROR KVC::ReadFast(WORD kind, DWORD firstNum, LONG count, DBDevInfo* aDevInfo)
{
	return DBReadArea(_conn, kind, firstNum, count, aDevInfo);
}

DBERROR KVC::ReadEx(WORD kind, DWORD firstNum, LONG count, DBDevInfo* aDevInfo)
{
	return DBReadLong(_conn, kind, firstNum, count, aDevInfo);
}

DBERROR KVC::WriteEx(WORD kind, DWORD firstNum, LONG count, DBDevInfo* aDevInfo)
{
	return DBWriteLong(_conn, kind, firstNum, count, aDevInfo);
}

DBERROR KVC::TriggerSet()
{
	DBDevInfo info;
	info.wKind = RESET_DEVTYPE;
	info.dwNo = RESET_DEVNUM;
	info.lValue = 1;
	return DBWrite(_conn, &info);
}

DBERROR KVC::TriggerReset(Result& r)
{
	DBDevInfo info;
	DBERROR result = DB_NOERROR;
	info.wKind = RESET_DEVTYPE;
	info.dwNo = TRIG_1STDEVNUM;
	DBRead(_conn, &info);
	if (info.lValue == 0)
	{
		info.dwNo = RESET_DEVNUM;
		info.lValue = 0;
		result = DBWrite(_conn, &info);
		if (result == DB_NOERROR)
		{
			r = Result::SUC;
		}
	}
	return result;
}

DBERROR KVC::TriggerOn()
{
	DBDevInfo info;
	info.wKind = TRIG_DEVTYPE;
	info.dwNo = TRIG_1STDEVNUM + 11;
	info.lValue = 0;
	DBWrite(_conn, &info);

	info.wKind = TRIG_DEVTYPE;
	info.dwNo = TRIG_1STDEVNUM + 10;
	info.lValue = CommandCode::Edit;
	DBWrite(_conn, &info);

	info.wKind = TRIG_DEVTYPE;
	info.dwNo = TRIG_1STDEVNUM;
	info.lValue = Trigger_Signal::On;
	return DBWrite(_conn, &info);
}

DBERROR KVC::TriggerOff()
{
	DBDevInfo info;
	info.wKind = TRIG_DEVTYPE;
	info.dwNo = TRIG_1STDEVNUM;
	info.lValue = Trigger_Signal::Off;
	return DBWrite(_conn, &info);
}

DBERROR KVC::Edit(int value)
{
	DBDevInfo info;
	info.wKind = DKV8K_ZF;
	info.dwNo = 60003;
	info.lValue = value;
	DBWrite(_conn, &info);

	info.dwNo = 60005;
	info.lValue = value + 10;
	DBWrite(_conn, &info);

	info.dwNo = 10000;
	info.lValue = value + 20;
	return DBWrite(_conn, &info);
}