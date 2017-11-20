// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

// Including SDKDDKVer.h defines the highest available Windows platform.
// If you wish to build your application for a previous Windows platform, include WinSDKVer.h and set the _WIN32_WINNT macro to the platform you wish to support before including SDKDDKVer.h.
#include <SDKDDKVer.h>

#define _CRT_SECURE_NO_WARNINGS
#include <assert.h>
#include <conio.h>
#include <string.h>
#include <sstream>
#include <comdef.h>
#include <comdefsp.h> //smart pointers of many interfaces

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <OleAuto.h>
#include <UIAutomation.h>



#define REF

_COM_SMARTPTR_TYPEDEF(IAccessible, __uuidof(IAccessible));

