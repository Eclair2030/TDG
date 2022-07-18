using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class PlasmaStage
    {
        public PlasmaStage()
        {
            _x = new Axises((int)PlasmaStagePosition.Count);
            _y = new Axises((int)PlasmaStagePosition.Count);
            _t1 = new Axises((int)PlasmaStagePosition.Count);
            _t2 = new Axises((int)PlasmaStagePosition.Count);
            _ym = new Axises((int)PlasmaStagePosition.Count);
        }

        public PlasmaStage(int stageNum)
        {
            _x = new Axises((int)PlasmaStagePosition.Count,
                Mary.BaseAddress(MotorNumber.Plasma_Stage_X), PlcDeviceType.Base,
                Mary.ModelAddress(MotorNumber.Plasma_Stage_X), PlcDeviceType.ModelOffset,
                Mary.UserAddress(MotorNumber.Plasma_Stage_X), PlcDeviceType.ModelOffset,
                Mary.SpeedAddress(MotorNumber.Plasma_Stage_X), PlcDeviceType.Base,
                0, PlcDeviceType.Base);
            _y = new Axises((int)PlasmaStagePosition.Count,
                Mary.BaseAddress(MotorNumber.Plasma_Stage_Y), PlcDeviceType.Base,
                Mary.ModelAddress(MotorNumber.Plasma_Stage_Y), PlcDeviceType.ModelOffset,
                Mary.UserAddress(MotorNumber.Plasma_Stage_Y), PlcDeviceType.ModelOffset,
                Mary.SpeedAddress(MotorNumber.Plasma_Stage_Y), PlcDeviceType.Base,
                0, PlcDeviceType.Base);
            _t1 = new Axises((int)PlasmaStagePosition.Count,
                Mary.BaseAddress(MotorNumber.Plasma_Stage_T1), PlcDeviceType.Base,
                Mary.ModelAddress(MotorNumber.Plasma_Stage_T1), PlcDeviceType.ModelOffset,
                Mary.UserAddress(MotorNumber.Plasma_Stage_T1), PlcDeviceType.ModelOffset,
                Mary.SpeedAddress(MotorNumber.Plasma_Stage_T1), PlcDeviceType.Base,
                0, PlcDeviceType.Base);
            _t2 = new Axises((int)PlasmaStagePosition.Count,
                Mary.BaseAddress(MotorNumber.Plasma_Stage_T2), PlcDeviceType.Base,
                Mary.ModelAddress(MotorNumber.Plasma_Stage_T2), PlcDeviceType.ModelOffset,
                Mary.UserAddress(MotorNumber.Plasma_Stage_T2), PlcDeviceType.ModelOffset,
                Mary.SpeedAddress(MotorNumber.Plasma_Stage_T2), PlcDeviceType.Base,
                0, PlcDeviceType.Base);
            _ym = new Axises((int)PlasmaStagePosition.Count,
                Mary.BaseAddress(MotorNumber.Plasma_Stage_Ym), PlcDeviceType.Base,
                Mary.ModelAddress(MotorNumber.Plasma_Stage_Ym), PlcDeviceType.ModelOffset,
                Mary.UserAddress(MotorNumber.Plasma_Stage_Ym), PlcDeviceType.ModelOffset,
                Mary.SpeedAddress(MotorNumber.Plasma_Stage_Ym), PlcDeviceType.Base,
                0, PlcDeviceType.Base);
    }

        public void ReadFromPLC()
        {
            ReadPlasmaStageX();
            ReadPlasmaStageY();
            ReadPlasmaStageT1();
            ReadPlasmaStageT2();
            ReadPlasmaStageYm();
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.Plasma_Stage_X:
                case PlcSavingTrigger.Plasma_Stage_X_1:
                    int[] baseArray = new int[_x.baseArray.Length];
                    _x.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_x.baseArray.Length];
                    _x.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_x.baseArray.Length];
                    _x.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_x.baseArray.Length];
                    _x.speedArray.CopyTo(speedArray, 0);
                    ReadPlasmaStageX();
                    for (int i = 0; i < _x.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _x.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit + 
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit + 
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _x.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _x.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _x.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Plasma_Stage_Y:
                case PlcSavingTrigger.Plasma_Stage_Y_1:
                    int[] baseArray_y = new int[_y.baseArray.Length];
                    _y.baseArray.CopyTo(baseArray_y, 0);
                    int[] modelArray_y = new int[_y.baseArray.Length];
                    _y.modelArray.CopyTo(modelArray_y, 0);
                    int[] userArray_y = new int[_y.baseArray.Length];
                    _y.userArray.CopyTo(userArray_y, 0);
                    int[] speedArray_y = new int[_y.baseArray.Length];
                    _y.speedArray.CopyTo(speedArray_y, 0);
                    ReadPlasmaStageY();
                    for (int i = 0; i < _y.baseArray.Length; i++)
                    {
                        if (baseArray_y[i] != _y.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_y[i] != _y.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_y[i] != _y.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_y[i] != _y.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Plasma_Stage_T1:
                case PlcSavingTrigger.Plasma_Stage_T1_1:
                    int[] baseArray_t1 = new int[_t1.baseArray.Length];
                    _t1.baseArray.CopyTo(baseArray_t1, 0);
                    int[] modelArray_t1 = new int[_t1.baseArray.Length];
                    _t1.modelArray.CopyTo(modelArray_t1, 0);
                    int[] userArray_t1 = new int[_t1.baseArray.Length];
                    _t1.userArray.CopyTo(userArray_t1, 0);
                    int[] speedArray_t1 = new int[_t1.baseArray.Length];
                    _t1.speedArray.CopyTo(speedArray_t1, 0);
                    ReadPlasmaStageT1();
                    for (int i = 0; i < _t1.baseArray.Length; i++)
                    {
                        if (baseArray_t1[i] != _t1.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_t1[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t1.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_t1[i] != _t1.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_t1[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t1.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_t1[i] != _t1.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_t1[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t1.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_t1[i] != _t1.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_t1[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t1.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Plasma_Stage_T2:
                case PlcSavingTrigger.Plasma_Stage_T2_1:
                    int[] baseArray_t2 = new int[_t2.baseArray.Length];
                    _t2.baseArray.CopyTo(baseArray_t2, 0);
                    int[] modelArray_t2 = new int[_t2.baseArray.Length];
                    _t2.modelArray.CopyTo(modelArray_t2, 0);
                    int[] userArray_t2 = new int[_t2.baseArray.Length];
                    _t2.userArray.CopyTo(userArray_t2, 0);
                    int[] speedArray_t2 = new int[_t2.baseArray.Length];
                    _t2.speedArray.CopyTo(speedArray_t2, 0);
                    ReadPlasmaStageT2();
                    for (int i = 0; i < _t2.baseArray.Length; i++)
                    {
                        if (baseArray_t2[i] != _t2.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_t2[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t2.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_t2[i] != _t2.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_t2[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t2.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_t2[i] != _t2.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_t2[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t2.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_t2[i] != _t2.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_t2[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t2.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Plasma_Stage_Ym:
                case PlcSavingTrigger.Plasma_Stage_Ym_1:
                    int[] baseArray_ym = new int[_ym.baseArray.Length];
                    _ym.baseArray.CopyTo(baseArray_ym, 0);
                    int[] modelArray_ym = new int[_ym.baseArray.Length];
                    _ym.modelArray.CopyTo(modelArray_ym, 0);
                    int[] userArray_ym = new int[_ym.baseArray.Length];
                    _ym.userArray.CopyTo(userArray_ym, 0);
                    int[] speedArray_ym = new int[_ym.baseArray.Length];
                    _ym.speedArray.CopyTo(speedArray_ym, 0);
                    ReadPlasmaStageYm();
                    for (int i = 0; i < _ym.baseArray.Length; i++)
                    {
                        if (baseArray_ym[i] != _ym.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_ym[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_ym.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_ym[i] != _ym.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_ym[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_ym.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_ym[i] != _ym.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_ym[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_ym.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_ym[i] != _ym.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_ym[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_ym.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                default:
                    break;
            }
            return posChange;
        }

        private void ReadPlasmaStageX()
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
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1]};
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
            FileStream file = File.Open(path + Mary._plasmaStage_X_FileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _x.baseArray.Length; i++)
            {
                content += ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit + 
                    _x.baseArray[i] + Mary._csvFileSplit + 
                    _x.modelArray[i] + Mary._csvFileSplit + 
                    _x.userArray[i] + Mary._csvFileSplit + 
                    _x.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();

        }

        private void ReadPlasmaStageY()
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
            FileStream file = File.Open(path + Mary._plasmaStage_Y_FileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _y.baseArray.Length; i++)
            {
                content += ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                    _y.baseArray[i] + Mary._csvFileSplit +
                    _y.modelArray[i] + Mary._csvFileSplit +
                    _y.userArray[i] + Mary._csvFileSplit +
                    _y.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();

        }

        private void ReadPlasmaStageT1()
        {
            int size = Mary._plcUnitReadLength * _t1.baseArray.Length;
            short[] data = new short[_t1.baseArray.Length * Mary._timesFromShortToInt];

            int ret = Mary.MdReceiveEx((int)_t1.baseDeviceType, _t1.baseDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t1.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t1.baseArray[i] = BitConverter.ToInt32(bt, 0);
                }

            }

            ret = Mary.MdReceiveEx((int)_t1.modelDeviceType, _t1.modelDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t1.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t1.modelArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_t1.userDeviceType, _t1.userDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t1.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t1.userArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_t1.speedDeviceType, _t1.speedDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t1.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t1.speedArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            string path = Mary._filePath;
            DirectoryInfo di = new DirectoryInfo(path);
            di.Create();
            di.Attributes = FileAttributes.Directory & FileAttributes.Normal;
            FileStream file = File.Open(path + Mary._plasmaStage_T1_FileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _t1.baseArray.Length; i++)
            {
                content += ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                    _t1.baseArray[i] + Mary._csvFileSplit +
                    _t1.modelArray[i] + Mary._csvFileSplit +
                    _t1.userArray[i] + Mary._csvFileSplit +
                    _t1.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();

        }

        private void ReadPlasmaStageT2()
        {
            int size = Mary._plcUnitReadLength * _t2.baseArray.Length;
            short[] data = new short[_t2.baseArray.Length * Mary._timesFromShortToInt];

            int ret = Mary.MdReceiveEx((int)_t2.baseDeviceType, _t2.baseDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t2.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t2.baseArray[i] = BitConverter.ToInt32(bt, 0);
                }

            }

            ret = Mary.MdReceiveEx((int)_t2.modelDeviceType, _t2.modelDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t2.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t2.modelArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_t2.userDeviceType, _t2.userDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t2.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t2.userArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_t2.speedDeviceType, _t2.speedDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _t2.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _t2.speedArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            string path = Mary._filePath;
            DirectoryInfo di = new DirectoryInfo(path);
            di.Create();
            di.Attributes = FileAttributes.Directory & FileAttributes.Normal;
            FileStream file = File.Open(path + Mary._plasmaStage_T2_FileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _t2.baseArray.Length; i++)
            {
                content += ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                    _t2.baseArray[i] + Mary._csvFileSplit +
                    _t2.modelArray[i] + Mary._csvFileSplit +
                    _t2.userArray[i] + Mary._csvFileSplit +
                    _t2.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();

        }

        private void ReadPlasmaStageYm()
        {
            int size = Mary._plcUnitReadLength * _ym.baseArray.Length;
            short[] data = new short[_ym.baseArray.Length * Mary._timesFromShortToInt];

            int ret = Mary.MdReceiveEx((int)_ym.baseDeviceType, _ym.baseDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _ym.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _ym.baseArray[i] = BitConverter.ToInt32(bt, 0);
                }

            }

            ret = Mary.MdReceiveEx((int)_ym.modelDeviceType, _ym.modelDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _ym.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _ym.modelArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_ym.userDeviceType, _ym.userDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _ym.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _ym.userArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_ym.speedDeviceType, _ym.speedDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _ym.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _ym.speedArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            string path = Mary._filePath;
            DirectoryInfo di = new DirectoryInfo(path);
            di.Create();
            di.Attributes = FileAttributes.Directory & FileAttributes.Normal;
            FileStream file = File.Open(path + Mary._plasmaStage_Ym_FileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _ym.baseArray.Length; i++)
            {
                content += ((PlasmaStagePosition)i).ToString() + Mary._csvFileSplit +
                    _ym.baseArray[i] + Mary._csvFileSplit +
                    _ym.modelArray[i] + Mary._csvFileSplit +
                    _ym.userArray[i] + Mary._csvFileSplit +
                    _ym.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();

        }

        Axises _x;
        Axises _y;
        Axises _t1, _t2;
        Axises _ym;
    }
}
