using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Involution
{
    internal class XmlOperator
    {
        public XmlOperator() { }

        public static List<AxisPage> LoadPositionData(out AxisCommon ac)
        {
            List<AxisPage> apList = new List<AxisPage>();
            ac = new AxisCommon();

            XmlDocument xml = new XmlDocument();
            xml.Load(@"D:\Develop\BondingTest\Involution\PositionSample.xml");
            XmlElement baseNode = xml.DocumentElement;
            XmlNode axises = baseNode.SelectSingleNode(AXISES);
            if (axises != null)
            {
                ac.BasicUnit = axises.Attributes[AxisCommon.BasicUnitName].Value;
                ac.BasicMultiple = axises.Attributes[AxisCommon.BasicMultipleName].Value;
                ac.ModelUnit = axises.Attributes[AxisCommon.ModelUnitName].Value;
                ac.ModelMultiple = axises.Attributes[AxisCommon.ModelMultipleName].Value;
                ac.UserUnit = axises.Attributes[AxisCommon.UserUnitName].Value;
                ac.UserMultiple = axises.Attributes[AxisCommon.UserMultipleName].Value;
                ac.SpeedUnit = axises.Attributes[AxisCommon.SpeedUnitName].Value;
                ac.SpeedMultiple = axises.Attributes[AxisCommon.SpeedMultipleName].Value;
                XmlNodeList list = axises.SelectNodes(AXIS);
                for (int i = 0; i < list.Count; i++)
                {
                    AxisPage ap = new AxisPage();
                    ap.AxisCode = list[i].Attributes[CODE].Value;
                    ap.AxisName = list[i].Attributes[NAME].Value;
                    ap.PageCode = list[i].Attributes[PAGE].Value;
                    ap.BasicType = GetIndexByType(list[i].Attributes[BASIC].Value);
                    ap.ModelType = GetIndexByType(list[i].Attributes[MODEL].Value);
                    ap.UserType = GetIndexByType(list[i].Attributes[USER].Value);
                    ap.SpeedType = GetIndexByType(list[i].Attributes[SPEED].Value);

                    XmlNodeList nlist = list[i].SelectNodes(NODE);
                    for (int j = 0; j < nlist.Count; j++)
                    {
                        AxisNode an = new AxisNode();
                        an.Index = j;
                        an.NodeName = nlist[j].Attributes[NAME].Value;
                        ap.NODES.Add(an);
                    }

                    apList.Add(ap);
                }
            }
            return apList;
        }

        

        public static int SavePositionData(List<AxisPage> list)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(@"D:\Develop\BondingTest\Involution\PositionSample.xml");
            XmlNodeList axises = xml.DocumentElement.SelectSingleNode(AXISES).SelectNodes(AXIS);
            if (axises != null)
            {
                for (int i = 0; i < axises.Count; i++)
                {

                }
            }
            return 0;
        }

        public static int GetIndexByType(string type)
        {
            int index = -1;
            if (type == ZF)
                index = 0;
            else if (type == DM)
                index = 1;
            else if (type == EM)
                index = 2;
            return index;
        }

        private static string AXISES = @"Axises";
        private static string AXIS = @"Axis";
        private static string CODE = @"Code";
        private static string NAME = @"Name";
        private static string PAGE = @"Page"; 
        private static string BASIC = @"Basic";
        private static string MODEL = @"Model";
        private static string USER = @"User";
        private static string SPEED = @"Speed";
        private static string NODE = @"Node";
        private static string ZF = @"75";
        private static string DM = @"18";
        private static string EM = @"31";
    }
}
