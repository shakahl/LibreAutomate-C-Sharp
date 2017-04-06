out

str cppFile="Q:\app\Catkeys\Api\Api.cpp"
str outFile="Q:\app\Catkeys\Api\_Api.cpp"

str+ g_SDK_Include="Q:\SDK10\Include\10.0.10586.0\um" ;;note: need to copy all from 'shared' folder to this folder, or else would need to specify two directories
str+ g_VS_Include="Q:\SDK10\Include\10.0.10586.0\ucrt"
//str+ g_VS_Include="C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\include"
str incDir2="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\include" ;;vcruntime.h etc missing in SDK

int noWarnings(0) onlyDirectives(0) onlyMacros(0)

sub.CreateCppFile cppFile 1 0 onlyDirectives

str cl=
F
 C:\mingw\mingw64\bin\gcc.exe -E -P
 -I "{g_SDK_Include}" -I "{g_VS_Include}" -idirafter "{incDir2}"
 -nostdinc
 -undef
 {iif(noWarnings "-w" "")}
 {iif(onlyDirectives "-fdirectives-only" "")}
 -d{iif(onlyMacros "M" "D")}
 -fdollars-in-identifiers
 "{cppFile}"
 -o "{outFile}"
cl.findreplace("[]" " ")
out RunConsole2(cl)
 -U __STDC__ -D __STDC__=0

 RunConsole2 "C:\mingw\mingw64\bin\gcc.exe -E --help"


#sub CreateCppFile
function $cppFile !use64bit !useSAL !directivesOnly

str s.getmacro(getopt(itemid))
int i=find(s "#ret[]")+6; s.get(s i);; out s

 s.setfile(cppFile); ret

if(use64bit) s-"#define USE64BIT[]"
if(useSAL) s-"#define _PREFAST_ //SAL[]"
if(directivesOnly) s-"#define DIRECTIVESONLY[]"

ARRAY(str) a

if useSAL and !directivesOnly
	 insert driverspecs.h macros
	str d.getfile(F"{g_SDK_Include}\driverspecs.h")
	 out s
	if(!findrx(d "(?m)^[ \t]*#[ \t]*define\s+__drv_\w+[ \t]*(?:\([\w, ]+\)|$)" 0 4 a)) end "failed" 1
	d=""; for(i 0 a.len) d.addline(a[0 i])
	 out d
	s.findreplace("[_SAL_]" d 4)

 finally undef C macros defined here
if(!findrx(s "(?m)^[ \t]*#[ \t]*define\s+(\w+).+[]" 0 4 a)) end "failed" 1
s+"[][]"; for(i 0 a.len) s+F"#undef {a[1 i]}[]"

 out s; end
s.setfile(cppFile)

 ret

if !directivesOnly
	 also modify some header files where gcc and/or converter don't like
	str h=F"{g_SDK_Include}\wtypes.h"
	s.getfile(h)
	if(s.replacerx("(?m)^ *#define _VARIANT_BOOL\s+/##/")) s.setfile(h)
	str hfiles="oaidl[]propidl"
	foreach h hfiles
		h=F"{g_SDK_Include}\{h}.h"
		s.getfile(h)
		if(s.replacerx("(?m)^ *_VARIANT_BOOL[\s*]+p?bool\b.+[]")) s.setfile(h)


#ret
#define CATKEYS

//#define __cplusplus 199711 //gcc defines the same
#define _MSC_VER 1900 //VS2015
#define _MSC_FULL_VER 190023026
#define _MSC_EXTENSIONS 1
//#define _STDCALL_SUPPORTED ;;need if _MSC_VER < 800
#define _MT
#define _DLL
#define _UNICODE
#define UNICODE

#define WINVER 0x0A00 //Win10
#define _WIN32_WINNT 0x0A00
#define _WIN32_IE 0x0B00 //IE11
#define _CRTIMP __declspec(dllimport)

#define _WIN32 //defined for 32 and 64
#ifdef USE64BIT //converter hangs
#define _M_AMD64 100 //defined for 64
#define _WIN64 //defined for 64
//#define _M_X64 //not used in SDK headers
#else
#define _M_IX86 600 //defined for 32
#endif
#define _INTEGRAL_MAX_BITS 64 //need if 32
//#define __w64 //don't need

//gcc cannot preprocess driverspecs.h because of nonstandard use of ## operator. But need its macros, therefore we extract them and insert in place of [...] below.
#if defined(_PREFAST_) && !defined(DIRECTIVESONLY)
#define DRIVERSPECS_H
#define CONCURRENCYSAL_H //TODO
[_SAL_]
#endif

//prevent adding XML interfaces, but need some for parameters
#define __msxml_h__
#define __msxml2_h__
#define __msxml6_h__
struct IXMLElement{};
struct IXMLDOMDocument{};

struct tagSERIALIZEDPROPSTORAGE{}; //undefined in headers, and it is documented

//instaed of converting HWND and LPARAM etc to IntPtr (or int/uint, converter bug), convert to Wnd and IntLong
struct Wnd { void* x; };
#define HWND Wnd
struct IntLong { void* x; };
#define INT_PTR IntLong //TODO: remove (or ignore) 'typedef unsigned __int64 IntLong;' etc in basetsd.h
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
