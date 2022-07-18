using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CClinkTest
{
    public class ModelPreBond
    {
        public ModelPreBond()
        {
            _stage = new PreBondStage(1);
            _tool1 = new PreBondTool(1);
            _tool2 = new PreBondTool(2);
            _carrier1 = new PreBondCarrier(1);
            _carrier2 = new PreBondCarrier(2);
            _camera = new PreBondCamera(1);
            _handler = new PreBondHandler(1);
            _usc = new PreBondUSC(1);
            _shuttle1 = new PreBondIcShuttle(1);
            _shuttle2 = new PreBondIcShuttle(2);
            _loadUnload1 = new PreBondIcLoadUnload(1);
            _loadUnload2 = new PreBondIcLoadUnload(2);
            _icCamera = new PreBondIcCamera(1);
            _buffer = new PreBondIcBuffer(1);
        }

        public void ReadFromPLC()
        {
            _stage.ReadFromPLC();
            _tool1.ReadFromPLC();
            _tool2.ReadFromPLC();
            _carrier1.ReadFromPLC();
            _carrier2.ReadFromPLC();
            _camera.ReadFromPLC();
            _handler.ReadFromPLC();
            _usc.ReadFromPLC();
            _shuttle1.ReadFromPLC();
            _shuttle2.ReadFromPLC();
            _loadUnload1.ReadFromPLC();
            _loadUnload2.ReadFromPLC();
            _icCamera.ReadFromPLC();
            _buffer.ReadFromPLC();
        }

        public string CompareWithPLC(PlcSavingTrigger trig)
        {
            string result = string.Empty;
            switch (trig)
            {
                case PlcSavingTrigger.Prb_Stage_X:
                case PlcSavingTrigger.Prb_Stage_X_1:
                case PlcSavingTrigger.Prb_Stage_Y:
                case PlcSavingTrigger.Prb_Stage_Y_1:
                case PlcSavingTrigger.Prb_Stage_T1:
                case PlcSavingTrigger.Prb_Stage_T1_1:
                case PlcSavingTrigger.Prb_Stage_T2:
                case PlcSavingTrigger.Prb_Stage_T2_1:
                    result = _stage.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Prb_Tool1_X:
                case PlcSavingTrigger.Prb_Tool1_Z:
                case PlcSavingTrigger.Prb_Tool1_T:
                    result = _tool1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Prb_Tool2_X:
                case PlcSavingTrigger.Prb_Tool2_Z:
                case PlcSavingTrigger.Prb_Tool2_T:
                    result = _tool2.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Prb_Carrier1_X:
                case PlcSavingTrigger.Prb_Carrier1_X_1:
                case PlcSavingTrigger.Prb_Carrier1_Y:
                case PlcSavingTrigger.Prb_Carrier1_Y_1:
                case PlcSavingTrigger.Prb_Carrier1_Z:
                case PlcSavingTrigger.Prb_Carrier1_Z_1:
                case PlcSavingTrigger.Prb_Carrier1_T1:
                case PlcSavingTrigger.Prb_Carrier1_T2:
                    result = _carrier1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Prb_Carrier2_X:
                case PlcSavingTrigger.Prb_Carrier2_Y:
                case PlcSavingTrigger.Prb_Carrier2_Z:
                case PlcSavingTrigger.Prb_Carrier2_T:
                    result = _carrier2.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Prb_Camera_X:
                case PlcSavingTrigger.Prb_Camera_Y:
                    result = _camera.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Prb_Handler_Default:
                    result = _handler.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.Prb_USC_Default:
                    result = _usc.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.IC_Shuttle1_X:
                case PlcSavingTrigger.IC_Shuttle1_Z:
                    result = _shuttle1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.IC_Shuttle2_X:
                case PlcSavingTrigger.IC_Shuttle2_Z:
                    result = _shuttle2.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.IC_LoadUnload1:
                    result = _loadUnload1.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.IC_LoadUnload2:
                    result = _loadUnload2.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.IC_Camera_Y:
                case PlcSavingTrigger.IC_Camera_T:
                    result = _icCamera.CompareWithPlc(trig);
                    break;
                case PlcSavingTrigger.IC_Buffer:
                    result = _buffer.CompareWithPlc(trig);
                    break;
                default:
                    break;
            }
            return result;
        }

        PreBondStage _stage;
        PreBondTool _tool1, _tool2;
        PreBondCarrier _carrier1, _carrier2;
        PreBondCamera _camera;
        PreBondHandler _handler;
        PreBondUSC _usc;
        PreBondIcShuttle _shuttle1, _shuttle2;
        PreBondIcLoadUnload _loadUnload1, _loadUnload2;
        PreBondIcCamera _icCamera;
        PreBondIcBuffer _buffer;
    }
}
