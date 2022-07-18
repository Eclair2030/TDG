using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class ModelAcf
    {
        public ModelAcf()
        {
            _stage1 = new AcfStage(1);
            _stage2 = new AcfStage(2);
            _feeding1 = new AcfFeeding(1);
            _feeding2 = new AcfFeeding(2);
            _tool1 = new AcfTool(1);
            _tool2 = new AcfTool(2);
            _reel1 = new AcfReel(1);
            _reel2 = new AcfReel(2);
            _separator1 = new AcfSeparator(1);
            _separator2 = new AcfSeparator(2);
            _handler = new AcfHandler(1);
        }

        public void ReadFromPLC()
        {
            _stage1.ReadFromPLC();
            _stage2.ReadFromPLC();
            _feeding1.ReadFromPLC();
            _feeding2.ReadFromPLC();
            _tool1.ReadFromPLC();
            _tool2.ReadFromPLC();
            _reel1.ReadFromPLC();
            _reel2.ReadFromPLC();
            _separator1.ReadFromPLC();
            _separator2.ReadFromPLC();
            _handler.ReadFromPLC();
        }

        public string CompareWithPLC(PlcSavingTrigger trig)
        {
            string result = string.Empty;

            switch (trig)
            {
                case PlcSavingTrigger.Acf_Stage1_X:
                case PlcSavingTrigger.Acf_Stage1_Y:
                case PlcSavingTrigger.Acf_Stage1_T:
                    result = _stage1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Acf_Stage2_X:
                case PlcSavingTrigger.Acf_Stage2_Y:
                case PlcSavingTrigger.Acf_Stage2_T:
                    result = _stage2.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Acf_Feeding_1:
                    result = _feeding1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Acf_Feeding_2:
                    result = _feeding1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Acf_Tool_1:
                    result = _tool1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Acf_Tool_2:
                    result = _tool2.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Acf_Reel_1:
                    result = _reel1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Acf_Reel_2:
                    result = _reel2.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Acf_Separator_1:
                    result = _separator1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Acf_Separator_2:
                    result = _separator2.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Acf_Handler_Default:
                    result = _handler.CompareWithPlc(trig);
                    break;
                default:
                    break;
            }

            return result;
        }

        AcfStage _stage1, _stage2;
        AcfFeeding _feeding1, _feeding2;
        AcfTool _tool1, _tool2;
        AcfReel _reel1, _reel2;
        AcfSeparator _separator1, _separator2;
        AcfHandler _handler;
    }
}
