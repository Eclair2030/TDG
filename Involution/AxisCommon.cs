using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Involution
{
    internal class AxisCommon
    {
        public AxisCommon() { }

        public string BasicUnit { get; set; }
        public static string BasicUnitName = @"BasicUnit";
        public string BasicMultiple { get; set; }
        public static string BasicMultipleName = @"BasicMultiple";
        public string ModelUnit { get; set; }
        public static string ModelUnitName = @"ModelUnit";
        public string ModelMultiple { get; set; }
        public static string ModelMultipleName = @"ModelMultiple";
        public string UserUnit { get; set; }
        public static string UserUnitName = @"UserUnit";
        public string UserMultiple { get; set; }
        public static string UserMultipleName = @"UserMultiple";
        public string SpeedUnit { get; set; }
        public static string SpeedUnitName = @"SpeedUnit";
        public string SpeedMultiple { get; set; }
        public static string SpeedMultipleName = @"SpeedMultiple";

        public static int GetIndexByMultipleValue(string value)
        {
            int index = -1;
            if (value == "1")
                index = 0;
            else if (value == "0.1")
                index = 1;
            else if (value == "0.01")
                index = 2;
            else if (value == "0.001")
                index = 3;
            else if (value == "0.0001")
                index = 4;
            return index;
        }
    }
}
