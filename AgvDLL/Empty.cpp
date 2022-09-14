#include "pch.h"
#include "Empty.h"

Empty::Empty()
{

}

bool Empty::FindEmpty(Mat* src, int* x, int* y, int* r)
{
	bool result = false;

	// smooth it, otherwise a lot of false circles may be detected
	Mat gray;
	GaussianBlur(*src, gray, Size(7, 7), 2, 2);
	std::vector<Vec3f> circles;
	HoughCircles(gray, circles, HOUGH_GRADIENT, 2, gray.rows / 4, 200, 100);
	for (size_t i = 0; i < circles.size(); i++)
	{
		Point center(cvRound(circles[i][0]), cvRound(circles[i][1]));
		*x = cvRound(circles[i][0]);
		*y = cvRound(circles[i][1]);
		*r = cvRound(circles[i][2]);
		result = true;
		break;
		// draw the circle center
		//circle(*pic, center, 3, Scalar(0, 255, 0), -1, 8, 0);
		// draw the circle outline
		//circle(*pic, center, radius, Scalar(0, 0, 255), 3, 8, 0);
	}

	return result;
}