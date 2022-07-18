using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class PreBondIcLoadUnload
    {
        public PreBondIcLoadUnload()
        {
            _default = new Axises((int)PrebondIcLoadUnloadPosition.Count);
        }

        public PreBondIcLoadUnload(int ldUldNum)
        {
            if (ldUldNum == 1)
            {
                _default = new Axises((int)PrebondIcLoadUnloadPosition.Count,
                3360, PlcDeviceType.Base,
                33300, PlcDeviceType.ModelOffset,
                81280, PlcDeviceType.ModelOffset,
                3320, PlcDeviceType.Base,
                0, PlcDeviceType.Base);
            }
            else if (ldUldNum == 2)
            {
                _default = new Axises((int)PrebondIcLoadUnloadPosition.Count,
                3660, PlcDeviceType.Base,
                33600, PlcDeviceType.ModelOffset,
                81400, PlcDeviceType.ModelOffset,
                3620, PlcDeviceType.Base,
                0, PlcDeviceType.Base);
            }
            
            _ldUldNum = ldUldNum;
        }

        public void ReadFromPLC()
        {
            ReadLoadUnloadDefault();
        }

        public string CompareWithPlc(PlcSavingTrigger trig)
        {
            string posChange = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.IC_LoadUnload1:
                case PlcSavingTrigger.IC_LoadUnload2:
                    int[] baseArray = new int[_default.baseArray.Length];
                    _default.baseArray.CopyTo(baseArray, 0);
                    int[] modelArray = new int[_default.baseArray.Length];
                    _default.modelArray.CopyTo(modelArray, 0);
                    int[] userArray = new int[_default.baseArray.Length];
                    _default.userArray.CopyTo(userArray, 0);
                    int[] speedArray = new int[_default.baseArray.Length];
                    _default.speedArray.CopyTo(speedArray, 0);
                    ReadLoadUnloadDefault();
                    for (int i = 0; i < _default.baseArray.Length; i++)
                    {
                        if (baseArray[i] != _default.baseArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcLoadUnloadPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Base.ToString() + Mary._csvFileSplit +
                                (baseArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_default.baseArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (modelArray[i] != _default.modelArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcLoadUnloadPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.ModelOffset.ToString() + Mary._csvFileSplit +
                                (modelArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_default.modelArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (userArray[i] != _default.userArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcLoadUnloadPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.UserOffset.ToString() + Mary._csvFileSplit +
                                (userArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_default.userArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                        if (speedArray[i] != _default.speedArray[i])
                        {
                            posChange += Mary._date.ToString("HH:mm:ss") + Mary._csvFileSplit +
                                trig.ToString() + Mary._csvFileSplit +
                                ((PrebondIcLoadUnloadPosition)i).ToString() + Mary._csvFileSplit +
                                PositionCategory.Speed.ToString() + Mary._csvFileSplit +
                                (speedArray[i] / Mary._dataShow).ToString() + Mary._csvFileSplit +
                                (_default.speedArray[i] / Mary._dataShow).ToString() + Environment.NewLine;
                        }
                    }
                    break;
                default:
                    break;
            }
            return posChange;
        }

        private void ReadLoadUnloadDefault()
        {
            int size = Mary._plcUnitReadLength * _default.baseArray.Length;
            short[] data = new short[_default.baseArray.Length * Mary._timesFromShortToInt];

            int ret = Mary.MdReceiveEx((int)_default.baseDeviceType, _default.baseDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _default.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _default.baseArray[i] = BitConverter.ToInt32(bt, 0);
                }

            }

            ret = Mary.MdReceiveEx((int)_default.modelDeviceType, _default.modelDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _default.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _default.modelArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_default.userDeviceType, _default.userDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _default.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _default.userArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            ret = Mary.MdReceiveEx((int)_default.speedDeviceType, _default.speedDeviceNumber, out size, out data[0]);
            if (ret == 0)
            {
                for (int i = 0; i < _default.baseArray.Length; i++)
                {
                    byte[] btLow = BitConverter.GetBytes(data[i * 2]);
                    byte[] btHigh = BitConverter.GetBytes(data[i * 2 + 1]);
                    byte[] bt = new byte[4] { btLow[0], btLow[1], btHigh[0], btHigh[1] };
                    _default.speedArray[i] = BitConverter.ToInt32(bt, 0);
                }
            }

            string path = Mary._filePath;
            DirectoryInfo di = new DirectoryInfo(path);
            di.Create();
            di.Attributes = FileAttributes.Directory & FileAttributes.Normal;
            FileStream file = File.Open(path + (_ldUldNum == 1 ? Mary._prebondLoadUnload1_Default_FileName : Mary._prebondLoadUnload2_Default_FileName) + Mary._fileExtendName, FileMode.OpenOrCreate);
            file.Seek(0, SeekOrigin.Begin);
            string content = Mary._tableHead;
            for (int i = 0; i < _default.baseArray.Length; i++)
            {
                content += ((PrebondIcLoadUnloadPosition)i).ToString() + Mary._csvFileSplit +
                    _default.baseArray[i] + Mary._csvFileSplit +
                    _default.modelArray[i] + Mary._csvFileSplit +
                    _default.userArray[i] + Mary._csvFileSplit +
                    _default.speedArray[i] + Environment.NewLine;
            }
            file.Write(Encoding.Default.GetBytes(content), 0, content.Length);
            file.Flush();
            file.Close();
        }

        Axises _default;
        int _ldUldNum;
    }
}
