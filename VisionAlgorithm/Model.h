#pragma once
#include "Camera.h"

typedef struct Recipe_Model
{
	int MODEL_RECIPE_CODE;//此工位的Recipe编号
	double DELAY;//拍照延迟时间(s)
	double OFFSET_X;//补偿值(mm)
	double OFFSET_Y;
	double OFFSET_T;
	double SPEC_X;//精度(mm)
	double SPEC_Y;
	double SPEC_T;
	double Y_MAP;//Y Map的角度(斜电极与水平线夹角)
	bool USE_Y_MAP;//是否启用Y MAP

};

class Model
{
public:
	Model();
	int SetRecipe(Recipe_Model& rm, Recipe_Camera& rc1, Recipe_Camera& rc2, const char* path1_1, const char* path1_2, const char* path1_3,
		const char* path2_1, const char* path2_2, const char* path2_3, MESSAGE_SHOW Message);

	int PreBondToolSnap(Mat* src1, int* markWidth1, int* markHeight1, int* centerX1, int* centerY1, 
											Mat* src2, int* markWidth2, int* markHeight2, int* centerX2, int* centerY2, MESSAGE_SHOW Message);
	int StageSnap();
	Recipe_Model GetRecipe();
	Recipe_Camera GetCamera1Recipe();
	Recipe_Camera GetCamera2Recipe();

	Camera CAM_LEFT;
	Camera CAM_RIGHT;
	Recipe_Model RECIPE;
private:
	bool TOOL_SNAPED;									//压头是否已拍照的标识
	double IC_PITCH;											//压头上IC的Mark Pitch
	Point2d IC_TARGET1, IC_TARGET2;			//压头上IC左右Mark在左相机中的坐标
};
