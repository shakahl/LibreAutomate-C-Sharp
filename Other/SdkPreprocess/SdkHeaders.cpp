#define _PREFAST_ //SAL

#define WINVER 0x0A00 //Win10
#define _WIN32_WINNT 0x0A00
#define _WIN32_IE 0x0B00 //IE11
#define _CRTIMP __declspec(dllimport)

//prevent adding XML interfaces, but need some for parameters
#define __msxml_h__
#define __msxml2_h__
#define __msxml6_h__
struct IXMLElement {};
struct IXMLDOMDocument {};

struct tagSERIALIZEDPROPSTORAGE {}; //undefined in headers, and it is documented

//instaed of converting HWND and LPARAM etc to IntPtr(or int / uint, converter bug), convert to Wnd and IntLong
struct Wnd { void* x; };
#define HWND Wnd
struct IntLong { void* x; };
#define INT_PTR IntLong
#define UINT_PTR IntLong
#define LONG_PTR IntLong
#define ULONG_PTR IntLong
#define HANDLE_PTR IntLong
#define SHANDLE_PTR IntLong
#define size_t IntLong
typedef IntLong *PIntLong;
#define PINT_PTR PIntLong
#define PUINT_PTR PIntLong
#define PLONG_PTR PIntLong
#define PULONG_PTR PIntLong
#define PHANDLE_PTR PIntLong
#define PSHANDLE_PTR PIntLong

#define WIN32_LEAN_AND_MEAN
#include <windows.h>

#include <shlobj.h> //includes ole2, commctrl, several other
#include <shellapi.h>
#include <shlwapi.h>
#include <olectl.h>
#include <oleacc.h>
#include <mmsystem.h>
#include <tlhelp32.h>
#include <psapi.h>
#include <winperf.h>
#include <pdh.h>
#include <wtsapi32.h>
#define _WINSOCK_DEPRECATED_NO_WARNINGS //gcc cannot parse
#include <winsock2.h>
#include <uxtheme.h>
#include <vsstyle.h>
#include <dwmapi.h>
#include <htmlhelp.h>
#include <richedit.h>
#include <winioctl.h>
#include <lmcons.h> //for UNLEN and other constants
#include <aclapi.h>
#include <sddl.h>
#include <ktmw32.h>
#include <winnetwk.h>
//these (and some of the above) would be included if no WIN32_LEAN_AND_MEAN:
#include <cderr.h> //commdlg errors
#include <commdlg.h>
#include <dde.h>
#include <ddeml.h>
#include <winefs.h> //encrypted file system. .NET can encrypt/decrypt a file, but maybe this also can be useful
#include <winspool.h> //printer API. .NET has PrintDocument class, but maybe this also can be useful
//#include <wincrypt.h> //large. .NET has System.Security.Cryptography namespace
