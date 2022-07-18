using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class PreBondCarrier
    {
        public PreBondCarrier()
        {
            _x = new Axises((int)PrebondCarrier1_XY_Position.Count);
            _y = new Axises((int)PrebondCarrier1_XY_Position.Count);
            _z = new Axises((int)PrebondCarrier1_Z_Position.Count);
            _t1 = new Axises((int)PrebondCarrier1_T_Position.Count);
            _t2 = new Axises((int)PrebondCarrier1_T_Position.Count);
        }

        public PreBondCarrier(int carrierNum)
        {
            if (carrierNum == 1)
            {
                _x = new Axises((int)PrebondCarrier1_XY_Position.Count,
                    7160, PlcDeviceType.Base,
                    37100, PlcDeviceType.ModelOffset,
                    82800, PlcDeviceType.ModelOffset,
                    7120, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _y = new Axises((int)PrebondCarrier1_XY_Position.Count,
                    7260, PlcDeviceType.Base,
                    37200, PlcDeviceType.ModelOffset,
                    82840, PlcDeviceType.ModelOffset,
                    7220, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _z = new Axises((int)PrebondCarrier1_Z_Position.Count,
                    7960, PlcDeviceType.Base,
                    32900, PlcDeviceType.ModelOffset,
                    81120, PlcDeviceType.ModelOffset,
                    2920, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t1 = new Axises((int)PrebondCarrier1_T_Position.Count,
                    2960, PlcDeviceType.Base,
                    37900, PlcDeviceType.ModelOffset,
                    83120, PlcDeviceType.ModelOffset,
                    7920, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t2 = new Axises((int)PrebondCarrier1_T_Position.Count,
                    3060, PlcDeviceType.Base,
                    33000, PlcDeviceType.ModelOffset,
                    81160, PlcDeviceType.ModelOffset,
                    3020, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            else if (carrierNum == 2)
            {
                _x = new Axises((int)PrebondCarrier2Position.Count,
                    7360, PlcDeviceType.Base,
                    37300, PlcDeviceType.ModelOffset,
                    82880, PlcDeviceType.ModelOffset,
                    7320, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _y = new Axises((int)PrebondCarrier2Position.Count,
                    7460, PlcDeviceType.Base,
                    37400, PlcDeviceType.ModelOffset,
                    82920, PlcDeviceType.ModelOffset,
                    7420, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _z = new Axises((int)PrebondCarrier2Position.Count,
                    3160, PlcDeviceType.Base,
                    33100, PlcDeviceType.ModelOffset,
                    81200, PlcDeviceType.ModelOffset,
                    3120, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t1 = new Axises((int)PrebondCarrier2Position.Count,
                    8060, PlcDeviceType.Base,
                    38000, PlcDeviceType.ModelOffset,
                    83160, PlcDeviceType.ModelOffset,
                    8020, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            _carrierNum = carrierNum;
        }

        public void ReadFromPLC()
        {
            ReadCarrierX();
            ReadCarrierY();
            ReadCarrierZ();
            ReadCarrierT1();
            if (_carrierNum == 1)
            {
                ReadCarrierT2();
            }
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;

            switch (trig)
            {
                case PlcSavingTrigger.Prb_Carrier1_X:
                case PlcSavingTrigger.Prb_Carrier1_X_1:
                case PlcSavingTrigger.Prb_Carrier2_X:
                    int[] baseArray = new int[_x.baseArray.Length];
                    _x.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_x.baseArray.Length];
                    _x.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_x.baseArray.Length];
                    _x.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_x.baseArray.Length];
                    _x.speedArray.CopyTo(speedArray, 0);
                    ReadCarrierX();
                    for (int i = 0; i < _x.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _x.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_XY_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _x.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_XY_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _x.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_XY_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _x.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_XY_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_x.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Prb_Carrier1_Y:
                case PlcSavingTrigger.Prb_Carrier1_Y_1:
                case PlcSavingTrigger.Prb_Carrier2_Y:
                    int[] baseArray_y = new int[_y.baseArray.Length];
                    _y.baseArray.CopyTo(baseArray_y, 0);
                    int[] modelArray_y = new int[_y.baseArray.Length];
                    _y.modelArray.CopyTo(modelArray_y, 0);
                    int[] userArray_y = new int[_y.baseArray.Length];
                    _y.userArray.CopyTo(userArray_y, 0);
                    int[] speedArray_y = new int[_y.baseArray.Length];
                    _y.speedArray.CopyTo(speedArray_y, 0);
                    ReadCarrierY();
                    for (int i = 0; i < _y.baseArray.Length; i++)
                    {
                        if (baseArray_y[i] != _y.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_XY_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_y[i] != _y.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_XY_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_y[i] != _y.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_XY_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_y[i] != _y.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_XY_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_y[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Prb_Carrier1_Z:
                case PlcSavingTrigger.Prb_Carrier1_Z_1:
                case PlcSavingTrigger.Prb_Carrier2_Z:
                    int[] baseArray_z = new int[_z.baseArray.Length];
                    _z.baseArray.CopyTo(baseArray_z, 0);
                    int[] modelArray_z = new int[_z.baseArray.Length];
                    _z.modelArray.CopyTo(modelArray_z, 0);
                    int[] userArray_z = new int[_z.baseArray.Length];
                    _z.userArray.CopyTo(userArray_z, 0);
                    int[] speedArray_z = new int[_z.baseArray.Length];
                    _z.speedArray.CopyTo(speedArray_z, 0);
                    ReadCarrierZ();
                    for (int i = 0; i < _z.baseArray.Length; i++)
                    {
                        if (baseArray_z[i] != _z.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_Z_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_z[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_z[i] != _z.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_Z_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_z[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_z[i] != _z.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_Z_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_z[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_z[i] != _z.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_Z_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_z[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_z.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Prb_Carrier1_T1:
                case PlcSavingTrigger.Prb_Carrier2_T:
                    int[] baseArray_t1 = new int[_t1.baseArray.Length];
                    _t1.baseArray.CopyTo(baseArray_t1, 0);
                    int[] modelArray_t1 = new int[_t1.baseArray.Length];
                    _t1.modelArray.CopyTo(modelArray_t1, 0);
                    int[] userArray_t1 = new int[_t1.baseArray.Length];
                    _t1.userArray.CopyTo(userArray_t1, 0);
                    int[] speedArray_t1 = new int[_t1.baseArray.Length];
                    _t1.speedArray.CopyTo(speedArray_t1, 0);
                    ReadCarrierT1();
                    for (int i = 0; i < _t1.baseArray.Length; i++)
                    {
                        if (baseArray_t1[i] != _t1.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_T_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_t1[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t1.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_t1[i] != _t1.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_T_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_t1[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t1.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_t1[i] != _t1.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_T_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_t1[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t1.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_t1[i] != _t1.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                (_carrierNum == 1 ? ((PrebondCarrier1_T_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_t1[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t1.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Prb_Carrier1_T2:
                    int[] baseArray_t2 = new int[_t2.baseArray.Length];
                    _t2.baseArray.CopyTo(baseArray_t2, 0);
                    int[] modelArray_t2 = new int[_t2.baseArray.Length];
                    _t2.modelArray.CopyTo(modelArray_t2, 0);
                    int[] userArray_t2 = new int[_t2.baseArray.Length];
                    _t2.userArray.CopyTo(userArray_t2, 0);
                    int[] speedArray_t2 = new int[_t2.baseArray.Length];
                    _t2.speedArray.CopyTo(speedArray_t2, 0);
                    ReadCarrierT2();
                    for (int i = 0; i < _t2.baseArray.Length; i++)
                    {
                        if (baseArray_t2[i] != _t2.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondCarrier1_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_t2[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t2.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_t2[i] != _t2.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondCarrier1_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_t2[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t2.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_t2[i] != _t2.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondCarrier1_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_t2[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t2.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_t2[i] != _t2.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondCarrier1_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray_t2[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t2.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                default:
                    break;
            }

            return posChange;
        }

        private void ReadCarrierX()
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
            FileStream file = File.Open(path + (_carrierNum == 1 ? Mary._prebondCarrier1_X_FileName : Mary._prebondCarrier2_X_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _x.baseArray.Length; i++)
            {
                content += (_carrierNum == 1 ? ((PrebondCarrier1_XY_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                    _x.baseArray[i] + Mary._csvFileSplit +
                    _x.modelArray[i] + Mary._csvFileSplit +
                    _x.userArray[i] + Mary._csvFileSplit +
                    _x.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        private void ReadCarrierY()
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
            FileStream file = File.Open(path + (_carrierNum == 1 ? Mary._prebondCarrier1_Y_FileName : Mary._prebondCarrier2_Y_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _y.baseArray.Length; i++)
            {
                content += (_carrierNum == 1 ? ((PrebondCarrier1_XY_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                    _y.baseArray[i] + Mary._csvFileSplit +
                    _y.modelArray[i] + Mary._csvFileSplit +
                    _y.userArray[i] + Mary._csvFileSplit +
                    _y.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        private void ReadCarrierZ()
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
            FileStream file = File.Open(path + (_carrierNum == 1 ? Mary._prebondCarrier1_Z_FileName : Mary._prebondCarrier2_Z_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _z.baseArray.Length; i++)
            {
                content += (_carrierNum == 1 ? ((PrebondCarrier1_Z_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                    _z.baseArray[i] + Mary._csvFileSplit +
                    _z.modelArray[i] + Mary._csvFileSplit +
                    _z.userArray[i] + Mary._csvFileSplit +
                    _z.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        private void ReadCarrierT1()
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
            FileStream file = File.Open(path + (_carrierNum == 1 ? Mary._prebondCarrier1_T1_FileName : Mary._prebondCarrier2_T_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _t1.baseArray.Length; i++)
            {
                content += (_carrierNum == 1 ? ((PrebondCarrier1_T_Position)i).ToString() : ((PrebondCarrier2Position)i).ToString()) + Mary._csvFileSplit +
                    _t1.baseArray[i] + Mary._csvFileSplit +
                    _t1.modelArray[i] + Mary._csvFileSplit +
                    _t1.userArray[i] + Mary._csvFileSplit +
                    _t1.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        private void ReadCarrierT2()
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
            FileStream file = File.Open(path + Mary._prebondCarrier1_T2_FileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _t2.baseArray.Length; i++)
            {
                content += ((PrebondCarrier1_T_Position)i).ToString() + Mary._csvFileSplit +
                    _t2.baseArray[i] + Mary._csvFileSplit +
                    _t2.modelArray[i] + Mary._csvFileSplit +
                    _t2.userArray[i] + Mary._csvFileSplit +
                    _t2.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        Axises _x;
        Axises _y;
        Axises _z;
        Axises _t1, _t2;
        int _carrierNum;
    }
}
