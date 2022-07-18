using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class ModelPlasma
    {
        public ModelPlasma()
        {
            _stage = new PlasmaStage(1);
            _handler = new PlasmaHandler(1);
        }

        public void ReadFromPLC()
        {
            _stage.ReadFromPLC();
            _handler.ReadFromPLC();
        }

        public string CompareWithPLC(PlcSavingTrigger trig)
        {
            string result = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.Plasma_Stage_X:
                case PlcSavingTrigger.Plasma_Stage_Y:
                case PlcSavingTrigger.Plasma_Stage_T1:
                case PlcSavingTrigger.Plasma_Stage_T2:
                case PlcSavingTrigger.Plasma_Stage_Ym:
                case PlcSavingTrigger.Plasma_Stage_X_1:
                case PlcSavingTrigger.Plasma_Stage_Y_1:
                case PlcSavingTrigger.Plasma_Stage_T1_1:
                case PlcSavingTrigger.Plasma_Stage_T2_1:
                case PlcSavingTrigger.Plasma_Stage_Ym_1:
                    result = _stage.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Plasma_Handler_Default:
                    result = _handler.CompareWithPlc(trig);
                    break;
                default:
                    break;
            }
            return result;
        }

        PlasmaStage _stage;
        PlasmaHandler _handler;
    }
}
