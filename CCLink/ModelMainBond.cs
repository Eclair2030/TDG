using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class ModelMainBond
    {
        public ModelMainBond()
        {
            _handler = new MainBondHandler(1);
            _stage1 = new MainBondStage(1);
            _stage2 = new MainBondStage(2);
            _stage3 = new MainBondStage(3);
            _stage4 = new MainBondStage(4);
            _tool1 = new MainBondTool(1);
            _tool2 = new MainBondTool(2);
            _tool3 = new MainBondTool(3);
            _tool4 = new MainBondTool(4);
            _camera = new MainBondCamera(1);
            _sheet12 = new MainBondSheet(1);
            _sheet34 = new MainBondSheet(2);
        }

        public void ReadFromPLC()
        {
            _handler.ReadFromPLC();
            _stage1.ReadFromPLC();
            _stage2.ReadFromPLC();
            _stage3.ReadFromPLC();
            _stage4.ReadFromPLC();
            _tool1.ReadFromPLC();
            _tool2.ReadFromPLC();
            _tool3.ReadFromPLC();
            _tool4.ReadFromPLC();
            _camera.ReadFromPLC();
            _sheet12.ReadFromPLC();
            _sheet34.ReadFromPLC();
        }

        public string CompareWithPLC(PlcSavingTrigger trig)
        {
            string result = string.Empty;

            switch (trig)
            {
                case PlcSavingTrigger.Fnb_Handler_Default:
                    result = _handler.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_Stage1_Stage1Y:
                case PlcSavingTrigger.Fnb_Stage1_Stage1T:
                    result = _stage1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_Stage1_Stage2Y:
                case PlcSavingTrigger.Fnb_Stage1_Stage2T:
                    result = _stage2.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_Stage2_Stage3Y:
                case PlcSavingTrigger.Fnb_Stage2_Stage3T:
                    result = _stage3.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_Stage2_Stage4Y:
                case PlcSavingTrigger.Fnb_Stage2_Stage4T:
                    result = _stage4.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_Tool1_Z:
                    result = _tool1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_Tool2_Z:
                    result = _tool2.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_Tool3_Z:
                    result = _tool3.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_Tool4_Z:
                    result = _tool4.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_Camera_Default:
                    result = _camera.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_SheetFeeding1_T:
                    result = _sheet12.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Fnb_SheetFeeding2_T:
                    result = _sheet34.CompareWithPlc(trig);
                    break;
                default:
                    break;
            }

            return result;
        }

        MainBondHandler _handler;
        MainBondStage _stage1, _stage2, _stage3, _stage4;
        MainBondTool _tool1, _tool2, _tool3, _tool4;
        MainBondCamera _camera;
        MainBondSheet _sheet12, _sheet34;
    }
}
