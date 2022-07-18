public enum OperateAction
{
    Start = 0,
    Stop = 1,
    Read = 2,
    Compare = 3
}

public enum Axis
{
    Plasma_Stage_X = 0,
    Plasma_Stage_Y = 1,
    Plasma_Stage_T1 = 2,
    Plasma_Stage_T2 = 3,
    Plasma_Stage_Ym = 4,
    Plasma_Handler_Default = 5
}

public enum PlcDeviceType
{
    ModelOffset = 13,
    Base = 220,
    Trigger = 13
}

public enum PositionCategory
{
    Base = 0,
    ModelOffset = 1,
    UserOffset = 2,
    Speed = 3,
    AlignSpeed = 4
}

public enum PlasmaStagePosition
{
    Load = 0,
    Vision_Mark_R = 1,
    Vision_Mark_L = 2,
    Panel02Work02 = 3,
    Panel02Work01 = 4,
    Air01PanelR = 5,
    Air02PanelR = 6,
    Panel01Work02 = 7,
    Panel01Work01 = 8,
    Empty = 9,
    AirBlower = 10,
    Unload = 11,
    Count = 12
}

public enum PlasmaHandlerPosition
{
    Standby = 0,
    Load = 1,
    Unload = 2,
    Count = 3
}

public enum AcfStagePosition
{
    Standby = 0,
    Load = 1,
    Vision_Mark_2 = 2,
    Vision_Mark_1 = 3,
    Wait = 4,
    Acf_Bonding_2 = 5,
    Acf_Bonding_1 = 6,
    Acf_Inspection_2 = 7,
    Acf_Inspection_1 = 8,
    Unload = 9,
    Count = 10
}

public enum AcfFeedingPosition
{
    Turn = 0,
    Count = 1
}

public enum AcfToolPosition
{
    Up = 0,
    SlowDown = 1,
    Bonding = 2,
    Test_Bonding = 3,
    Count = 4
}

public enum AcfReelPosition
{
    ReelTurn = 0,
    Count = 1
}

public enum AcfSeparatorPosition
{
    Separator_Start = 0,
    Separator_End = 1,
    Count = 2
}

public enum AcfHandlerPosition
{
    Standby = 0,
    Load = 1,
    Unload = 2,
    Count = 3
}

public enum PrebondStagePosition
{
    Load = 0,
    Wait_2 = 1,
    Wait_1 = 2,
    Panel_02_Mark_02 = 3,
    Panel_02_Mark_01 = 4,
    Panel_01_Mark_02 = 5,
    Panel_01_Mark_01 = 6,
    Bond_02_COF_02 = 7,
    Bond_02_COF_01 = 8,
    Bond_01_COF_02 = 9,
    Bond_01_COF_01 = 10,
    Bond_02_IC_02 = 11,
    Bond_02_IC_01 = 12,
    Bond_01_IC_02 = 13,
    Bond_01_IC_01 = 14,
    Unload = 15,
    TestStage_2 = 16,
    TestStage_1 = 17,
    Count = 18
}

public enum PrebondTool_X_Position
{
    Standby = 0,
    Bonding = 1,
    Reject = 2,
    Avoid = 3,
    Load = 4,
    Count = 5
}

public enum PrebondTool_Z_Position
{
    Up_Standby = 0,
    Load = 1,
    Vision = 2,
    SlowDown = 3,
    Bonding = 4,
    Reject = 5,
    TestBonding = 6,
    Count = 7
}

public enum PrebondTool_T_Position
{
    Standby = 0,
    Load = 1,
    Vision = 2,
    Bonding = 3,
    Count = 4
}

public enum PrebondCarrier1_XY_Position
{
    Standby = 0,
    COF_Wait = 1,
    IC_Wait = 2,
    Load_COF_1 = 3,
    Load_COF_2 = 4,
    COF_Align = 5,
    Unload_COF_1 = 6,
    Unload_COF_2 = 7,
    Load_IC_Buffer = 8,
    Load_IC_Tray = 9,
    IC_Align = 10,
    Unload_IC_1 = 11,
    Unload_IC_2 = 12,
    Unload_Wait_1 = 13,
    Unload_Wait_2 = 14,
    Count = 15
}

public enum PrebondCarrier1_Z_Position
{
    Standby = 0,
    COF_Wait = 1,
    IC_Wait = 2,
    Load_COF_1 = 3,
    Load_COF_2 = 4,
    COF_Align = 5,
    Unload_COF_1 = 6,
    Unload_COF_2 = 7,
    Load_IC_Buffer = 8,
    Load_IC_Tray = 9,
    IC_Align = 10,
    Unload_IC_1 = 11,
    Unload_IC_2 = 12,
    REV_Aviod = 13,
    Count = 14
}

public enum PrebondCarrier1_T_Position
{
    Standby = 0,
    COF_Wait = 1,
    IC_Wait = 2,
    Load_IC_Buffer = 3,
    Load_IC_Tray = 4,
    IC_Align = 5,
    Unload_IC_1 = 6,
    Unload_IC_2 = 7,
    Count = 8
}

