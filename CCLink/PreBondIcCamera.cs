using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class PreBondIcCamera
    {
        public PreBondIcCamera()
        {
            _y = new Axises((int)PrebondIcCamera_Y_Position.Count);
            _t = new Axises((int)PrebondIcCamera_T_Position.Count);
        }

        public PreBondIcCamera(int cameraNum)
        {
            _y = new Axises((int)PrebondIcCamera_Y_Position.Count,
                8160, PlcDeviceType.Base,
                38100, PlcDeviceType.ModelOffset,
                83200, PlcDeviceType.ModelOffset,
                8120, PlcDeviceType.Base,
                0, PlcDeviceType.Base);
            _t = new Axises((int)PrebondIcCamera_T_Position.Count,
                8260, PlcDeviceType.Base,
                38200, PlcDeviceType.ModelOffset,
                83240, PlcDeviceType.ModelOffset,
                8220, PlcDeviceType.Base,
                0, PlcDeviceType.Base);
        }

        public void ReadFromPLC()
        {
            ReadCameraY();
            ReadCameraT();
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.IC_Camera_Y:
                    int[] baseArray = new int[_y.baseArray.Length];
                    _y.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_y.baseArray.Length];
                    _y.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_y.baseArray.Length];
                    _y.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_y.baseArray.Length];
                    _y.speedArray.CopyTo(speedArray, 0);
                    ReadCameraY();
                    for (int i = 0; i < _y.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _y.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcCamera_Y_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _y.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcCamera_Y_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _y.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcCamera_Y_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _y.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcCamera_Y_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.IC_Camera_T:
                    int[] baseArray_t = new int[_t.baseArray.Length];
                    _t.baseArray.CopyTo(baseArray_t, 0);
                    int[] modelArray_t = new int[_t.baseArray.Length];
                    _t.modelArray.CopyTo(modelArray_t, 0);
                    int[] userArray_t = new int[_t.baseArray.Length];
                    _t.userArray.CopyTo(userArray_t, 0);
                    int[] speedArray_t = new int[_t.baseArray.Length];
                    _t.speedArray.CopyTo(speedArray_t, 0);
                    ReadCameraT();
                    for (int i = 0; i < _t.baseArray.Length; i++)
                    {
                        if (baseArray_t[i] != _t.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcCamera_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_t[i] != _t.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcCamera_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_t[i] != _t.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcCamera_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_t[i] != _t.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcCamera_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                default:
                    break;
            }
            return posChange;
        }

        private void ReadCameraY()
        {
            int size = Mary._plcUnitReadLength * _y.baseArray.Length;
            short[] data = new short[_y.baseArray.Length * Mary._timesFromShortToInt];

            int ret = Mary.MdReceiveEx((int)_y.baseDeviceType, _y.baseDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _y.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _y.baseArray[i] = BitConverter.ToInt32(bt, 0);
                }

            }

            ret = Mary.MdReceiveEx((int)_y.modelDeviceType, _y.modelDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _y.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _y.modelArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_y.userDeviceType, _y.userDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _y.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _y.userArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_y.speedDeviceType, _y.speedDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _y.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _y.speedArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            string path = Mary._filePath;
            DirectoryInfo di = new DirectoryInfo(path);
            di.Create();
            di.Attributes = FileAttributes.Directory & FileAttributes.Normal;
            FileStream file = File.Open(path + Mary._prebondIcCamera_Y_FileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _y.baseArray.Length; i++)
            {
                content += ((PrebondIcCamera_Y_Position)i).ToString() + Mary._csvFileSplit +
                    _y.baseArray[i] + Mary._csvFileSplit +
                    _y.modelArray[i] + Mary._csvFileSplit +
                    _y.userArray[i] + Mary._csvFileSplit +
                    _y.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        private void ReadCameraT()
        {
            int size = Mary._plcUnitReadLength * _t.baseArray.Length;
            short[] data = new short[_t.baseArray.Length * Mary._timesFromShortToInt];

            int ret = Mary.MdReceiveEx((int)_t.baseDeviceType, _t.baseDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t.baseArray[i] = BitConverter.ToInt32(bt, 0);
                }

            }

            ret = Mary.MdReceiveEx((int)_t.modelDeviceType, _t.modelDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t.modelArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_t.userDeviceType, _t.userDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t.userArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_t.speedDeviceType, _t.speedDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t.speedArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            string path = Mary._filePath;
            DirectoryInfo di = new DirectoryInfo(path);
            di.Create();
            di.Attributes = FileAttributes.Directory & FileAttributes.Normal;
            FileStream file = File.Open(path + Mary._prebondIcCamera_T_FileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _t.baseArray.Length; i++)
            {
                content += ((PrebondIcCamera_T_Position)i).ToString() + Mary._csvFileSplit +
                    _t.baseArray[i] + Mary._csvFileSplit +
                    _t.modelArray[i] + Mary._csvFileSplit +
                    _t.userArray[i] + Mary._csvFileSplit +
                    _t.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        Axises _y;
        Axises _t;
    }
}
