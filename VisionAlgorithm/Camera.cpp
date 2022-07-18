#include "pch.h"
#include "Camera.h"

Camera::Camera()
{
	for (size_t i = 0; i < 3; i++)
	{
		RECIPE.MarkPoint[i].X = 0;
		RECIPE.MarkPoint[i].Y = 0;
	}
}

int Camera::InitParameter(Recipe_Camera& rc, const char* path1, const char* path2, const char* path3, MESSAGE_SHOW Message)
{
	int result = COMPLETE;
	RECIPE = rc;
	Message("start init camera parameter.");
	try
	{
		MARKLIST.clear();
		if (strcmp(path1, "") != 0)
		{
			Mat mark1 = imread(path1, IMREAD_GRAYSCALE);
			MARKLIST.push_back(mark1);
			Message("mark1 init complete.");
		}
		if (strcmp(path2, "") != 0)
		{
			Mat mark2 = imread(path2, IMREAD_GRAYSCALE);
			MARKLIST.push_back(mark2);
			Message("mark2 init complete.");
		}
		if (strcmp(path3, "") != 0)
		{
			Mat mark3 = imread(path3, IMREAD_GRAYSCALE);
			MARKLIST.push_back(mark3);
			Message("mark3 init complete.");
		}
	}
	catch (const std::exception&)
	{
		result = OTHEREXCEPTION;
	}
	
	return result;
}

int Camera::Snap(Mat* src, int stepCode, int* markWidth, int* markHeight, int* centerX, int* centerY, MESSAGE_SHOW Message)
{
	int result = UNKNOW;
	imwrite("D:\\src1.jpg", *src);
	for (size_t i = 0; i < 3; i++)
	{
		if (RECIPE.ENABLE[i] == 1)
		{
			Mat mark = MARKLIST[i];
			if (!mark.empty())
			{
				*markWidth = mark.cols;
				*markHeight = mark.rows;
				result = _MatchMark(src, &mark, centerX, centerY, Message);
				if (result == COMPLETE)
				{
					//*centerX += RECIPE.MarkPoint[i].X;
					//*centerY += RECIPE.MarkPoint[i].Y;
					break;
				}
			}
		}
	}
	
	return result;
}

int Camera::_MatchMark(Mat* src, Mat* mark, int* centerX, int* centerY, MESSAGE_SHOW Message)
{
	int result = UNKNOW;
	clock_t start, markInit, loadSrc, match1, matchfinal;
	start = clock();
	Scalar s(0);
	double sigmaMark = 0;
	for (size_t i = 0; i < mark->cols; i++)
	{
		for (size_t j = 0; j < mark->rows; j++)
		{
			sigmaMark += (double)mark->at<uchar>(j, i) * (double)mark->at<uchar>(j, i);
		}
	}

	int size = 5;		//在Mark中找出size * size的点阵
	int xstep = 7, ystep = 6;	//找点阵的跨步
	Point ptCenter(150, 100);

	std::vector<uchar> tempVec;
	for (size_t i = 0; i < size; i++)
	{
		for (size_t j = 0; j < size; j++)
		{
			tempVec.push_back(mark->at<uchar>(mark->rows * (i + 1) / (size + 1), mark->cols * (j + 1) / (size + 1)));
		}
	}
	markInit = clock();
	std::string msg = "Mark init time cost: " + std::to_string((double)(markInit - start) / CLOCKS_PER_SEC) + " s.";
	Message(msg.c_str());

	Mat Pic = src->clone();
	//cvtColor(*src, Pic, COLOR_BGR2GRAY);
	loadSrc = clock();
	msg = "load src picture time cost: " + std::to_string((double)(loadSrc - markInit) / CLOCKS_PER_SEC) + " s.";
	Message(msg.c_str());

	int score = 50000;
	size_t minIndexi = 0, minIndexj = 0;

	for (size_t i = 0; i < Pic.cols - mark->cols; i += xstep)
	{
		for (size_t j = 0; j < Pic.rows - mark->rows; j += ystep)
		{
			Rect rect(i, j, mark->cols, mark->rows);
			Mat sub(Pic, rect);
			std::vector<uchar> tp;
			for (size_t n = 0; n < size; n++)
			{
				for (size_t m = 0; m < size; m++)
				{
					tp.push_back(sub.at<uchar>(sub.rows * (n + 1) / (size + 1), sub.cols * (m + 1) / (size + 1)));
				}
			}
			//Mat mtp(size, size, CV_8UC1, tp.data());
			int s1 = 0;
			for (size_t k = 0; k < tempVec.size(); k++)
			{
				s1 += abs(tempVec[k] - tp[k]);
			}
			if (s1 < score)
			{
				score = s1;
				minIndexi = i;
				minIndexj = j;
			}
		}
	}

	match1 = clock();
	msg = "mark match more or less time cost: " + std::to_string((double)(match1 - loadSrc) / CLOCKS_PER_SEC) + " s.";
	Message(msg.c_str());

	Rect finalRect(minIndexi - xstep, minIndexj - ystep, 2 * xstep + mark->cols, 2 * ystep + mark->rows);
	Mat finalSub(Pic, finalRect);

	int markIndex_x = 0, markIndex_y = 0;
	size_t sigma_x = 0, sigma_y = 0;
	double sigma_Final = 0;
	for (size_t i = 0; i < finalSub.cols - mark->cols; i++)
	{
		for (size_t j = 0; j < finalSub.rows - mark->rows; j++)
		{
			Rect rect(i, j, mark->cols, mark->rows);
			Mat sample(finalSub, rect);
			Mat markmaybe(mark->rows, mark->cols, CV_16SC1, s);
			double sigmaSrc = 0, sigmaSt = 0;
			for (size_t i1 = 0; i1 < mark->cols; i1 += 1)
			{
				for (size_t j1 = 0; j1 < mark->rows; j1 += 1)
				{
					if (sample.at<uchar>(j1, i1) > 100 && sample.at<uchar>(j1, i1) < 160)
					{
						markmaybe.at<short>(j1, i1) = 255;
					}
					uchar spt = finalSub.at<uchar>(j + j1, i + i1);
					uchar tpt = mark->at<uchar>(j1, i1);
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
	msg = "max sigma R:" + std::to_string(sigma_Final);
	Message(msg.c_str());
	matchfinal = clock();
	msg = "final mark match time cost: " + std::to_string((double)(matchfinal - match1) / CLOCKS_PER_SEC) + " s.";
	Message(msg.c_str());
	
	*centerX = finalRect.x + sigma_x;
	*centerY = finalRect.y + sigma_y;
	char chx[16], chy[16];
	_itoa_s(*centerX, chx, 10);
	_itoa_s(*centerY, chy, 10);
	Message(chx);
	Message(chy);
	result = COMPLETE;

	return result;
}

Recipe_Camera Camera::GetRecipe()
{
	return RECIPE;
}