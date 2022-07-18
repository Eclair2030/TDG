#pragma once
#include "Model.h"
class PBD_Tool :
    public Model
{
public:
    PBD_Tool();
    int SeqShot(int stepCode, Mat* src, int* markWidth, int* markHeight, int* centerX, int* centerY, MESSAGE_SHOW Message);
    int SHOT_CODE;
    int SHOT_ADDR;

private:
    SequenceSnapStep STEP;
};

