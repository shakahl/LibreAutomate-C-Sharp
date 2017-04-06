 ALGORITHM

 Join all direct and indirect include files into single file (FILE1) with gcc -fdirectives-only.
 Load FILE1 and get all #define with regex.
 From all the #define format const so that VC preprocessor will expand macros in their values.
 Append the const to FILE1 data and save in FILE1.
 Preprocess FILE1 with the SDK VC compiler and save in FILE2.
   It expands macros of the added const.
   Don't use gcc for it because it fails to preprocess SDK (or need to modify it etc).
 Then convert everything to C# with our converter.

out

str cppFile="Q:\app\Catkeys\Api\Api.cpp"
str gccOutFile="Q:\app\Catkeys\Api\Api-joined.cpp"
str outFile="Q:\app\Catkeys\Api\Api-preprocessed.cpp"

str SDK_Include="Q:\SDK10\Include\10.0.10586.0\um" ;;note: need to copy all from 'shared' folder to this folder, or else would need to specify two directories
str CRT_Include="Q:\SDK10\Include\10.0.10586.0\ucrt"
str VS_CRT_Include="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\include" ;;vcruntime.h etc missing in SDK

str gccPath="C:\mingw\mingw64\bin\gcc.exe"
str clPath="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\cl.exe"

str s se

sub.CreateCppFile cppFile 1 0 0

 join all header files with gcc
str cl=
F
 "{gccPath}" -E
 -x c++
 -I "{SDK_Include}" -I "{CRT_Include}" -idirafter "{VS_CRT_Include}"
 -nostdinc -nostdinc++
 -undef -fdirectives-only
 -fdollars-in-identifiers
 -o "{gccOutFile}"
 "{cppFile}"
cl.findreplace("[]" " ")
if(RunConsole2(cl)) end "failed"
 -P ;;removes #n "include file" etc

 process #define etc
s.getfile(gccOutFile)

 unindent etc
s.replacerx("(?m)^[ \t]*#[ \t]*" "#")
 tested: gcc joins lines ending with \

 remove some #define to avoid cl warnings. Some are always added by gcc, some added here because need for gcc.
if(!s.replacerx("(?m)^#define[ \t]+(?:__STDC__|__cplusplus|__STDC_HOSTED__|_MSC_VER|_MSC_FULL_VER|_MSC_EXTENSIONS|_WCHAR_T_DEFINED|_MT|_DLL|_WIN32)\b[^\r\n]*$")) end "failed"

 preserve #define and #undef
int nConst nFunc nUndef
nConst=s.replacerx("(?m)^#define[ \t]+(\w+)[ \t]+(\S[^\r\n]*) *$" "@^c____$1 $1[]$0")
nFunc=s.replacerx("(?m)^#define[ \t]+(\w+\(.+?\))[ \t]*(\S[^\r\n]*) *$" "@^f____$1 $1[]$0")
nUndef=s.replacerx("(?m)^#undef[ \t]+(\w+) *$" "@^u____$1[]$0")
 out F" {nConst} {nFunc} {nUndef}"
 28514 2609 605
if(!nConst or !nFunc or !nUndef) end "failed"

 mark to remove garbage (use '#n "include file"' added by gcc when not using -P)
int n1=s.replacerx("(?m)^#\d+ ''.+[/\\](?:specstrings\w*|sal|ConcurrencySal|\w*driverspecs|winapifamily|winpackagefamily|ucrt/\w+|VC\\\\include/\w+)\.h''(?s).+?(?=^#\d)" "[2][]$0[3][]")
 out n1
 remove '#n "include file"' etc that gcc adds when not using -P
if(!s.replacerx("(?m)^#\d.+[]")) end "failed"

 escape raw strings
REPLACERX rr.frepl=&sub.RxRawString
s.replacerx("\b(L|U|u8?)?R''(.*)\(((?s).*?)\)\2''" rr)

s.setfile(gccOutFile)
ret

 precompile with cl
cl=
F
 "{clPath}"
 /nologo /MD
 /EP
 "{gccOutFile}"

cl.findreplace("[]" " ")
if(RunConsole3(cl s se)) end "failed"

 remove the marked garbage
if(!s.replacerx("(?ms)^[2].+?^[3]")) end "failed"

 remove empty lines to make easier to debug-read
if(!s.replacerx("(?m)^\s*[]")) end "failed"

 make #pragma pack easier to parse
