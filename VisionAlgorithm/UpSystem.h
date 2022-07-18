#pragma once
#include "Model.h"

class UpSystem
{
public:
	UpSystem();
	UpSystem(int usType, const char* serverIpAddress, int serverPort);
	~UpSystem();
	int						Connect(MESSAGE_SHOW Message);
	int						Close(MESSAGE_SHOW Message);
	int						HeartBeat(MESSAGE_SHOW Message);
	int						GetSignal(MESSAGE_SHOW Message, int* Command, int* SubCommand, int* Code);
	int						InitShot(Model* m, MESSAGE_SHOW Message);
	UpSystem&		operator=(const UpSystem& us);

private:
	int						_WriteToUpSystem(int code, int number, int count, char* data, MESSAGE_SHOW Message);
	int						_ReadFromUpSystem(int code, int number, int count, char* data, MESSAGE_SHOW Message);
	SOCKET				UPSYS_CONNECTION;
	sockaddr_in		ADDR;
	int						US_TYPE;
	bool					IS_CONNECTED;
};

