#include "pch.h"
#include "Calib.h"

Calib::Calib()
{
	_Signal = 0;
}

Calib::Calib(int s, double rcx, double rcy, double rx, double ry)
{
	_Signal = s;
}

int Calib::Start()
{
	_Signal = 1;
	_Step = 0;
	return COMPLETE;
}

int Calib::Stop()
{
	int result = UNKNOW;
	_Signal = 0;
	return result;
}

int Calib::NextStep()
{
	int result = UNKNOW;
	if (_Step < 0 || _Step > _MaxStep)
	{
		result = OTHEREXCEPTION;
		return result;
	}
	switch (_Step)
	{
	case 0:	//在拍照位拍第1照
		break;
	case 1:	//X+方向第2照
		break;
	case 2:	//X-方向第3照
		break;
	case 3:	//Y+方向第4照
		break;
	case 4:	//Y-方向第5照
		break;
	case 5:	//T+方向第6照
		break;
	case 6:	//T-方向第7照
		break;
	default:
		result = OTHEREXCEPTION;
		break;
	}
	_Step++;
	return result;
}