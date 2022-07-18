using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class Axises
    {
        public Axises(){ }

        public Axises(int count, int baseNum, PlcDeviceType baseType, int modelNum, PlcDeviceType modelType, int userNum, 
            PlcDeviceType userType, int seppdNum, PlcDeviceType speedType, int alignSpeedNum, PlcDeviceType alignSpeedType)
        {
            baseArray = new int[count];
            modelArray = new int[count];
            userArray = new int[count];
            speedArray = new int[count];
            alignSpeedArray = new int[count];

            baseDeviceNumber = baseNum;
            modelDeviceNumber = modelNum;
            userDeviceNumber = userNum;
            speedDeviceNumber = seppdNum;
            alignSpeedDeviceNumber = alignSpeedNum;

            baseDeviceType = baseType;
            modelDeviceType = modelType;
            userDeviceType = userType;
            speedDeviceType = speedType;
            alignSpeedDeviceType = alignSpeedType;
    }

        public Axises(int count)
        {
            baseArray = new int[count];
            modelArray = new int[count];
            userArray = new int[count];
            speedArray = new int[count];
            alignSpeedArray = new int[count];
        }

        //??
        public string CompareWithPLC(PlcSavingTrigger trig, PrebondTool_X_Position pos, int[] bArray, int[] mArray, int[] uArray, int[] sArray)
        {
            string posChange = string.Empty;
            for (int i = 0; i < baseArray.Length; i++)
            {
                if (bArray[i] != baseArray[i])
                {
                    posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                        trig.ToString() + Mary._csvFileSplit +
                        ((PrebondTool_X_Position)i).ToString() + Mary._csvFileSplit +
                        PositionCategory.Base.ToString() + Mary._csvFileSplit +
                        bArray[i].ToString() + Mary._csvFileSplit +
                        baseArray[i].ToString() + Environment.NewLine;
                }
                if (mArray[i] != modelArray[i])
                {
                    posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                        trig.ToString() + Mary._csvFileSplit +
                        ((PrebondTool_X_Position)i).ToString() + Mary._csvFileSplit +
                        PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                        mArray[i].ToString() + Mary._csvFileSplit +
                        modelArray[i].ToString() + Environment.NewLine;
                }
                if (uArray[i] != userArray[i])
                {
                    posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                        trig.ToString() + Mary._csvFileSplit +
                        ((PrebondTool_X_Position)i).ToString() + Mary._csvFileSplit +
                        PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                        uArray[i].ToString() + Mary._csvFileSplit +
                        userArray[i].ToString() + Environment.NewLine;
                }
                if (sArray[i] != speedArray[i])
                {
                    posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                        trig.ToString() + Mary._csvFileSplit +
                        ((PrebondTool_X_Position)i).ToString() + Mary._csvFileSplit +
                        PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                        sArray[i].ToString() + Mary._csvFileSplit +
                        speedArray[i].ToString() + Environment.NewLine;
                }
            }
            return posChange;
        }

        public int[] baseArray;
        public int[] modelArray;
        public int[] userArray;
        public int[] speedArray;
        public int[] alignSpeedArray;

        public int baseDeviceNumber;
        public int modelDeviceNumber;
        public int userDeviceNumber;
        public int speedDeviceNumber;
        public int alignSpeedDeviceNumber;

        public PlcDeviceType baseDeviceType;
        public PlcDeviceType modelDeviceType;
        public PlcDeviceType userDeviceType;
        public PlcDeviceType speedDeviceType;
        public PlcDeviceType alignSpeedDeviceType;
    }
}
