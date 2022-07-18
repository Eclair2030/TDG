using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDG_Vision
{
    internal class RecipeModel
    {
        public RecipeModel()
		{
			L_CAMERA_RECIPE = new RecipeCamera();
			R_CAMERA_RECIPE = new RecipeCamera();
			MODEL_CODE = "0";
		}

		public string MODEL_CODE { get; set; }			//此工位的Recipe编号
		public double DELAY;											//拍照延迟时间(s)
		public double OFFSET_X;									//补偿值(mm)
		public double OFFSET_Y;
		public double OFFSET_T;
		public double SPEC_X;										//精度(mm)
		public double SPEC_Y;
		public double SPEC_T;
		public double CALIB_X;										//标定时的移动距离
		public double CALIB_Y;
		public double CALIB_T;
		public double Y_MAP;                                        //Y Map的角度(斜电极与水平线夹角)
		public bool USE_Y_MAP { get; set; }					//是否启用Y MAP

		public RecipeCamera L_CAMERA_RECIPE;
		public RecipeCamera R_CAMERA_RECIPE;
	}
}
