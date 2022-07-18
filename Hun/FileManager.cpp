#include "pch.h"
#include "FileManager.h"
#include <direct.h>

FileManager::FileManager()
{
	_path = FILE_PATH;
	_head = TABLE_HEAD;
	_basePath = BASE_FILE_PATH;
	_baseHead = BASE_TABLE_HEAD;
}

FileManager::FileManager(tm& t)
{
	_time = t;

	_path = FILE_PATH + to_string(t.tm_year + 1900) + "//" + to_string(t.tm_mon + 1) + "//";
	_mkdir(_path.c_str());
	_path += to_string(t.tm_mday) + ".csv";
	_head = TABLE_HEAD;

	_basePath = BASE_FILE_PATH + to_string(t.tm_year + 1900) + "//" + to_string(t.tm_mon + 1) + "//";
	_mkdir(_basePath.c_str());
	_basePath += "base_" + to_string(t.tm_mday) + ".csv";
	_baseHead = BASE_TABLE_HEAD;
}

int FileManager::Write(string msg)
{
	int result = Result::UNKNOW;
	try
	{
		createOrOpen(false);
		if (_file.is_open())
		{
			_file << msg;
			close(false);
			result = Result::SUC;
		}
		else
		{
			result = Result::FAIL;
		}
	}
	catch (const std::exception&)
	{
		result = Result::FAIL;
	}
	return result;
}

Result FileManager::BaseWrite(string msg)
{
	Result result = Result::UNKNOW;
	try
	{
		createOrOpen(true);
		if (_baseFile.is_open())
		{
			_baseFile << msg;
			close(true);
			result = Result::SUC;
		}
		else
		{
			result = Result::FAIL;
		}
	}
	catch (const exception&)
	{
		result = Result::FAIL;
	}
	return result;
}

bool FileManager::FlushBuffer(UI_MESSAGE Message, string& posMsg, string& baseMsg)
{
	bool result = false;
	int count = 0;
	if (!posMsg.empty())
	{
		createOrOpen(false);
		if (_file.is_open())
		{
			_file << posMsg;
			close(false);
			Message("position recipe buffer write complete.", MessageType::FunctionResult);
			posMsg.clear();
			count++;
		}
	}
	else
	{
		count++;
	}
	if (!baseMsg.empty())
	{
		createOrOpen(true);
		if (_baseFile.is_open())
		{
			_baseFile << baseMsg;
			close(true);
			Message("base recipe buffer write complete.", MessageType::FunctionResult);
			baseMsg.clear();
			count++;
		}
	}
	else
	{
		count++;
	}
	if (count == 2)
	{
		result = true;
	}
	return result;
}

int FileManager::fileExist(bool isBase)
{
	struct stat buffer;
	return stat(isBase ? _basePath.c_str() : _path.c_str(), &buffer);
}

void FileManager::createOrOpen(bool isBase)
{
	string head = fileExist(isBase) != 0 ? (isBase ? _baseHead : _head) : "";
	if (isBase)
	{
		_baseFile.open(_basePath, ios::binary | ios::app | ios::in | ios::out);
		_baseFile << head;
	}
	else
	{
		_file.open(_path, ios::binary | ios::app | ios::in | ios::out);
		_file << head;
	}
}

void FileManager::close(bool isBase)
{
	if (isBase)
	{
		if (_baseFile.is_open())
		{
			_baseFile.close();
		}
	}
	else
	{
		if (_file.is_open())
		{
			_file.close();
		}
	}
}

void FileManager::bufferToFile(UI_MESSAGE Message)
{
	
}