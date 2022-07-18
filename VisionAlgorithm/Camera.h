#pragma once
#include "Mark.h"

typedef struct Recipe_Camera
{
	std::vector<Mark> MARK_LIST;							//Mark列表
	int ENABLE[3]{ 0, 0, 0 };											//Mark禁用列表(0：禁用，1：启用)
	MarkCenter MarkPoint[3];										//Mark参考点坐标(相对于Mark左上角开始计算)
	double RotateCenter_X, RotateCenter_Y;			//pixel
	double Resolution_X, Resolution_Y;					// mm/pixel
	double Alpha;															//相机X轴逆时针到电机X轴的角度
	double Beta;																//相机Y轴逆时针到电机Y轴的角度
};

/// <summary>
/// 一个Camera对象代表一次拍照的一个相机对象，左右Mark轮流拍的相机和左右Mark同时拍照的相机在工位中建立两个Camera对象
/// </summary>
class Camera
{
public:
	Camera();
	int InitParameter(Recipe_Camera& rc, const char* path1, const char* path2, const char* path3, MESSAGE_SHOW Message);

	/// <summary>
	/// 相机拍照
	/// </summary>
	/// <param name="src">原图</param>
	/// <param name="stepCode">拍照步骤序号</param>
	/// <param name="markWidth">Mark宽度</param>
	/// <param name="markHeight">Mark高度</param>
	/// <param name="centerX">Mark参考点在原图中的X坐标</param>
	/// <param name="centerY">Mark参考点在原图中的Y坐标</param>
	/// <param name="Message">文本信息显示函数</param>
	/// <returns>执行结果</returns>
	int Snap(Mat* src, int stepCode, int* markWidth, int* markHeight, int* centerX, int* centerY, MESSAGE_SHOW Message);

	

	Recipe_Camera GetRecipe();
	
private:
	int _MatchMark(Mat* src, Mat* mark, int* centerX, int* centerY, MESSAGE_SHOW Message);
	Recipe_Camera RECIPE;
	std::vector<Mat> MARKLIST;
};
