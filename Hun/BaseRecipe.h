#pragma once
#include "KVC.h"
class BaseRecipe
{
public:
	BaseRecipe();
	int GetRecipeID();
	int SetRecipeID(int id);
	DBERROR ReadBaseRecipedatas(UI_MESSAGE Message, KVC* k, Result& r);
	Result Compare(UI_MESSAGE Message, string& msg, KVC* k, string dt);

private:
	int _id;
};

