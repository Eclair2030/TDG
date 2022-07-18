using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class MainBondSheet
    {
        public MainBondSheet() { }
        //61,62
        public MainBondSheet(int sheetNum)
        {
            if (sheetNum == 1)
            {
                _turn = new Axises((int)MainbondSheetPosition.Count,
                    6160, PlcDeviceType.Base,
                    36100, PlcDeviceType.ModelOffset,
                    82400, PlcDeviceType.ModelOffset,
                    6120, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            else if (sheetNum == 2)
            {
                _turn = new Axises((int)MainbondSheetPosition.Count,
                    6260, PlcDeviceType.Base,
                    36200, PlcDeviceType.ModelOffset,
                    82440, PlcDeviceType.ModelOffset,
                    6220, PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            _sheetNum = sheetNum;
        }

        public void ReadFromPLC()
        {
            ReadSheetTurn();
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.Fnb_SheetFeeding1_T:
                case PlcSavingTrigger.Fnb_SheetFeeding2_T:
                    int[] baseArray = new int[_turn.baseArray.Length];
                    _turn.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_turn.baseArray.Length];
                    _turn.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_turn.baseArray.Length];
                    _turn.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_turn.baseArray.Length];
                    _turn.speedArray.CopyTo(speedArray, 0);
                    ReadSheetTurn();
                    for (int i = 0; i < _turn.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _turn.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondToolPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_turn.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _turn.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondToolPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_turn.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _turn.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondToolPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_turn.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _turn.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((MainbondToolPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_turn.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                default:
                    break;
            }
            return posChange;
        }

        private void ReadSheetTurn()
        {
            int size = Mary._plcUnitReadLength * _turn.baseArray.Length;
            short[] data = new short[_turn.baseArray.Length * Mary._timesFromShortToInt];

            int ret = Mary.MdReceiveEx((int)_turn.baseDeviceType, _turn.baseDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _turn.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _turn.baseArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_turn.modelDeviceType, _turn.modelDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _turn.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _turn.modelArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_turn.userDeviceType, _turn.userDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _turn.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _turn.userArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_turn.speedDeviceType, _turn.speedDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _turn.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _turn.speedArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            string path = Mary._filePath;
            DirectoryInfo di = new DirectoryInfo(path);
            di.Create();
            di.Attributes = FileAttributes.Directory & FileAttributes.Normal;
            string fileName = string.Empty;
            switch (_sheetNum)
            {
                case 1:
                    fileName = Mary._mainbondSheet1_Turn_FileName;
                    break;
                case 2:
                    fileName = Mary._mainbondSheet2_Turn_FileName;
                    break;
                default:
                    break;
            }
            FileStream file = File.Open(path + fileName + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _turn.baseArray.Length; i++)
            {
                content += ((MainbondSheetPosition)i).ToString() + Mary._csvFileSplit +
                    _turn.baseArray[i] + Mary._csvFileSplit +
                    _turn.modelArray[i] + Mary._csvFileSplit +
                    _turn.userArray[i] + Mary._csvFileSplit +
                    _turn.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        Axises _turn;
        int _sheetNum;
    }
}
