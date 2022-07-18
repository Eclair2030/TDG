using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class MainBondStage
    {
        public MainBondStage() { }

        public MainBondStage(int stageNum)
        {
            if (stageNum == 1)
            {
                _y = new Axises((int)MainbondStage_Y_Position.Count,
                    4960, PlcDeviceType.Base,
                    34900, PlcDeviceType.ModelOffset,
                    81920, PlcDeviceType.ModelOffset,
                    4920, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t = new Axises((int)MainbondStage_T_Position.Count,
                    5760, PlcDeviceType.Base,
                    35700, PlcDeviceType.ModelOffset,
                    82240, PlcDeviceType.ModelOffset,
                    5720, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            else if (stageNum == 2)
            {
                _y = new Axises((int)MainbondStage_Y_Position.Count,
                    5060, PlcDeviceType.Base,
                    35000, PlcDeviceType.ModelOffset,
                    81960, PlcDeviceType.ModelOffset,
                    5020, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t = new Axises((int)MainbondStage_T_Position.Count,
                    5860, PlcDeviceType.Base,
                    35800, PlcDeviceType.ModelOffset,
                    82280, PlcDeviceType.ModelOffset,
                    5820, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            else if (stageNum == 3)
            {
                _y = new Axises((int)MainbondStage_Y_Position.Count,
                    5160, PlcDeviceType.Base,
                    35100, PlcDeviceType.ModelOffset,
                    82000, PlcDeviceType.ModelOffset,
                    5120, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t = new Axises((int)MainbondStage_T_Position.Count,
                    5960, PlcDeviceType.Base,
                    35900, PlcDeviceType.ModelOffset,
                    82320, PlcDeviceType.ModelOffset,
                    5920, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            else if (stageNum == 4)
            {
                _y = new Axises((int)MainbondStage_Y_Position.Count,
                    5260, PlcDeviceType.Base,
                    35200, PlcDeviceType.ModelOffset,
                    82040, PlcDeviceType.ModelOffset,
                    5220, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
                _t = new Axises((int)MainbondStage_T_Position.Count,
                    6060, PlcDeviceType.Base,
                    36000, PlcDeviceType.ModelOffset,
                    82360, PlcDeviceType.ModelOffset,
                    6020, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            _stageNum = stageNum;
        }

        public void ReadFromPLC()
        {
            ReadStageY();
            ReadStageT();
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.Fnb_Stage1_Stage1Y:
                case PlcSavingTrigger.Fnb_Stage1_Stage2Y:
                case PlcSavingTrigger.Fnb_Stage2_Stage3Y:
                case PlcSavingTrigger.Fnb_Stage2_Stage4Y:
                    int[] baseArray = new int[_y.baseArray.Length];
                    _y.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_y.baseArray.Length];
                    _y.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_y.baseArray.Length];
                    _y.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_y.baseArray.Length];
                    _y.speedArray.CopyTo(speedArray, 0);
                    ReadStageY();
                    for (int i = 0; i < _y.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _y.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondStage_Y_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _y.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondStage_Y_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _y.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondStage_Y_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _y.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondStage_Y_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_y.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                case PlcSavingTrigger.Fnb_Stage1_Stage1T:
                case PlcSavingTrigger.Fnb_Stage1_Stage2T:
                case PlcSavingTrigger.Fnb_Stage2_Stage3T:
                case PlcSavingTrigger.Fnb_Stage2_Stage4T:
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
                                ((MainbondStage_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray_t[i] != _t.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondStage_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray_t[i] != _t.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondStage_T_Position)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray_t[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_t.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray_t[i] != _t.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondStage_T_Position)i).ToString() + Mary._csvFileSplit +
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
            string fileName = string.Empty;
            switch (_stageNum)
            {
                case 1:
                    fileName = Mary._mainbondStage1_Y_FileName;
                    break;
                case 2:
                    fileName = Mary._mainbondStage2_Y_FileName;
                    break;
                case 3:
                    fileName = Mary._mainbondStage3_Y_FileName;
                    break;
                case 4:
                    fileName = Mary._mainbondStage4_Y_FileName;
                    break;
                default:
                    break;
            }
            FileStream file = File.Open(path + fileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _y.baseArray.Length; i++)
            {
                content += ((MainbondStage_Y_Position)i).ToString() + Mary._csvFileSplit +
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
            string fileName = string.Empty;
            switch (_stageNum)
            {
                case 1:
                    fileName = Mary._mainbondStage1_T_FileName;
                    break;
                case 2:
                    fileName = Mary._mainbondStage2_T_FileName;
                    break;
                case 3:
                    fileName = Mary._mainbondStage3_T_FileName;
                    break;
                case 4:
                    fileName = Mary._mainbondStage4_T_FileName;
                    break;
                default:
                    break;
            }
            FileStream file = File.Open(path + fileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _t.baseArray.Length; i++)
            {
                content += ((MainbondStage_T_Position)i).ToString() + Mary._csvFileSplit +
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
        int _stageNum;
    }
}
