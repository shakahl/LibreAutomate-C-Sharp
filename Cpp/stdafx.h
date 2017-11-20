// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

// Including SDKDDKVer.h defines the highest available Windows platform.
// If you wish to build your application for a previous Windows platform, include WinSDKVer.h and set the _WIN32_WINNT macro to the platform you wish to support before including SDKDDKVer.h.
#include <SDKDDKVer.h>

#define WIN32_LEAN_AND_MEAN
#define _CRT_SECURE_NO_WARNINGS

#include <assert.h>
#include <conio.h>
#include <string.h>
#include <sstream>
#include <memory>
#include <comdef.h>
#include <comdefsp.h> //smart pointers of many interfaces

#include <atlbase.h>
#include <atlcoll.h>

#include <windows.h>
#include <Sddl.h>
#include <OleAuto.h>
#include <OleAcc.h>
//#include <UIAutomation.h>

#define PCRE2_STATIC 1
#define PCRE2_CODE_UNIT_WIDTH 16
#include "..\pcre\pcre2.h"



#define null nullptr
#define out
#define ref

using STR = LPCWSTR;

_COM_SMARTPTR_TYPEDEF(IAccessible, __uuidof(IAccessible));

