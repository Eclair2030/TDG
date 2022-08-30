#include "pch.h"
#include "Staff.h"

Staff::Staff()
{

}

bool Staff::FindEmptyStaff(Mat* src, int* x, int* y, int* r)
{
	bool result = false;

	Mat thres;
	threshold(*src, thres, 20, 255, THRESH_BINARY);
	//【4】执行形态学开操作去除噪点
	Mat kernel = getStructuringElement(MORPH_RECT, Size(7, 7), Point(-1, -1));
	morphologyEx(thres, thres, MORPH_DILATE, kernel, Point(-1, -1), 1);

	//【5】边缘检测
	Canny(thres, thres, 0, 255);
	imshow("canny", thres);

	//【6】轮廓发现
	std::vector<std::vector<Point>> contours;
	std::vector<Vec4i> her;
	findContours(thres, contours, her, RETR_TREE, CHAIN_APPROX_SIMPLE);
	double area = 0.0;
	for (size_t i = 0; i < contours.size(); i++) {
		double area = contourArea(contours[i], false);
		//【7】根据面积及纵横比过滤轮廓
		if (area > 10) {
			Rect rect = boundingRect(contours[i]);
			float scale = float(rect.width) / float(rect.height);
			//if (scale < 1.5 && scale>0.7) 
			{
				*x = rect.x + rect.width / 2;
				*y = rect.y + rect.height / 2;
				*r = (rect.width + rect.height) / 4;
				result = true;
				break;
			}
		}
	}

	return result;
}