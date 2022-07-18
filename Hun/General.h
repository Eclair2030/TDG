#pragma once

typedef enum
{
	Off = 0,
	On = 1,
}Trigger_Signal;

typedef enum
{
	None = 0,
	Create = 1,
	Copy = 2,
	Delete = 3,
	Edit = 4,
}CommandCode;

typedef enum
{
	UNKNOW = 0,
	SUC = 1,
	FAIL = 2,
	CONNERR = 3,
	SAME = 4,
	DIFF = 5,
	EMPTY = 6,
}Result;

typedef enum
{
	StartAddress = 0,
	BaseType = 1,
	UserType = 2,
	SpeedType = 3,
}Map_Index;

typedef enum
{
	Default = 0,
	CppProcess = 1,
	FunctionResult = 2,
	Error = 3,
}MessageType;

typedef void(*UI_MESSAGE)(const char* msg, MessageType t);
typedef void(*Flush_Buffer)(int iStart);
