using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AgvDispatchor
{
    internal class MessageManager
    {
        public MessageManager(TextBlock tb, ScrollViewer sv, int slines, int tlines)
        {
            TEXT_AREA = tb;
            SLIDER = sv;
            TEXT_ARRAY = new Queue<Run>(0);
            SHOW_LINES = slines;
            TOTAL_LINES = tlines;
            SLIDE_PARAM = SLIDER.ScrollableHeight / (TOTAL_LINES - SHOW_LINES);
            BRUSH_TYPE = new Dictionary<MessageType, Brush>()
            {
                { MessageType.Default, Brushes.Black },
                { MessageType.Result, Brushes.Blue },
                { MessageType.Important, Brushes.Chocolate },
                { MessageType.Error, Brushes.DarkRed },
                { MessageType.Exception, Brushes.Orange },
            };
        }

        public void AddText(string mes, MessageType mt)
        {
            Run r = new Run(mes + Environment.NewLine);
            r.Foreground = BRUSH_TYPE[mt];
            TEXT_ARRAY.Enqueue(r);
            lock (TEXT_ARRAY)
            {
                while (TEXT_ARRAY.Count > TOTAL_LINES)
                {
                    TEXT_ARRAY.Dequeue();
                }
            }
            TEXT_AREA.Inlines.Clear();
            foreach (Run r0 in TEXT_ARRAY)
            {
                TEXT_AREA.Inlines.Add(r0);
            }
            SLIDER.ScrollToVerticalOffset((TEXT_ARRAY.Count - SHOW_LINES) * SLIDE_PARAM);
        }

        private TextBlock TEXT_AREA;
        private ScrollViewer SLIDER;
        private Queue<Run> TEXT_ARRAY;                      //存储用于显示的各行字符串
        private int SHOW_LINES;                                         //TextBlock可见部分的行数
        private int TOTAL_LINES;                                        //TextBlock的总行数
        private double SLIDE_PARAM;                               //每行文字所占的滚动高度
        private Dictionary<MessageType, Brush> BRUSH_TYPE;
    }

    public enum MessageType
    {
        Default = 0,
        Result = 1,
        Important = 2,
        Error = 3,
        Exception = 4,
    }
}