public enum PrebondCarrier2Position
{
    Standby = 0,
    LD_COF_Press01 = 1,
    LD_COF_Press02 = 2,
    COF_Wait = 3,
    COF_Unload_01 = 4,
    COF_Unload_02 = 5,
    Count = 6
}

public enum PrebondAlignCameraPosition
{
    Standby = 0,
    Work = 1,
    Count = 2
}

public enum PrebondHandlerPosition
{
    Standby = 0,
    Load = 1,
    Unload_Stage_L = 2,
    Unload_Stage_R = 3,
    Count = 4
}

public enum PrebondUSCPosition
{
    Standby = 0,
    Clear_COF1 = 1,
    Clear_COF2 = 2,
    Wait = 3,
    Count = 4
}

public enum PrebondIcShuttle_X_Position
{
    LD = 0,
    ULD = 1,
    Align = 2,
    Pickup = 3,
    Count = 4
}

public enum PrebondIcShuttle_Z_Position
{
    Wait = 0,
    Tray_LD = 1,
    Tray_LD_Complete = 2,
    Tray_ULD = 3,
    Tray_ULD_Complete = 4,
    Vision = 5,
    Pickup = 6,
    Count = 7
}

public enum PrebondIcLoadUnloadPosition
{
    Clamp = 0,
    Unclamp = 1,
    Count = 2
}

public enum PrebondIcCamera_Y_Position
{
    Standby = 0,
    Align_First_Rows = 1,
    Pickup = 2,
    PickDown = 3,
    Align_Second_Rows = 4,
    Count = 5
}

public enum PrebondIcCamera_T_Position
{
    Work_0 = 0,
    Work_90 = 1,
    Work_180 = 2,
    Work_270 = 3,
    Count = 4
}

public enum PrebondIcBufferPosition
{
    Standby = 0,
    IC_LD = 1,
    IC_ULD = 2,
    Count = 3
}

public enum MainbondHandlerPosition
{
    Standby = 0,
    Load_Stage_L = 1,
    Load_Stage_R = 2,
    Panel_R_Mcr = 3,
    Panel_L_Mcr = 4,
    Unload = 5,
    Count = 6
}

public enum MainbondStage_Y_Position
{
    Load = 0,
    PreAlign = 1,
    Vision_Mark_Start = 2,
    Vision_Mark_End = 3,
    Bonding = 4,
    Unload = 5,
    Cassette_Change = 6,
    Test_Bonding = 7,
    Count = 8
}

public enum MainbondStage_T_Position
{
    Load = 0,
    PreAlign = 1,
    Vision_Mark_Start = 2,
    Vision_Mark_End = 3,
    Bonding = 4,
    Unload = 5,
    Cassette_Change = 6,
    Count = 7
}

public enum MainbondToolPosition
{
    Up = 0,
    SlowDown = 1,
    Bonding = 2,
    Test_Bonding = 3,
    Sheet_Change = 4,
    Count = 5
}

public enum MainbondCameraPosition
{
    Stage_12_Mark_2 = 0,
    Stage_12_Mark_1 = 1,
    Stage_34_Mark_2 = 2,
    Stage_34_Mark_1 = 3,
    Count = 4
}

public enum MainbondSheetPosition
{
    Turn = 0,
    Count = 1
}

