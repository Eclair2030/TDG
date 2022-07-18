using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class AcfFeeding
    {
        public AcfFeeding()
        {
            _turn = new Axises((int)AcfFeedingPosition.Count);
        }

        public AcfFeeding(int feedNum)
        {
            if (feedNum == 1)
            {
                _turn = new Axises((int)AcfFeedingPosition.Count,
                    Mary.BaseAddress(MotorNumber.Acf_Feed1_Turn), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Acf_Feed1_Turn), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Acf_Feed1_Turn), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Acf_Feed1_Turn), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            else if (feedNum == 2)
            {
                _turn = new Axises((int)AcfFeedingPosition.Count,
                    Mary.BaseAddress(MotorNumber.Acf_Feed2_Turn), PlcDeviceType.Base,
                    Mary.ModelAddress(MotorNumber.Acf_Feed2_Turn), PlcDeviceType.ModelOffset,
                    Mary.UserAddress(MotorNumber.Acf_Feed2_Turn), PlcDeviceType.ModelOffset,
                    Mary.SpeedAddress(MotorNumber.Acf_Feed2_Turn), PlcDeviceType.Base,
                    0, PlcDeviceType.Base);
            }
            _feedNum = feedNum;
        }

        public void ReadFromPLC()
        {
            ReadReelTurn();
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.Acf_Feeding_1:
                case PlcSavingTrigger.Acf_Feeding_2:
                    int[] baseArray = new int[_turn.baseArray.Length];
                    _turn.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_turn.baseArray.Length];
                    _turn.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_turn.baseArray.Length];
                    _turn.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_turn.baseArray.Length];
                    _turn.speedArray.CopyTo(speedArray, 0);
                    ReadReelTurn();
                    for (int i = 0; i < _turn.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _turn.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfFeedingPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_turn.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _turn.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfFeedingPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_turn.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _turn.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfFeedingPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_turn.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _turn.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((AcfFeedingPosition)i).ToString() + Mary._csvFileSplit +
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

        private void ReadReelTurn()
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
            FileStream file = File.Open(path + (_feedNum == 1 ? Mary._acfFeed1_Turn_FileName : Mary._acfFeed2_Turn_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _turn.baseArray.Length; i++)
            {
                content += ((AcfFeedingPosition)i).ToString() + Mary._csvFileSplit +
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
        int _feedNum;
    }
}
