using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TDG_Vision
{
    internal class ToolShape
    {
        public ToolShape() 
        {
            LINE_TOP = new Line();
            LINE_BOTTOM = new Line();
            LINE_LEFT = new Line();
            LINE_RIGHT = new Line();
            LINE_CENTER_HON = new Line();
            LINE_CENTER_VER = new Line();
            THICKNESS = 4;
            LS = LineSelect.None;
            LINE_TOP.StrokeThickness = THICKNESS;
            LINE_BOTTOM.StrokeThickness = THICKNESS;
            LINE_LEFT.StrokeThickness = THICKNESS;
            LINE_RIGHT.StrokeThickness = THICKNESS;
            LINE_CENTER_HON.StrokeThickness = THICKNESS;
            LINE_CENTER_VER.StrokeThickness = THICKNESS;
            COLOR_DEFAULT = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            COLOR_SELECT = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
            COLOR_CENTER_LINE_DEFAULT = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            COLOR_CENTER_LINE_SELECT = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
            LINE_TOP.Stroke = COLOR_DEFAULT;
            LINE_BOTTOM.Stroke = COLOR_DEFAULT;
            LINE_LEFT.Stroke = COLOR_DEFAULT;
            LINE_RIGHT.Stroke = COLOR_DEFAULT;
            LINE_CENTER_HON.Stroke = COLOR_CENTER_LINE_DEFAULT;
            LINE_CENTER_VER.Stroke = COLOR_CENTER_LINE_DEFAULT;
        }

        public void ShowDefaultRect(Canvas can, double ratio)
        {
            PT_LT.X = can.Width / 4;
            PT_LT.Y = can.Height / 4;
            PT_RB.X = can.Width * 3 / 4;
            PT_RB.Y = can.Height * 3 / 4;
            PT_MARK_CENTER.X = can.Width / 2;
            PT_MARK_CENTER.Y = can.Height / 2;
            PT_LT_SOURCE.X = PT_LT.X * ratio;
            PT_LT_SOURCE.Y = PT_LT.Y * ratio;
            PT_RB_SOURCE.X = PT_RB.X * ratio;
            PT_RB_SOURCE.Y = PT_RB.Y * ratio;
            PT_MARK_CENTER_SOURCE.X = PT_MARK_CENTER.X * ratio;
            PT_MARK_CENTER_SOURCE.Y= PT_MARK_CENTER.Y * ratio;
            LENGTH_HON = can.Width;
            LENGTH_VER = can.Height;
            can.Children.Add(LINE_TOP);
            can.Children.Add(LINE_BOTTOM);
            can.Children.Add(LINE_LEFT);
            can.Children.Add(LINE_RIGHT);
            can.Children.Add(LINE_CENTER_HON);
            can.Children.Add(LINE_CENTER_VER);
            _UpdateLines();
        }

        public void RemoveRect(Canvas can)
        {
            can.Children.Clear();
        }

        /// <summary>
        /// 图像缩放时重新设置Rect的位置
        /// </summary>
        /// <param name="lt_Img">图片缩放时，像框的左上角在原图上的位置</param>
        public void SetRect(Point lt_Img, double ratio)
        {
            PT_LT.X = (PT_LT_SOURCE.X - lt_Img.X) / ratio;
            PT_LT.Y = (PT_LT_SOURCE.Y - lt_Img.Y) / ratio;
            PT_RB.X = (PT_RB_SOURCE.X - lt_Img.X) / ratio;
            PT_RB.Y = (PT_RB_SOURCE.Y - lt_Img.Y) / ratio;
            PT_MARK_CENTER.X = (PT_MARK_CENTER_SOURCE.X - lt_Img.X) / ratio;
            PT_MARK_CENTER.Y = (PT_MARK_CENTER_SOURCE.Y - lt_Img.Y) / ratio;
            _UpdateLines();
        }

        public Int32Rect GetRect()
        {
            Int32Rect rect = new Int32Rect(Convert.ToInt32(PT_LT.X), Convert.ToInt32(PT_LT.Y), Convert.ToInt32(PT_RB.X - PT_LT.X), Convert.ToInt32(PT_RB.Y - PT_LT.Y));
            return rect;
        }

        public Point GetMarkCenter()
        {
            return new Point(PT_MARK_CENTER.X - PT_LT.X, PT_MARK_CENTER.Y - PT_LT.Y);
        }

        public LineSelect PointOnLine(Point pt)
        {
            LineSelect ls = LineSelect.None;
            if (pt.X > PT_LT.X + 5 && pt.X < PT_RB.X - 5 && pt.Y > PT_LT.Y - 2 && pt.Y < PT_LT.Y + 2)
            {
                ls = LineSelect.Top;
            }
            else if (pt.X > PT_LT.X + 5 && pt.X < PT_RB.X - 5 && pt.Y > PT_RB.Y - 2 && pt.Y < PT_RB.Y + 2)
            {
                ls = LineSelect.Bottom;
            }
            else if (pt.X > PT_LT.X - 2 && pt.X < PT_LT.X + 2 && pt.Y > PT_LT.Y + 5 && pt.Y < PT_RB.Y - 5)
            {
                ls = LineSelect.Left;
            }
            else if (pt.X > PT_RB.X - 2 && pt.X < PT_RB.X + 2 && pt.Y > PT_LT.Y + 5 && pt.Y < PT_RB.Y - 5)
            {
                ls = LineSelect.Right;
            }
            else if (pt.X > 5 && pt.X < LENGTH_HON - 5 && pt.Y > PT_MARK_CENTER.Y - 3 && pt.Y < PT_MARK_CENTER.Y + 3)
            {
                ls = LineSelect.Hon;
            }
            else if (pt.Y > 5 && pt.X < LENGTH_VER - 5 && pt.X > PT_MARK_CENTER.X - 3 && pt.X < PT_MARK_CENTER.X + 3)
            {
                ls = LineSelect.Ver;
            }
            return ls;
        }

        public void ChangeSelect(LineSelect ls)
        {
            LS = ls;
            LINE_TOP.Stroke = COLOR_DEFAULT;
            LINE_LEFT.Stroke = COLOR_DEFAULT;
            LINE_RIGHT.Stroke = COLOR_DEFAULT;
            LINE_BOTTOM.Stroke = COLOR_DEFAULT;
            LINE_CENTER_HON.Stroke = COLOR_CENTER_LINE_DEFAULT;
            LINE_CENTER_VER.Stroke = COLOR_CENTER_LINE_DEFAULT;
            switch (LS)
            {
                case LineSelect.Top:
                    LINE_TOP.Stroke = COLOR_SELECT;
                    break;
                case LineSelect.Left:
                    LINE_LEFT.Stroke = COLOR_SELECT;
                    break;
                case LineSelect.Right:
                    LINE_RIGHT.Stroke = COLOR_SELECT;
                    break;
                case LineSelect.Bottom:
                    LINE_BOTTOM.Stroke = COLOR_SELECT;
                    break;
                case LineSelect.Hon:
                    LINE_CENTER_HON.Stroke = COLOR_CENTER_LINE_SELECT;
                    break;
                case LineSelect.Ver:
                    LINE_CENTER_VER.Stroke = COLOR_CENTER_LINE_SELECT;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="can"></param>
        /// <param name="offX">Rect在像框内的X向移动量</param>
        /// <param name="offY">Rect在像框内的Y向移动量</param>
        /// <param name="lt">像框左上角在原图中的坐标位置</param>
        /// <param name="ratio"></param>
        public void UpdateRect(Canvas can, double offX, double offY, Point lt, double ratio)
        {
            switch (LS)
            {
                case LineSelect.None:
                    break;
                case LineSelect.Top:
                case LineSelect.Left:
                    PT_LT.X += offX;
                    PT_LT.Y += offY;
                    PT_RB.X += offX;
                    PT_RB.Y += offY;
                    PT_LT_SOURCE.X += offX * ratio;
                    PT_LT_SOURCE.Y += offY * ratio;
                    PT_RB_SOURCE.X += offX * ratio;
                    PT_RB_SOURCE.Y += offY * ratio;
                    _UpdateLines();
                    break;
                case LineSelect.Right:
                    PT_RB.X += offX;
                    PT_RB_SOURCE.X += offX * ratio;
                    _UpdateLines();
                    break;
                case LineSelect.Bottom:
                    PT_RB.Y += offY;
                    PT_RB_SOURCE.Y += offY * ratio;
                    _UpdateLines();
                    break;
                case LineSelect.Hon:
                    PT_MARK_CENTER.Y += offY;
                    PT_MARK_CENTER_SOURCE.Y += offY * ratio;
                    _UpdateLines();
                    break;
                case LineSelect.Ver:
                    PT_MARK_CENTER.X += offX;
                    PT_MARK_CENTER_SOURCE.X += offX * ratio;
                    _UpdateLines();
                    break;
                default:
                    break;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="img_lt">图像的左上角在原图中的位置</param>
        /// <param name="rect_lt">Mark框的左上角在原图中的位置</param>
        /// <param name="ratio">像框中原图与像框的尺寸比例</param>
        /// <param name="width">Mark框在原图上的宽度</param>
        /// <param name="height">Mark框的原图上的高度</param>
        public void DrawMarkResult(Canvas can, Point img_lt, Point rect_lt, double ratio, double width, double height)
        {
            PT_RESULT_RECT_LT_SOURCE = rect_lt;
            MARK_RESULT_RECT = new Rectangle();
            MARK_RESULT_RECT.Margin = new Thickness((rect_lt.X - img_lt.X) / ratio, (rect_lt.Y - img_lt.Y) / ratio, 0, 0);
            MARK_RESULT_RECT.Width = width / ratio;
            MARK_RESULT_RECT.Height = height / ratio;
            MARK_RESULT_RECT.Stroke = new SolidColorBrush(Colors.Red);
            can.Children.Add(MARK_RESULT_RECT);
        }

        public void ClearMarkResult(Canvas can)
        {
            can.Children.Remove(MARK_RESULT_RECT);
        }

        private void _UpdateLines()
        {
            LINE_TOP.X1 = LINE_BOTTOM.X1 = LINE_LEFT.X1 = LINE_LEFT.X2 = PT_LT.X;
            LINE_TOP.X2 = LINE_BOTTOM.X2 = LINE_RIGHT.X1 = LINE_RIGHT.X2 = PT_RB.X;
            LINE_TOP.Y1 = LINE_TOP.Y2 = LINE_LEFT.Y1 = LINE_RIGHT.Y1 = PT_LT.Y;
            LINE_BOTTOM.Y1 = LINE_BOTTOM.Y2 = LINE_LEFT.Y2 = LINE_RIGHT.Y2 = PT_RB.Y;
            LINE_CENTER_HON.X1 = 0;
            LINE_CENTER_HON.X2 = LENGTH_HON;
            LINE_CENTER_HON.Y1 = LINE_CENTER_HON.Y2 = PT_MARK_CENTER.Y;
            LINE_CENTER_VER.Y1 = 0;
            LINE_CENTER_VER.Y2 = LENGTH_VER;
            LINE_CENTER_VER.X1 = LINE_CENTER_VER.X2 = PT_MARK_CENTER.X;
        }


        private Line LINE_TOP, LINE_LEFT, LINE_BOTTOM, LINE_RIGHT;
        private Line LINE_CENTER_HON, LINE_CENTER_VER;
        private Rectangle MARK_RESULT_RECT;
        private Point PT_LT, PT_RB;                                                                                                                 //Rect的左上角、右下角在像框内的坐标
        private Point PT_LT_SOURCE, PT_RB_SOURCE;                                                                               //Rect的左上角、右下角在原图上的位置
        private Point PT_MARK_CENTER;                                                                                                       //Mark参考点在像框内的坐标
        private Point PT_MARK_CENTER_SOURCE;                                                                                    //Mark参考点在原图上的坐标
        private double LENGTH_HON, LENGTH_VER;                                                                                  //Mark参考点横线、竖线长度
        private double THICKNESS;
        private Point PT_RESULT_SOURCE;                                                                                                     //Mark匹配结果的参考点在原图上的坐标
        private Point PT_RESULT_RECT_LT_SOURCE;                                                                                 //Mark匹配结果框的左上角在原图上的坐标
        private double PT_RESULT_RECT_WIDTH_SOURCE, PT_RESULT_RECT_HEIGHT_SOURCE;   //Mark匹配结果框在原图上的长度和宽度
        private LineSelect LS;
        private Brush COLOR_DEFAULT, COLOR_SELECT;                                                                          //Mark边框的默认颜色与选中颜色
        private Brush COLOR_CENTER_LINE_DEFAULT, COLOR_CENTER_LINE_SELECT;                      //Mark参考点十字线的默认颜色与选中颜色
    }

    public enum LineSelect
    {
        None = 0,
        Top = 1,
        Left = 2,
        Right = 3,
        Bottom = 4,
        Hon = 5,
        Ver = 6
    }
}
