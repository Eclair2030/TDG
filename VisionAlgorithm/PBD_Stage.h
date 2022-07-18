#pragma once
#include "Model.h"
class PBD_Stage :
    public Model
{
public:
    PBD_Stage();
    int SeqShot(int stepCode, Mat* src, int* markWidth, int* markHeight, int* centerX, int* centerY, MESSAGE_SHOW Message);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ptPanel1">Panel左Mark在左相机坐标系的坐标</param>
    /// <param name="ptPanel2">Panel右Mark在右相机坐标系的坐标</param>
    /// <param name="ptIC1">IC左Mark在左相机坐标系的坐标</param>
    /// <param name="ptIC2">IC右Mark在右相机坐标系的坐标</param>
    /// <param name="Message"></param>
    /// <returns></returns>
    int CaculateAlignData(Point2d ptPanel1, Point2d ptPanel2, Point2d ptIC1, Point2d ptIC2, MESSAGE_SHOW Message);
};
