using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Involution
{
    internal class AxisPage
    {
        public AxisPage()
        {
            NODES = new List<AxisNode>();
        }

        public AxisPage(int value) 
        {
            if (value == 0)
            {
                Code = string.Empty;
                AxisCode = string.Empty;
                AxisName = string.Empty;
                PageCode = string.Empty;
                BasicType = -1;
                ModelType = -1;
                UserType = -1;
                SpeedType = -1;
                NODES = new List<AxisNode>();
                for (int i = 0; i < 20; i++)
                {
                    AxisNode an = new AxisNode();
                    an.Index = i;
                    an.NodeName = string.Empty;
                    NODES.Add(an);
                }
            }
        }

        public string Code { get; set; }
        public string AxisCode {get; set;}
        public string AxisName { get; set; }
        public string PageCode { get; set; }
        public int BasicType { get; set; }
        public int ModelType { get; set; }
        public int UserType { get; set; }
        public int SpeedType { get; set; }

        public List<AxisNode> NODES;
    }
}
