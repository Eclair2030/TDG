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
using namespace cv::ml;
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
	Mat mark = imread("D:\\OLED\\AGV\\Big stone test\\mark.bmp", IMREAD_GRAYSCALE);
	double sigmaMark = 0;
	for (size_t i = 0; i < mark.cols; i++)
	{
		for (size_t j = 0; j < mark.rows; j++)
		{
			sigmaMark += (double)mark.at<uchar>(j, i) * (double)mark.at<uchar>(j, i);
		}
	}

	Mat Pic = imread("D:\\OLED\\AGV\\Big stone test\\middle.jpg", IMREAD_GRAYSCALE);
	Mat res;
	matchTemplate(Pic, mark, res, TM_CCOEFF);
	double min, max;
	Point maxP;
	minMaxLoc(res, &min, &max, 0, &maxP);
	std::cout << "min: " << min << " , max: " << max << std::endl;
	std::cout << "max point x: " << maxP.x << " , y: " << maxP.y << std::endl;
	rectangle(Pic, Rect(maxP.x, maxP.y, mark.cols, mark.rows), Scalar(0, 0, 255));
	imshow("A", Pic);
	//minMaxLoc(Pic, )
	/*size_t sigma_x = 0, sigma_y = 0;
	double sigma_Final = 0;
	for (size_t i = 0; i < Pic.cols - mark.cols; i++)
	{
		for (size_t j = 0; j < Pic.rows - mark.rows; j++)
		{
			Rect rect(i, j, mark.cols, mark.rows);
			Mat sample(Pic, rect);
			Mat markmaybe(mark.rows, mark.cols, CV_16SC1, Scalar(0));
			double sigmaSrc = 0, sigmaSt = 0;
			for (size_t i1 = 0; i1 < mark.cols; i1 += 1)
			{
				for (size_t j1 = 0; j1 < mark.rows; j1 += 1)
				{
					if (sample.at<uchar>(j1, i1) > 100 && sample.at<uchar>(j1, i1) < 160)
					{
						markmaybe.at<short>(j1, i1) = 255;
					}
					uchar spt = Pic.at<uchar>(j + j1, i + i1);
					uchar tpt = mark.at<uchar>(j1, i1);
					sigmaSrc += (double)spt * (double)spt;
					sigmaSt += (double)spt * (double)tpt;
				}
			}

			double R = sigmaSt / (sqrt(sigmaSrc) * sqrt(sigmaMark));
			if (R > sigma_Final)
			{
				sigma_Final = R;
				sigma_x = i;
				sigma_y = j;
			}
		}
	}

	rectangle(Pic, Rect(sigma_x, sigma_y, mark.cols, mark.rows), Scalar(0, 0, 255));
	imshow("A", Pic);*/

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
