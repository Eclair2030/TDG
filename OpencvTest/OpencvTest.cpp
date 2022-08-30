// OpencvTest.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include "pch.h"
//#include "MasterYe.h"
//#include<ACPathManager2.h>
//#include<DataBuilder.h>
//#include<DBPlcDef.h>
//#include <filesystem>
//#include <fstream>
//#include <direct.h>

#include <bitset>
#include <cmath>

using namespace cv;
//using namespace cv::ml;
//using namespace std;

void Sign()
{
	Mat img = imread("D:\\zy.bmp", IMREAD_GRAYSCALE);
	std::cout << "rows: " << img.rows << std::endl;
	std::cout << "cols: " << img.cols << std::endl;
	for (size_t i = 0; i < img.rows; i++)
	{
		for (size_t j = 0; j < img.cols; j++)
		{
			if ((int)img.at<uchar>(i, j) > 100)
			{
				img.at<uchar>(i, j) = 255;
			}
		}
	}

	Mat des(img.rows, img.cols, CV_8UC4);
	for (size_t i = 0; i < img.rows; i++)
	{
		for (size_t j = 0; j < img.cols; j++)
		{
			uchar u = img.at<uchar>(i, j);
			if ((int)u > 100)
			{
				des.at<Vec4b>(i, j)[0] = 255;
				des.at<Vec4b>(i, j)[1] = 255;
				des.at<Vec4b>(i, j)[2] = 255;
				des.at<Vec4b>(i, j)[3] = 0;
			}
			else
			{
				des.at<Vec4b>(i, j)[0] = 0;
				des.at<Vec4b>(i, j)[1] = 0;
				des.at<Vec4b>(i, j)[2] = 0;
				des.at<Vec4b>(i, j)[3] = 255;
			}
		}
	}
	imshow("bbb", des);
	imwrite("D:\\zy.png", des);
}

int main(int argc, char** argv)
{
	Mat Pic = imread("D:\\AGV1.bmp", IMREAD_GRAYSCALE);
	Mat thres;
	threshold(Pic, thres, 20, 255, THRESH_BINARY);
	imshow("threshold", thres);
	//【4】执行形态学开操作去除噪点
	Mat kernel = getStructuringElement(MORPH_RECT, Size(7, 7), Point(-1, -1));
	morphologyEx(thres, thres, MORPH_DILATE, kernel, Point(-1, -1), 1);
	imshow("morphologyEx", thres);

	//【5】边缘检测
	Canny(thres, thres, 0, 255);
	imshow("canny", thres);

	//【6】轮廓发现
	std::vector<std::vector<Point>> contours;
	std::vector<Vec4i> her;
	findContours(thres, contours, her, RETR_TREE, CHAIN_APPROX_SIMPLE);
	Mat resultImage;
	cvtColor(Pic, resultImage, COLOR_GRAY2BGR);
	RNG rng(12345);
	double area = 0.0;
	Point pRadius;
	for (size_t i = 0; i < contours.size(); i++) {
		double area = contourArea(contours[i], false);
		std::cout << area << std::endl;
		//【7】根据面积及纵横比过滤轮廓
		if (area > 50) {
			Rect rect = boundingRect(contours[i]);
			float scale = float(rect.width) / float(rect.height);
			//if (scale < 1.5 && scale>0.7) 
			{
				drawContours(resultImage, contours, i, Scalar(0, 0, 255), 2);
				int x = rect.width / 2;
				int y = rect.height / 2;
				//【8】找出圆心并绘制
				pRadius = Point(rect.x + x, rect.y + y);
				circle(resultImage, pRadius, 2, Scalar(255, 0, 0), 2);
			}
		}
	}
	imshow("result", resultImage);

	cv::waitKey(0);
}


// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门提示: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件
