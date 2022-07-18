using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class AcfStage
    {
        public AcfStage()
        {
            _x = new Axises((int)AcfStagePosition.Count);
            _y = new Axises((int)AcfStagePosition.Count);
            _t = new Axises((int)AcfStagePosition.Count);
        }

        public AcfStage(int stageNum)
        {
            if (stageNum == 1)
            {
                _x = new Axises((int)AcfStagePosition.Count,
                    Mary.BaseAddress(MotorNumber.Acf_Stage1_X), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Acf_Stage1_X), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Acf_Stage1_X), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Acf_Stage1_X), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _y = new Axises((int)AcfStagePosition.Count,
                    Mary.BaseAddress(MotorNumber.Acf_Stage1_Y), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Acf_Stage1_Y), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Acf_Stage1_Y), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Acf_Stage1_Y), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t = new Axises((int)AcfStagePosition.Count,
                    Mary.BaseAddress(MotorNumber.Acf_Stage1_T), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Acf_Stage1_T), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Acf_Stage1_T), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Acf_Stage1_T), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            else if (stageNum == 2)
            {
                _x = new Axises((int)AcfStagePosition.Count,
                    Mary.BaseAddress(MotorNumber.Acf_Stage2_X), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Acf_Stage2_X), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Acf_Stage2_X), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Acf_Stage2_X), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _y = new Axises((int)AcfStagePosition.Count,
                    Mary.BaseAddress(MotorNumber.Acf_Stage2_Y), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Acf_Stage2_Y), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Acf_Stage2_Y), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Acf_Stage2_Y), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t = new Axises((int)AcfStagePosition.Count,
                    Mary.BaseAddress(MotorNumber.Acf_Stage2_T), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Acf_Stage2_T), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Acf_Stage2_T), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Acf_Stage2_T), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            _stageNum = stageNum;
        }

        public void ReadFromPLC()
        {
            ReadStageX();
            ReadStageY();
            ReadStageT();
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.Acf_Stage1_X:
                case PlcSavingTrigger.Acf_Stage2_X:
                    int[] baseArray = new int[_x.baseArray.Length];
                    _x.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_x.baseArray.Length];
                    _x.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_x.baseArray.Length];
                    _x.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_x.baseArray.Length];
                    _x.speedArray.CopyTo(speedArray, 0);
                    ReadStageX();
                    for (int i = 0; i < _x.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _x.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _x.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _x.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _x.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Acf_Stage1_Y:
                case PlcSavingTrigger.Acf_Stage2_Y:
                    int[] baseArray_y = new int[_y.baseArray.Length];
                    _y.baseArray.CopyTo(baseArray_y, 0);
                    int[] modelArray_y = new int[_y.baseArray.Length];
                    _y.modelArray.CopyTo(modelArray_y, 0);
                    int[] userArray_y = new int[_y.baseArray.Length];
                    _y.userArray.CopyTo(userArray_y, 0);
                    int[] speedArray_y = new int[_y.baseArray.Length];
                    _y.speedArray.CopyTo(speedArray_y, 0);
                    ReadStageY();
                    for (int i = 0; i < _y.baseArray.Length; i++)
                    {
                        if (baseArray_y[i] != _y.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_y[i] != _y.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_y[i] != _y.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_y[i] != _y.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Acf_Stage1_T:
                case PlcSavingTrigger.Acf_Stage2_T:
                    int[] baseArray_t = new int[_t.baseArray.Length];
                    _t.baseArray.CopyTo(baseArray_t, 0);
                    int[] modelArray_t = new int[_t.baseArray.Length];
                    _t.modelArray.CopyTo(modelArray_t, 0);
                    int[] userArray_t = new int[_t.baseArray.Length];
                    _t.userArray.CopyTo(userArray_t, 0);
                    int[] speedArray_t = new int[_t.baseArray.Length];
                    _t.speedArray.CopyTo(speedArray_t, 0);
                    ReadStageT();
                    for (int i = 0; i < _t.baseArray.Length; i++)
                    {
                        if (baseArray_t[i] != _t.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_t[i] != _t.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_t[i] != _t.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_t[i] != _t.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
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

        private void ReadStageX()
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
            FileStream file = File.Open(path + (_stageNum == 1 ? Mary._acfStage1_X_FileName : Mary._acfStage2_X_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _x.baseArray.Length; i++)
            {
                content += ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                    _x.baseArray[i] + Mary._csvFileSplit +
                    _x.modelArray[i] + Mary._csvFileSplit +
                    _x.userArray[i] + Mary._csvFileSplit +
                    _x.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        private void ReadStageY()
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
            FileStream file = File.Open(path + (_stageNum == 1 ? Mary._acfStage1_Y_FileName : Mary._acfStage2_Y_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _y.baseArray.Length; i++)
            {
                content += ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
                    _y.baseArray[i] + Mary._csvFileSplit +
                    _y.modelArray[i] + Mary._csvFileSplit +
                    _y.userArray[i] + Mary._csvFileSplit +
                    _y.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        private void ReadStageT()
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
            FileStream file = File.Open(path + (_stageNum == 1 ? Mary._acfStage1_T_FileName : Mary._acfStage2_T_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _t.baseArray.Length; i++)
            {
                content += ((AcfStagePosition)i).ToString() + Mary._csvFileSplit +
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
        Axises _y;
        Axises _t;
        int _stageNum;
    }
}
