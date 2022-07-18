using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class AcfTool
    {
        public AcfTool()
        {
            _z = new Axises((int)AcfToolPosition.Count);
        }

        public AcfTool(int toolNum)
        {
            if (toolNum == 1)
            {
                _z = new Axises((int)AcfToolPosition.Count,
                    Mary.BaseAddress(MotorNumber.Acf_Tool1_Z), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Acf_Tool1_Z), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Acf_Tool1_Z), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Acf_Tool1_Z), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            else if (toolNum == 2)
            {
                _z = new Axises((int)AcfToolPosition.Count,
                    Mary.BaseAddress(MotorNumber.Acf_Tool2_Z), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Acf_Tool2_Z), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Acf_Tool2_Z), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Acf_Tool2_Z), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            _toolNum = toolNum;
        }

        public void ReadFromPLC()
        {
            ReadToolZ();
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.Acf_Tool_1:
                case PlcSavingTrigger.Acf_Tool_2:
                    int[] baseArray = new int[_z.baseArray.Length];
                    _z.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_z.baseArray.Length];
                    _z.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_z.baseArray.Length];
                    _z.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_z.baseArray.Length];
                    _z.speedArray.CopyTo(speedArray, 0);
                    ReadToolZ();
                    for (int i = 0; i < _z.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _z.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfToolPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _z.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfToolPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _z.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfToolPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _z.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfToolPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                default:
                    break;
            }
            return posChange;
        }

        private void ReadToolZ()
        {
            int size = Mary._plcUnitReadLength * _z.baseArray.Length;
            short[] data = new short[_z.baseArray.Length * Mary._timesFromShortToInt];

            int ret = Mary.MdReceiveEx((int)_z.baseDeviceType, _z.baseDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _z.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _z.baseArray[i] = BitConverter.ToInt32(bt, 0);
                }

            }

            ret = Mary.MdReceiveEx((int)_z.modelDeviceType, _z.modelDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _z.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _z.modelArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_z.userDeviceType, _z.userDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _z.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _z.userArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_z.speedDeviceType, _z.speedDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _z.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _z.speedArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            string path = Mary._filePath;
            DirectoryInfo di = new DirectoryInfo(path);
            di.Create();
            di.Attributes = FileAttributes.Directory & FileAttributes.Normal;
            FileStream file = File.Open(path + (_toolNum == 1 ? Mary._acfTool1_Z_FileName : Mary._acfTool2_Z_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _z.baseArray.Length; i++)
            {
                content += ((AcfToolPosition)i).ToString() + Mary._csvFileSplit +
                    _z.baseArray[i] + Mary._csvFileSplit +
                    _z.modelArray[i] + Mary._csvFileSplit +
                    _z.userArray[i] + Mary._csvFileSplit +
                    _z.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        Axises _z;
        int _toolNum;
    }
}
