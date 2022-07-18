#pragma once


class FileManager
{
public:
	FileManager();
	FileManager(tm& t);

	int Write(string msg);
	Result BaseWrite(string msg);
	bool FlushBuffer(UI_MESSAGE Message, string& posMsg, string& baseMsg);
	void bufferToFile(UI_MESSAGE Message);
private:
	int fileExist(bool isBase);
	void createOrOpen(bool isBase);
	void close(bool isBase);
	

	ofstream _file, _baseFile;
	string _path, _basePath;
	string _head, _baseHead;
	tm _time;
	
	
};

