using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TDG_Vision
{
    internal class RecipeCamera
    {
        public RecipeCamera() 
		{
			MARK_ENABLE_LIST = new int[3] {0, 0, 0};
			MARK_CEN = new MarkCenter[3] { new MarkCenter(), new MarkCenter(), new MarkCenter()};
		}

		//public int CAMERA_CODE { get; set; }			//此相机的Recipe编号

		//public List<string> MARK_LIST;                      //Mark图片保存路径列表(弃用，改为固定路径)

		public int[] MARK_ENABLE_LIST;                  //Mark禁用列表(0：禁用，1：启用)
		public double RotateCenter_X;					//标定数据
		public double RotateCenter_Y;
		public double Resolution_X;
		public double Resolution_Y;
		public MarkCenter[] MARK_CEN;								//Mark参考点
	}

	public struct MarkCenter
    {
		public double X;
		public double Y;
    }
}