public enum PlcSavingTrigger
{
    Plasma_Stage_X = 2021,
    Plasma_Stage_X_1 = 2022,
    Plasma_Stage_Y = 2023,
    Plasma_Stage_Y_1 = 2024,
    Plasma_Stage_T1 = 2025,
    Plasma_Stage_T1_1 = 2026,
    Plasma_Stage_T2 = 2027,
    Plasma_Stage_T2_1 = 2028,
    Plasma_Stage_Ym = 2029,
    Plasma_Stage_Ym_1 = 2030,
    Plasma_Handler_Default = 2031,
    Acf_Stage1_X = 2050,
    Acf_Stage1_Y = 2052,
    Acf_Stage1_T = 2054,
    Acf_Stage2_X = 2056,
    Acf_Stage2_Y = 2058,
    Acf_Stage2_T = 2060,
    Acf_Feeding_1 = 2070,
    Acf_Feeding_2 = 2071,
    Acf_Tool_1 = 2075,
    Acf_Tool_2 = 2076,
    Acf_Reel_1 = 2080,
    Acf_Reel_2 = 2081,
    Acf_Separator_1 = 2085,
    Acf_Separator_2 = 2086,
    Acf_Handler_Default = 2110,
    Prb_Stage_X = 2140,
    Prb_Stage_X_1 = 2141,
    Prb_Stage_Y = 2142,
    Prb_Stage_Y_1 = 2143,
    Prb_Stage_T1 = 2144,
    Prb_Stage_T1_1 = 2145,
    Prb_Stage_T2 = 2146,
    Prb_Stage_T2_1 = 2147,
    Prb_Tool1_X = 2150,
    Prb_Tool1_Z = 2170,
    Prb_Tool1_T = 2171,
    Prb_Tool2_X = 2160,
    Prb_Tool2_Z = 2172,
    Prb_Tool2_T = 2173,
    Prb_Carrier1_X = 2210,
    Prb_Carrier1_X_1 = 2211,
    Prb_Carrier1_Y = 2212,
    Prb_Carrier1_Y_1 = 2213,
    Prb_Carrier1_Z = 2214,
    Prb_Carrier1_Z_1 = 2215,
    Prb_Carrier1_T1 = 2216,
    Prb_Carrier1_T2 = 2217,
    Prb_Carrier2_X = 2218,
    Prb_Carrier2_Y = 2219,
    Prb_Carrier2_Z = 2220,
    Prb_Carrier2_T = 2221,
    Prb_Camera_X = 2190,
    Prb_Camera_Y = 2191,
    Prb_Handler_Default = 2200,
    Prb_USC_Default = 2222,
    IC_Shuttle1_X = 2230,
    IC_Shuttle1_Z = 2231,
    IC_Shuttle2_X = 2240,
    IC_Shuttle2_Z = 2241,
    IC_LoadUnload1 = 2250,
    IC_LoadUnload2 = 2260,
    IC_Camera_Y = 2270,
    IC_Camera_T = 2271,
    IC_Buffer = 2280,
    Fnb_Handler_Default = 2501,
    Fnb_Stage1_Stage1Y = 2503,
    Fnb_Stage1_Stage1T = 2505,
    Fnb_Stage1_Stage2Y = 2507,
    Fnb_Stage1_Stage2T = 2509,
    Fnb_Stage2_Stage3Y = 2511,
    Fnb_Stage2_Stage3T = 2513,
    Fnb_Stage2_Stage4Y = 2515,
    Fnb_Stage2_Stage4T = 2517,
    Fnb_Tool1_Z = 2519,
    Fnb_Tool2_Z = 2521,
    Fnb_Tool3_Z = 2523,
    Fnb_Tool4_Z = 2525,
    Fnb_Camera_Default = 2527,
    Fnb_SheetFeeding1_T = 2529,
    Fnb_SheetFeeding2_T = 2531
}

public enum MotorNumber
{
    Plasma_Stage_X = 1,
    Plasma_Stage_Y = 2,
    Plasma_Stage_T1 = 4,
    Plasma_Stage_T2 = 5,
    Plasma_Stage_Ym = 3,
    Plasma_Handler_Default = 12,
    Acf_Stage1_X = 65,
    Acf_Stage1_Y = 8,
    Acf_Stage1_T = 10,
    Acf_Stage2_X = 66,
    Acf_Stage2_Y = 9,
    Acf_Stage2_T = 11,
    Acf_Feed1_Turn = 13,
    Acf_Feed2_Turn = 17,
    Acf_Tool1_Z = 14,
    Acf_Tool2_Z = 18,
    Acf_Reel1_Turn = 15,
    Acf_Reel2_Turn = 19,
    Acf_Separator1_Default = 16,
    Acf_Separator2_Default = 20,
    Acf_Handler_Default = 23,
    Prb_Stage_X = 67,
    Prb_Stage_Y = 68,
    Prb_Stage_T1 = 21,
    Prb_Stage_T2 = 22,
    Prb_Tool1_X = 70,
    Prb_Tool1_Z = 25,
    Prb_Tool1_T = 78,
    Prb_Tool2_X = 69,
    Prb_Tool2_Z = 24,
    Prb_Tool2_T = 77,
    Prb_Carrier1_X = 71,
    Prb_Carrier1_Y = 72,
    Prb_Carrier1_Z = 29,
    Prb_Carrier1_T1 = 79,
    Prb_Carrier1_T2 = 30,
    Prb_Carrier2_X = 73,
    Prb_Carrier2_Y = 74,
    Prb_Carrier2_Z = 31,
    Prb_Carrier2_T = 80,
    Prb_Camera_X = 26,
    Prb_Camera_Y = 27,
    Prb_Handler_Default = 75,
    Prb_USC_Default = 28,
    IC_Shuttle1_X = 34,
    IC_Shuttle1_Z = 35,
    IC_Shuttle2_X = 37,
    IC_Shuttle2_Z = 38,
    IC_LoadUnload1 = 33,
    IC_LoadUnload2 = 36,
    IC_Camera_Y = 81,
    IC_Camera_T = 82,
    IC_Buffer = 83,
    Fnb_Handler_Default = 76,
    Fnb_Stage1_Stage1Y = 49,
    Fnb_Stage1_Stage1T = 57,
    Fnb_Stage1_Stage2Y = 50,
    Fnb_Stage1_Stage2T = 58,
    Fnb_Stage2_Stage3Y = 51,
    Fnb_Stage2_Stage3T = 59,
    Fnb_Stage2_Stage4Y = 52,
    Fnb_Stage2_Stage4T = 60,
    Fnb_Tool1_Z = 53,
    Fnb_Tool2_Z = 54,
    Fnb_Tool3_Z = 55,
    Fnb_Tool4_Z = 56,
    Fnb_Camera_Default = 63,
    Fnb_SheetFeeding1_T = 61,
    Fnb_SheetFeeding2_T = 62
}