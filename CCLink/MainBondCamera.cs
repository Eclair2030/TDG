using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class MainBondCamera
    {
        public MainBondCamera() { }
        
        public MainBondCamera(int cameraNum)
        {
            _x = new Axises((int)MainbondCameraPosition.Count,
                6360, PlcDeviceType.Base,
                36300, PlcDeviceType.ModelOffset,
                82480, PlcDeviceType.ModelOffset,
                6320, PlcDeviceType.Base,
                0, PlcDeviceType.Base);
        }

        public void ReadFromPLC()
        {
            ReadCameraX();
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.Fnb_Camera_Default:
                    int[] baseArray = new int[_x.baseArray.Length];
                    _x.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_x.baseArray.Length];
                    _x.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_x.baseArray.Length];
                    _x.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_x.baseArray.Length];
                    _x.speedArray.CopyTo(speedArray, 0);
                    ReadCameraX();
                    for (int i = 0; i < _x.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _x.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondCameraPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _x.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondCameraPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _x.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondCameraPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _x.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondCameraPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                default:
                    break;
            }
            return posChange;
        }

        private void ReadCameraX()
        {
            int size = Mary._plcUnitReadLength * _x.baseArray.Length;
            short[] data = new short[_x.baseArray.Length * Mary._timesFromShortToInt];

            int ret = Mary.MdReceiveEx((int)_x.baseDeviceType, _x.baseDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _x.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _x.baseArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_x.modelDeviceType, _x.modelDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _x.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _x.modelArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_x.userDeviceType, _x.userDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _x.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _x.userArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_x.speedDeviceType, _x.speedDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _x.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _x.speedArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            string path = Mary._filePath;
            DirectoryInfo di = new DirectoryInfo(path);
            di.Create();
            di.Attributes = FileAttributes.Directory & FileAttributes.Normal;
            string fileName = string.Empty;
            FileStream file = File.Open(path + Mary._mainbondCamera_X_FileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _x.baseArray.Length; i++)
            {
                content += ((MainbondCameraPosition)i).ToString() + Mary._csvFileSplit +
                    _x.baseArray[i] + Mary._csvFileSplit +
                    _x.modelArray[i] + Mary._csvFileSplit +
                    _x.userArray[i] + Mary._csvFileSplit +
                    _x.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        Axises _x;
    }
}