if(!s.replacerx("(?m)^#pragma[ \t]+pack[ \t]*\(" "@(")) end "failed"

 remove other #pragma, because can be eg inside a struct, making difficult to parse later
if(!s.replacerx("(?m)^#.+[]")) end "failed"

 move our marked #define/#undef to the end, because can be inside a struct etc
str rx="(?m)^@\^.+[]"
ARRAY(str) a; int i
if(!findrx(s rx 0 4 a) or !s.replacerx(rx)) end "failed"
str sa="[]"; for(i 0 a.len) sa+a[0 i]
s+sa

 remove thousand separator ' from number literals
s.replacerx("(\d)'(\d)" "$1$2")

 out s
if(se.len>10) out se
s.setfile(outFile)


#sub CreateCppFile
function $cppFile !use64bit !useSAL !ignoreSAL

str s.getmacro(getopt(itemid))
int i=find(s "#ret[]")+6; s.get(s i);; out s

 s.setfile(cppFile); ret

if(use64bit) s-"#define USE64BIT[]"
if(useSAL) s-"#define _PREFAST_[]"
if(ignoreSAL) s-"#define SPECSTRINGS_H[]"

 finally undef C macros defined here
ARRAY(str) a
if(!findrx(s "(?m)^[ \t]*#[ \t]*define\s+(\w+).+[]" 0 4 a)) end "failed" 1
s+"[][]"; for(i 0 a.len) s+F"#undef {a[1 i]}[]"

 out s; end
s.setfile(cppFile)


#sub RxRawString
function# REPLACERXCB&x

out x.match


#ret

#define RAW1 R"(raw1)"
#define RAW2 R"(ra"w1)"
#define RAW3 R"(ra
w2)"


#define CATKEYS

//#define __cplusplus 199711 //gcc defines the same
#define _MSC_VER 1900 //VS2015
#define _MSC_FULL_VER 190023026
#define _MSC_EXTENSIONS 1
//#define _STDCALL_SUPPORTED ;;need if _MSC_VER < 800

#define _UNICODE
#define UNICODE
#define _WCHAR_T_DEFINED
#define _MT
#define _DLL

#define _WIN32 //defined for 32 and 64
#define WINVER 0x0A00 //Win10
#define _WIN32_WINNT 0x0A00
#define _WIN32_IE 0x0B00 //IE11
#define _CRTIMP __declspec(dllimport)

#ifdef USE64BIT
#define _M_AMD64 100 //defined for 64
#define _WIN64 //defined for 64
//#define _M_X64 //not used in SDK headers
#else
#define _M_IX86 600 //defined for 32
#endif
#define _INTEGRAL_MAX_BITS 64 //need if 32
//#define __w64 //don't need

//prevent adding XML interfaces, but need some for parameters
#define __msxml_h__
#define __msxml2_h__
#define __msxml6_h__
struct IXMLElement{};
struct IXMLDOMDocument{};

struct tagSERIALIZEDPROPSTORAGE{}; //undefined in headers, and it is documented

//instaed of converting HWND and LPARAM etc to IntPtr (or int/uint, converter bug), convert to Wnd and LPARAM
struct Wnd { void* x; };
#define HWND Wnd
struct LPARAM { void* x; };
#define INT_PTR LPARAM //TODO: remove (or ignore) 'typedef unsigned __int64 LPARAM;' etc in basetsd.h
#define UINT_PTR LPARAM
#define LONG_PTR LPARAM
#define ULONG_PTR LPARAM
#define HANDLE_PTR LPARAM
#define SHANDLE_PTR LPARAM
#define WPARAM LPARAM
#define LPARAM LPARAM
#define LRESULT LPARAM
#define SIZE_T LPARAM
#define SSIZE_T LPARAM
#define size_t LPARAM
#define intptr_t LPARAM
#define uintptr_t LPARAM
typedef LPARAM *PLPARAM;
#define PINT_PTR PLPARAM
#define PUINT_PTR PLPARAM
#define PLONG_PTR PLPARAM
#define PULONG_PTR PLPARAM
#define PHANDLE_PTR PLPARAM
#define PSHANDLE_PTR PLPARAM
#define PSIZE_T PLPARAM
#define PSSIZE_T PLPARAM

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

//#include <no_sal2.h> //TODO: test, maybe need this
