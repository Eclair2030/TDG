#pragma once
#include "CustomDefine.h"

class MarkMatch
{
public:
	MarkMatch();
	int TryMatch(Mat* mark, Mat* img, double* x, double* y);
};

