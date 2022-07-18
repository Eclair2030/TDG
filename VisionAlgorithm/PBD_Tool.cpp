#include "pch.h"
#include "PBD_Tool.h"

PBD_Tool::PBD_Tool():Model()
{
	STEP = SequenceSnapStep::None;
}

int PBD_Tool::SeqShot(int stepCode, Mat* src, int* markWidth, int* markHeight, int* centerX, int* centerY, MESSAGE_SHOW Message)
{
	int result = UNKNOW;
	if (stepCode == 1)
	{
		Message("here");
		result = CAM_LEFT.Snap(src, stepCode, markWidth, markHeight, centerX, centerY, Message);
		STEP = SequenceSnapStep::OneTaken;
	}
	else if (stepCode == 2)
	{
		if (STEP != SequenceSnapStep::OneTaken)
		{
			std::string msg = "First snap has not taken.";
			Message(msg.c_str());
			return FAIL;
		}
		CAM_LEFT.Snap(src, stepCode, markWidth, markHeight, centerX, centerY, Message);
		STEP = SequenceSnapStep::Empty;
		result = COMPLETE;
	}
	else
	{
		std::string msg = "Wrong tool snap stepCode.";
		Message(msg.c_str());
		result = FAIL;
	}
	return result;
}
