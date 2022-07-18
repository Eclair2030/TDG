#include "pch.h"
#include "Model.h"

Model::Model()
{
	TOOL_SNAPED = false;
}

int Model::SetRecipe(Recipe_Model& rm, Recipe_Camera& rc1, Recipe_Camera& rc2, const char* path1_1, const char* path1_2, const char* path1_3,
	const char* path2_1, const char* path2_2, const char* path2_3, MESSAGE_SHOW Message)
{
	int result = COMPLETE;

	RECIPE = rm;
	result = CAM_LEFT.InitParameter(rc1, path1_1, path1_2, path1_3, Message) == COMPLETE && 
		CAM_RIGHT.InitParameter(rc2, path2_1, path2_2, path2_3, Message) == COMPLETE ? COMPLETE : FAIL;
	return result;
}