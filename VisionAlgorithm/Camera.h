#pragma once
#include "Mark.h"

typedef struct Recipe_Camera
{
	std::vector<Mark> MARK_LIST;							//Mark�б�
	int ENABLE[3]{ 0, 0, 0 };											//Mark�����б�(0�����ã�1������)
	MarkCenter MarkPoint[3];										//Mark�ο�������(�����Mark���Ͻǿ�ʼ����)
	double RotateCenter_X, RotateCenter_Y;			//pixel
	double Resolution_X, Resolution_Y;					// mm/pixel
	double Alpha;															//���X����ʱ�뵽���X��ĽǶ�
	double Beta;																//���Y����ʱ�뵽���Y��ĽǶ�
};

/// <summary>
/// һ��Camera�������һ�����յ�һ�������������Mark�����ĵ����������Markͬʱ���յ�����ڹ�λ�н�������Camera����
/// </summary>
class Camera
{
public:
	Camera();
	int InitParameter(Recipe_Camera& rc, const char* path1, const char* path2, const char* path3, MESSAGE_SHOW Message);

	/// <summary>
	/// �������
	/// </summary>
	/// <param name="src">ԭͼ</param>
	/// <param name="stepCode">���ղ������</param>
	/// <param name="markWidth">Mark���</param>
	/// <param name="markHeight">Mark�߶�</param>
	/// <param name="centerX">Mark�ο�����ԭͼ�е�X����</param>
	/// <param name="centerY">Mark�ο�����ԭͼ�е�Y����</param>
	/// <param name="Message">�ı���Ϣ��ʾ����</param>
	/// <returns>ִ�н��</returns>
	int Snap(Mat* src, int stepCode, int* markWidth, int* markHeight, int* centerX, int* centerY, MESSAGE_SHOW Message);

	

	Recipe_Camera GetRecipe();
	
private:
	int _MatchMark(Mat* src, Mat* mark, int* centerX, int* centerY, MESSAGE_SHOW Message);
	Recipe_Camera RECIPE;
	std::vector<Mat> MARKLIST;
};
