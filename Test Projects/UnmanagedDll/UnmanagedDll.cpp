// UnmanagedDll.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"


extern "C" __declspec(dllexport)
void TestUnmanaged()
{
MessageBox(0, L"", L"bb", 0);
}
