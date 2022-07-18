#include "pch.h"
#include "PBD_Stage.h"

PBD_Stage::PBD_Stage():Model()
{}

int PBD_Stage::SeqShot(int stepCode, Mat* src, int* markWidth, int* markHeight, int* centerX, int* centerY, MESSAGE_SHOW Message)
{
	return COMPLETE;
}

int PBD_Stage::CaculateAlignData(Point2d ptPanel1, Point2d ptPanel2, Point2d ptIC1, Point2d ptIC2, MESSAGE_SHOW Message)
{
	int result = UNKNOW;
	ptPanel1.y = -ptPanel1.y;																									//����Y����ȡ������Ӧ��׼����ϵ
	ptPanel2.y = -ptPanel2.y;
	ptIC1.y = -ptIC1.y;
	ptIC2.y = -ptIC2.y;
	double xita = CAM_RIGHT.GetRecipe().Alpha - CAM_LEFT.GetRecipe().Alpha;		//�����X���������X����ļнǣ�˳ʱ��Ϊ+
	double rotateX_l = CAM_LEFT.GetRecipe().RotateCenter_X;
	double rotateY_l = CAM_LEFT.GetRecipe().RotateCenter_Y;
	double rotateX_R = CAM_RIGHT.GetRecipe().RotateCenter_X;
	double rotateY_R = CAM_RIGHT.GetRecipe().RotateCenter_Y;

	//�������Mark����ֵת�����������
	double rightX = ptPanel2.x;
	double rightY = ptPanel2.y;
	ptPanel2.x = rotateX_l - (rotateX_R - rightX) * cos(xita) + (rotateY_R - rightY) * sin(xita);
	ptPanel2.y = rotateY_l - (rotateY_R - rightY) * cos(xita) - (rotateX_R - rightX) * sin(xita);
	rightX = ptIC2.x;
	rightY = ptIC2.y;
	ptIC2.x = rotateX_l - (rotateX_R - rightX) * cos(xita) + (rotateY_R - rightY) * sin(xita);
	ptIC2.y = rotateY_l - (rotateY_R - rightY) * cos(xita) - (rotateX_R - rightX) * sin(xita);

	double xitaIC;																													//���X��������ʱ�뵽IC���ҷ���ĽǶ�
	if (ptIC1.x == ptIC2.x)
	{
		xitaIC = PI / 2;
	}
	else
	{
		xitaIC = atan((ptIC2.y - ptIC1.y) / (ptIC2.x - ptIC1.x));
	}
	//ICλ�ø���offset����ֵ��x��yƽ�Ƶ�����λ��
	Point2d IcNew1(ptIC1.x + RECIPE.OFFSET_X * cos(xitaIC) - RECIPE.OFFSET_Y * sin(xitaIC), ptIC1.y + RECIPE.OFFSET_X * sin(xitaIC) - RECIPE.OFFSET_Y * cos(xitaIC));
	Point2d IcNew2(ptIC2.x + RECIPE.OFFSET_X * cos(xitaIC) - RECIPE.OFFSET_Y * sin(xitaIC), ptIC2.y + RECIPE.OFFSET_X * sin(xitaIC) - RECIPE.OFFSET_Y * cos(xitaIC));
	//IcFinal[(IcNewx-Ax)*cos(offsetT)-(IcNewy-Ay)*sin(offsetT)+Ax,(IcNewx-Ax)*sin(offsetT)+(IcNewy-Ay) *cos(offsetT)+Ay]
	double offsetT = RECIPE.OFFSET_T * PI / 180;
	//ICλ�ø���offset����ֵ��t��ת������λ��
	Point2d IcFinal1((IcNew1.x - rotateX_l) * cos(offsetT) - (IcNew1.y - rotateY_l) * sin(offsetT) + rotateX_l, (IcNew1.x - rotateX_l) * sin(offsetT) + (IcNew1.y - rotateY_l) * cos(offsetT) + rotateY_l);
	Point2d IcFinal2((IcNew2.x - rotateX_l) * cos(offsetT) - (IcNew2.y - rotateY_l) * sin(offsetT) + rotateX_l, (IcNew2.x - rotateX_l) * sin(offsetT) + (IcNew2.y - rotateY_l) * cos(offsetT) + rotateY_l);
	Point2d IcTarget1, IcTarget2;
	double icPitch = sqrt(pow(IcFinal2.x - IcFinal1.x, 2) + pow(IcFinal2.y - IcFinal1.y, 2));
	double panelPitch;
	if (RECIPE.USE_Y_MAP)
	{

	}
	else
	{
		IcTarget1 = IcFinal1;
		IcTarget2 = IcFinal2;
	}


	return result;
}
