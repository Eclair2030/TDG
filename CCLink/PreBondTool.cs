using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class PreBondTool
    {
        public PreBondTool()
        {
            _x = new Axises((int)PrebondTool_X_Position.Count);
            _z = new Axises((int)PrebondTool_Z_Position.Count);
            _t = new Axises((int)PrebondTool_T_Position.Count);
        }

        public PreBondTool(int toolNum)
        {
            if (toolNum == 1)
            {
                _x = new Axises((int)PrebondTool_X_Position.Count,
                    Mary.BaseAddress(MotorNumber.Prb_Tool1_X), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Prb_Tool1_X), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Prb_Tool1_X), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Prb_Tool1_X), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _z = new Axises((int)PrebondTool_Z_Position.Count,
                    Mary.BaseAddress(MotorNumber.Prb_Tool1_Z), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Prb_Tool1_Z), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Prb_Tool1_Z), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Prb_Tool1_Z), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t = new Axises((int)PrebondTool_T_Position.Count,
                    Mary.BaseAddress(MotorNumber.Prb_Tool1_T), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Prb_Tool1_T), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Prb_Tool1_T), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Prb_Tool1_T), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            else if (toolNum == 2)
            {
                _x = new Axises((int)PrebondTool_X_Position.Count,
                    Mary.BaseAddress(MotorNumber.Prb_Tool2_X), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Prb_Tool2_X), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Prb_Tool2_X), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Prb_Tool2_X), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _z = new Axises((int)PrebondTool_Z_Position.Count,
                    Mary.BaseAddress(MotorNumber.Prb_Tool2_Z), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Prb_Tool2_Z), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Prb_Tool2_Z), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Prb_Tool2_Z), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t = new Axises((int)PrebondTool_T_Position.Count,
                    Mary.BaseAddress(MotorNumber.Prb_Tool2_T), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Prb_Tool2_T), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Prb_Tool2_T), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Prb_Tool2_T), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            _toolNum = toolNum;
        }

        public void ReadFromPLC()
        {
            ReadToolX();
            ReadToolZ();
            ReadToolT();
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.Prb_Tool1_X:
                case PlcSavingTrigger.Prb_Tool2_X:
                    int[] baseArray = new int[_x.baseArray.Length];
                    _x.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_x.baseArray.Length];
                    _x.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_x.baseArray.Length];
                    _x.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_x.baseArray.Length];
                    _x.speedArray.CopyTo(speedArray, 0);
                    ReadToolX();
                    for (int i = 0; i < _x.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _x.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_X_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _x.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_X_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _x.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_X_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _x.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_X_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Prb_Tool1_Z:
                case PlcSavingTrigger.Prb_Tool2_Z:
                    int[] baseArray_z = new int[_z.baseArray.Length];
                    _z.baseArray.CopyTo(baseArray_z, 0);
                    int[] modelArray_z = new int[_z.baseArray.Length];
                    _z.modelArray.CopyTo(modelArray_z, 0);
                    int[] userArray_z = new int[_z.baseArray.Length];
                    _z.userArray.CopyTo(userArray_z, 0);
                    int[] speedArray_z = new int[_z.baseArray.Length];
                    _z.speedArray.CopyTo(speedArray_z, 0);
                    ReadToolZ();
                    for (int i = 0; i < _z.baseArray.Length; i++)
                    {
                        if (baseArray_z[i] != _z.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_Z_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_z[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_z[i] != _z.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_Z_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_z[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_z[i] != _z.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_Z_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_z[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_z[i] != _z.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_Z_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_z[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Prb_Tool1_T:
                case PlcSavingTrigger.Prb_Tool2_T:
                    int[] baseArray_t = new int[_t.baseArray.Length];
                    _t.baseArray.CopyTo(baseArray_t, 0);
                    int[] modelArray_t = new int[_t.baseArray.Length];
                    _t.modelArray.CopyTo(modelArray_t, 0);
                    int[] userArray_t = new int[_t.baseArray.Length];
                    _t.userArray.CopyTo(userArray_t, 0);
                    int[] speedArray_t = new int[_t.baseArray.Length];
                    _t.speedArray.CopyTo(speedArray_t, 0);
                    ReadToolT();
                    for (int i = 0; i < _t.baseArray.Length; i++)
                    {
                        if (baseArray_t[i] != _t.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_t[i] != _t.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_t[i] != _t.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_t[i] != _t.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondTool_T_Position)i).ToString() + Mary._csvFileSplit +
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

        private void ReadToolX()
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
            FileStream file = File.Open(path + (_toolNum == 1 ? Mary._prebondTool1_X_FileName : Mary._prebondTool2_X_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _x.baseArray.Length; i++)
            {
                content += ((PrebondStagePosition)i).ToString() + Mary._csvFileSplit +
                    _x.baseArray[i] + Mary._csvFileSplit +
                    _x.modelArray[i] + Mary._csvFileSplit +
                    _x.userArray[i] + Mary._csvFileSplit +
                    _x.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
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
            FileStream file = File.Open(path + (_toolNum == 1 ? Mary._prebondTool1_Z_FileName : Mary._prebondTool2_Z_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _z.baseArray.Length; i++)
            {
                content += ((PrebondStagePosition)i).ToString() + Mary._csvFileSplit +
                    _z.baseArray[i] + Mary._csvFileSplit +
                    _z.modelArray[i] + Mary._csvFileSplit +
                    _z.userArray[i] + Mary._csvFileSplit +
                    _z.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        private void ReadToolT()
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
            FileStream file = File.Open(path + (_toolNum == 1 ? Mary._prebondTool1_T_FileName : Mary._prebondTool2_T_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _t.baseArray.Length; i++)
            {
                content += ((PrebondStagePosition)i).ToString() + Mary._csvFileSplit +
                    _t.baseArray[i] + Mary._csvFileSplit +
                    _t.modelArray[i] + Mary._csvFileSplit +
                    _t.userArray[i] + Mary._csvFileSplit +
                    _t.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        Axises _x;
        Axises _z;
        Axises _t;
        int _toolNum;
    }
}
