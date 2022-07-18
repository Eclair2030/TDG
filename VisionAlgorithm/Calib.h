#pragma once
#include "CustomDefine.h"

class Calib
{
public:
	Calib();
	Calib(int, double, double, double, double);
	int Start();
	int Stop();
	int NextStep();

private:
	int _Signal;		//signal for busy, 0: idle; 1: calibrationing
	int _Step;	//step number
	int _MaxStep;
	
};

