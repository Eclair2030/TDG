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
    /// <param name="ptPanel1">Panel��Mark�����������ϵ������</param>
    /// <param name="ptPanel2">Panel��Mark�����������ϵ������</param>
    /// <param name="ptIC1">IC��Mark�����������ϵ������</param>
    /// <param name="ptIC2">IC��Mark�����������ϵ������</param>
    /// <param name="Message"></param>
    /// <returns></returns>
    int CaculateAlignData(Point2d ptPanel1, Point2d ptPanel2, Point2d ptIC1, Point2d ptIC2, MESSAGE_SHOW Message);
};
