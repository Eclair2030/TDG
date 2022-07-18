#pragma once
#include "Camera.h"

typedef struct Recipe_Model
{
	int MODEL_RECIPE_CODE;//�˹�λ��Recipe���
	double DELAY;//�����ӳ�ʱ��(s)
	double OFFSET_X;//����ֵ(mm)
	double OFFSET_Y;
	double OFFSET_T;
	double SPEC_X;//����(mm)
	double SPEC_Y;
	double SPEC_T;
	double Y_MAP;//Y Map�ĽǶ�(б�缫��ˮƽ�߼н�)
	bool USE_Y_MAP;//�Ƿ�����Y MAP

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
	bool TOOL_SNAPED;									//ѹͷ�Ƿ������յı�ʶ
	double IC_PITCH;											//ѹͷ��IC��Mark Pitch
	Point2d IC_TARGET1, IC_TARGET2;			//ѹͷ��IC����Mark��������е�����
};
