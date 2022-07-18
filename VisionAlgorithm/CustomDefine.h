#pragma once
#define UNKNOW									-1
#define COMPLETE									0
#define FAIL												1
#define TIMEOUT									2
#define DISCONNECT								3
#define OTHEREXCEPTION					-2

#define UNIT_D										0xA8

#define HEARTBEAT_LOCAL					1
#define HEARTBEAT_UPSYS					2

#define SHOT_UNIT_COUNT					2

#define SIGNAL_DEVICE							200
#define SIGNAL_RESULT_DEVICE			300

#define PI													acos(-1)

enum UpSysType
{
	Default = 0,
	Keyence = 1,
	Mitsubishi = 2,
	Omron = 3,
};

enum UpSysConnResult
{
	NoAnswer = 2,
};

enum CalibResult
{
	SignalFail = 1,
	SnapFail = 2,
	MarkFail = 3,
	CalculateFail = 4,
};

enum MatchResult
{
	OK = 0,
	MarkMiss = 2,
	ImageSizeError = 3,
};

enum SequenceSnapStep
{
	None = 0,
	Empty = 1,
	OneTaken = 2
};

typedef struct MarkCenter
{
	double X;
	double Y;
};

typedef void (_stdcall *MESSAGE_SHOW)(const char* msg);