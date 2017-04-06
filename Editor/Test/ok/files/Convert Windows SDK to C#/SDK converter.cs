 Preprocesses Windows SDK headers.
 The output files then can be used by the converter (my C# project "SdkConverter").

 The full sequence of actions:
   1. Once: run macros "SDK get dll names" and "SDK get GUID". They create database files used by the converter. They are very slow, unless using caching. Don't need to run them again when you add/remove header files.
   2. Run this macro. It creates two files (32-bit and 64-bit) for the converter.
   3. Run the converter. It also creates two files. Run it from Visual Studio, not as compiled exe, to be able to quickly modify its source code when something fails etc.
   4. Run macro "SDK append 32-bit diff". It creates final .cs file Api.cs.

 All temporary and output files are in folder "Q:\app\Catkeys\Api". Links to some of them added in this QM folder.


out

str clangPath="C:\Program Files\LLVM\bin\clang.exe"
 RunConsole2 F"''{clangPath}'' -help"; ret

 note: need to copy all from 'shared' folder to 'um' folder, else would need to specify two directories
str SDK_Include="Q:\SDK10\Include\10.0.10586.0\um"
 str SDK_Include="C:\Program Files (x86)\Windows Kits\10\Include\10.0.14393.0\um" ;;newer. Currently this macro does not work with it.
str CRT_Include="Q:\SDK10\Include\10.0.10586.0\ucrt"
str VS_CRT_Include="C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\include" ;;vcruntime.h etc missing in SDK

sub.Preprocess 1 "Q:\app\Catkeys\Api\Api-preprocessed-64.cpp"
sub.Preprocess 0 "Q:\app\Catkeys\Api\Api-preprocessed-32.cpp"


#sub Preprocess v
function !is64bit $outFile

 Preprocesses C++ headers from macro "SDK headers" and saves in file outFile.

 temp files
str cppFile="Q:\app\Catkeys\Api\Api-headers.cpp" ;;saved macro "SDK headers" text
str intermFile="Q:\app\Catkeys\Api\Api-interm.cpp" ;;first clang invocation output

 save macro "SDK headers" text to cppFile, with some modifications, etc
sub.CreateCppFile cppFile SDK_Include is64bit

 run clang to preprocess cppFile
str cl=
F
 "{clangPath}"
 -E -dD -undef
 -fms-compatibility -fms-extensions
 -fdeclspec -fdollars-in-identifiers
 -nobuiltininc -nostdinc++
 -Wno-extra-tokens -Wno-comment -Wno-nonportable-include-path -Wno-expansion-to-defined
 -I "{SDK_Include}" -I "{CRT_Include}" -idirafter "{VS_CRT_Include}"
 -o "{intermFile}"
 "{cppFile}"
cl.findreplace("[]" " ")
if(RunConsole2(cl)) end "failed" 1
#region some_clang_command_line_options
 -emit-ast
 -fdiagnostics-parseable-fixits
                          Print fix-its in machine parseable form
 -fdiagnostics-print-source-range-info
                          Print source range spans in numeric form
 -fdiagnostics-show-note-include-stack
                          Display include stacks for diagnostic notes
  -femit-all-decls        Emit all declarations, even if unused
  -ffreestanding          Assert that the compilation takes place in a freestanding environment
  -fgnu-keywords          Allow GNU-extension keywords regardless of language standard
  -fgnu89-inline          Use the gnu89 inline semantics
  -fms-compatibility-version=<value>
                          Dot-separated value representing the Microsoft compiler version number to report in _MSC_VER (0 = don't define it (default))
  -fmsc-version=<value>   Microsoft compiler version number to report in _MSC_VER (0 = don't define it (default))
  -fno-merge-all-constants
                          Disallow merging of constants
  -fno-show-column        Do not include column number on diagnostics
  -fno-show-source-location
                          Do not include source location information with diagnostics
  -fno-signed-char        Char is unsigned
  -fpack-struct=<value>   Specify the default maximum struct packing alignment
  -H                      Show header includes and nesting depth
  -idirafter <value>      Add directory to AFTER include search path
  -nobuiltininc           Disable builtin #include directories
  -nostdinc++             Disable standard #include directories for the C++ standard library
  -print-search-dirs      Print the paths used for finding libraries and programs
  -P                      Disable linemarker output in -E mode
  -Qunused-arguments      Don't emit warning for unused driver arguments
  -std=<value>            Language standard to compile for
  -stdlib=<value>         C++ standard library to use
  -undef                  undef all system defines
  -W<warning>             Enable the specified warning
  -w                      Suppress all warnings
  -x <language>           Treat subsequent input files as having type <language>
#endregion
 __________________________________

str s se

s.getfile(intermFile)

 tested:
 clang removes space/tab before and after # in directives.
 clang joins lines ending with \.
 clang uses \n for newlines, except in raw strings.
 clang replaces '#pragma  pack' to '#pragma pack', but does not change spaces after it.

 remove some #define etc that are added by clang or added here
if(s.replacerx("(?s)^.+?\n#define CATKEYS *\n" "" 4)<0) end "failed" 1

 remove SAL, CRT etc (use '# n "include file"' added by clang when not using -P)
s.replacerx("(?mi)(?>^# \d+ ''.+[/\\](?:specstrings\w*|\w*sal|\w+specs|winapifamily|winpackagefamily|stralign|ucrt\\[^\.'']+|VC\\\\include\\\\(?!excpt\.h)[^\.'']+)\.h'')(?s).+?(?=^# \d)")

 remove '# n "include file"' etc that clang adds when not using -P
if(!s.replacerx("(?m)^# \d.+\n")) end "failed" 1

 escape raw strings
REPLACERX rr.frepl=&sub.RxRawString
s.replacerx("R''(.*)\(((?s).*?)\)\1''" rr)

 remove empty lines to make easier to debug-read
if(!s.replacerx("(?m)^\s*\n")) end "failed" 1

 make #pragma pack easier to parse
if(!s.replacerx("(?m)^#pragma[ \t]+pack[ \t]*\(" "`(")) end "failed" 1

 remove #pragma that we don't need
if(!s.replacerx("(?m)^#pragma.+\n")) end "failed" 1

 remove thousand separator ' from number literals
s.replacerx("(?<=\d)'(?=\d)")

 replace double-word types, to make easier to convert
s.replacerx("\blong\s+long\b" "__int64")
s.replacerx("\blong\s+double\b" "double")
s.replacerx("\bsigned\s+\b(char|short|int|long|__int64|__int8|__int16|__int32)\b" "$1")
s.replacerx("\bunsigned\s+\b(char|short|int|long|__int64|__int8|__int16|__int32)\b" "u$$$1")

 convert DECLARE_HANDLE(X) to IntPtr
s.replacerx("\bstruct\s+(\w+)__\{int\s+unused;\};\s*typedef\s+struct \1__\s*\*\1;" "typedef IntPtr $1;")
 remove HWND typedef, we add it in converter
s.replacerx("\btypedef IntPtr HWND;" " ")

 replace inline functions LongToHandle/LongToPtr to IntPtr cast
s.replacerx("(?ms)^__inline\s+void\s*\*\s*(U?LongTo(?:Handle|Ptr))\s*\([^{]+\{[^}]+\}" "#define $1(h) (IntPtr)(h)")

 remove unused SAL annotations (defined as '^"text"; most removed by our #undef/#define)
rr.frepl=&sub.Callback_str_replacerx_sal
s.replacerx("\^''\w+''" rr)
s.replacerx("(\^''\w+'')(?:\s*\^''\w+'')+" "$1") ;;multiple-to-one (usually the first is what we need)

 manage #define/#undef
str sa
sub.DefineUndef s sa

 remove __declspec and other keywords that we don't need
foreach(_s "__unaligned[]__w64[]_w64[]static[]volatile[]explicit[]friend[]mutable[]__ptr32[]__ptr64[]constexpr[]thread_local[]__restrict[]restrict[]tile_static[]__clrcall[]__vectorcall") s.findreplace(_s "" 2)
s.replacerx("\balignas\s*\(\s*\w+\s*\)")
int n1 n2
n1=findrx(s "\b__declspec\s*\(\s*''" 0 4);; out n1 ;;295584 with SAL, 0 without
if(n1 and n1!=s.replacerx("(?s)\b__declspec\s*\(\s*''.*?''\s*\)")) end "failed" 1
n1=findrx(s "\b__declspec\s*\(\s*uuid\b" 0 4);; out n1 ;;713
if(n1) n2=s.replacerx("\b__declspec\s*\(\s*uuid\s*\(\s*(''[^'']+'')\s*\)\s*\)" "uuid($1)"); if(n2!=n1) end "failed" 1
n1=findrx(s "\b__declspec" 0 4);; out n1 ;;5574
if(n1) n2=s.replacerx("(?s)\b__declspec\s*\(\s*\w+\s*(?:\(\s*(?:\w+|''[^'']*'')\s*\))?\)"); if(n2!=n1) end "failed" 1

 make sure that uuid is not before struct, and remove from others
s.replacerx("\btypedef\s+(uuid\s*\(.+?\))\s*struct\b" "typedef struct $1 ")
s.replacerx("\btypedef\s+uuid\s*\(.+?\)" "typedef ")

 replace 'typedef TYPE [callConv] FUNCTYPE(' to 'typedef TYPE ([callConv]*FUNCTYPE)('
s.replacerx("(\btypedef\s+\w+[*&\s]+)(__?(?:stdcall|cdecl|fastcall|thiscall)\s+)?(\w+)\s*\(" "$1 ($2*$3)(")

 convert 'using X = Y' to 'typedef Y X', also for function typedefs. 0 in SDK.
 s.replacerx("\busing\s+(\w+)\s*=([\s\w,*&]+?);" "typedef $2 $1;")
 s.replacerx("\busing\s+(\w+)\s*=\s*([\s\w,*&]+?)\(([\s\w]*\*\s*)\)\s*\(" "typedef $2($3$1)(")

 add * to 'typedef T(__stdcall X)(...);' etc
ARRAY(str) aFN; int i
rr.frepl=&sub.Callback_str_replacerx_typedefFuncPtr
rr.paramr=&aFN
s.replacerx("\btypedef\s+\w+[\s*&]*\((?:_?_stdcall|_?_cdecl)\s+(\w+)\s*\)\s*\(" rr)
for(i 0 aFN.len) s.replacerx(F"(\btypedef\s+{aFN[i]})\s*\*" "$1 ") ;;remove * from 'typedef FUNC *LPFUNC;'

 remove 'void' from 'Func(void)'
s.replacerx("\(\s*void\s*\)(?=\s*[;=])" "()")

 convert 'X const *' to 'const X *' etc
s.replacerx("\b(\w+)\s+const\s*(?=\*|\w+\s*\[)" "const $1 ")
s.replacerx("\*\s*const\b" "**") ;;2

 remove 'public' from base (used for interface struct)
s.replacerx(":\s*public\s+(?=\w)" ":")

 somehow one '_VARIANT_BOOL bool;' not removed
s.replacerx("\b_VARIANT_BOOL bool;" " ")

 remove C_ASSERT
s.replacerx("\btypedef\s+char\s+__C_ASSERT__\b[^;]*;" " ")

 replace 'inline namespace' to 'namespace inline'
s.replacerx("\binline\s+namespace\b" "namespace inline") ;;0 in SDK

 replace 2 __stdcall to 1
s.replacerx("\b__stdcall\s+__stdcall\b" "__stdcall") ;;4 in SDK

 resolve identifier conflicts where the same name is defined in two header files
s.replacerx("(?m)^typedef PDH_HLOG HLOG;") ;;the typedef is not used

 replace 'TYPE __forceinline' to '__forceinline TYPE'
s.replacerx("(?m)LONGLONG\s+__forceinline\b" "__forceinline LONGLONG")

 replace some code that cannot be converted
s.findreplace(" rgbIndexId[sizeof(JET_API_PTR)+sizeof(u$long)+sizeof(u$long)]" " rgbIndexId[ 24 ]") ;;last member in struct that is only used as pointer parameter

 remove classes, because converter does not convert or skip them
s.replacerx("(?ms)^class PMSIHANDLE\s*\{.+?^\};")

 convert property keys
sa.replacerx("(?m)^`d\$\$\$_INIT_(PKEY_\w+) +\{ *\{ *([^\}]+) *\} *, *(\w+) *\} *$" "`cp ''internal static PROPERTYKEY $1 = new PROPERTYKEY() { fmtid = new Guid($2), pid = $3 };''")
s.replacerx("(?m)^extern ''C'' const PROPERTYKEY PKEY_\w+;$")

 __________________________________

 run clang again on #define/#undef to expand macros in their values

str defFileIn.expandpath("$temp qm$\sdk def in.cpp")
str defFileOut.expandpath("$temp qm$\sdk def out.cpp")
sa.setfile(defFileIn)

cl=
F
 "{clangPath}"
 -E -P -undef
 -fms-compatibility -fms-extensions
 -fdeclspec -fdollars-in-identifiers
 -nobuiltininc -nostdinc++
 -o "{defFileOut}"
 "{defFileIn}"
cl.findreplace("[]" " ")
if(RunConsole2(cl)) end "failed" 1

sa.getfile(defFileOut)
s.addline(sa)


 replace known sizeof(string)
s.findreplace("sizeof(''://'')" "4" 2|64)
s.findreplace("sizeof(L''\\TransactionManager\\'')" "42" 2|64)
s.findreplace("sizeof(L''\\Transaction\\'')" "28" 2|64)
s.findreplace("sizeof(L''\\Enlistment\\'')" "26" 2|64)
s.findreplace("sizeof(L''\\ResourceManager\\'')" "36" 2|64)

 replace known ?: operators like '((1 != 0) ? 0x00010000 : 0)'
s.replacerx("\(\(0 != 0\) \? \w+ : (\w+)\)" "($1)")
s.replacerx("\(\(1 != 0\) \? (\w+) : \w+\)" "($1)")


s.setfile(outFile)

out F"PREPROCESSED {iif(is64bit `64` `32`)}-bit"


#sub CreateCppFile
function $cppFile $SDK_Include !is64bit

str s.getmacro("SDK headers")
 s.setfile(cppFile); ret

if(is64bit) s-"#define USE64BIT[]"

 finally undef C macros defined here
ARRAY(str) a; int i
if(!findrx(s "(?m)^[ \t]*#[ \t]*define\s+(\w+).*[]" 0 4 a)) end "failed" 1
s+"[][]"; for(i 0 a.len) s+F"#undef {a[1 i]}[]"

 out s; end
s.setfile(cppFile)

 SAL
str catsal.getpath(cppFile "\catkeys-sal.h")
s=
 #define __SPECSTRINGS_STRICT_LEVEL 0 //prevent redefining _Outptr_ etc to something useless
 #undef _SA_annotes3
 #define _SA_annotes3(n,pp1,pp2,pp3) ^pp1
 //only _SA_annotes3 is used
 //#undef _SA_annotes1
 //#define _SA_annotes1(n,pp1) ^pp1
 //#undef _SA_annotes2
 //#define _SA_annotes2(n,pp1,pp2) ^pp1
;
s.setfile(catsal)

str sal
sal.from(SDK_Include "\sal.h")
s.getfile(sal)
if(find(s "CATKEYS")<0)
	str incl=F"#ifdef CATKEYS[]#include ''{catsal}''[]#endif[]"
	if(s.replacerx("(?m)^#define _SA_annotes3\(n,pp1,pp2,pp3\)[]" F"$0{incl}" 4)<0) end "failed" 1
	s.setfile(sal)

str appmodel=F"{SDK_Include}\appmodel.h"
if !FileExists(appmodel)
	FileCopy F"{SDK_Include}\..\winrt\appmodel.h" appmodel
	FileCopy F"{SDK_Include}\..\winrt\minappmodel.h" F"{SDK_Include}\minappmodel.h"


#sub RxRawString
function# REPLACERXCB&x

out F"<>info: raw string. Please review because extracting raw strings is unreliable:[]<c 0x4080>{x.match}</c>"

str s.get(x.subject x.vec[2].cpMin (x.vec[2].cpMax-x.vec[2].cpMin))
s.findreplace("\" "\\")
s.findreplace("''" "\''")
s.findreplace("[10]" "\n")
s.findreplace("[13]" "\r")
 out s
x.match.from("''" s "''")


#sub Callback_str_replacerx_typedefFuncPtr
function# REPLACERXCB&x

ARRAY(str)& aFN=+x.rrx.paramr

 out x.match
int offs=x.vec[0].cpMin
int i(x.vec[1].cpMin-offs) j(x.vec[1].cpMax-offs)
x.match[i-1]='*'
aFN[].get(x.match i j-i)


#sub DefineUndef
function str&s str&sa

 Extracts all #define/#undef, removes from s, and adds to sa.
 Also appends to sa '`d$$$NAME VALUE'. The second clang call will unexpand C macros in values.

ARRAY(str) a; int i
str rx="(?m)^#(d|u)(?:efine|ndef)[ \t]+([\w\$]+\b)(.*\n)"
if(!findrx(s rx 0 4 a) or !s.replacerx(rx)) end "failed" 1

for(i 0 a.len) sa+a[0 i]
for(i 0 a.len) sa+F"`{a[1 i]}$$$_{a[2 i]}{a[3 i]}"

 info:
 We add '`d$$$NAME VALUE' at the end, not by each #define, because:
 1. Some constants are used in values of other constants before defining. Example:
    #define X2 X1+1
    #define X1 5
 2. It is how C++ compiler works. Example:
    #define X1 5
    #define X2 X1+1
    #undef X1
    #define X1 10
    ...
    Out(X2); //11, not 6. And error if we undef X1.


#sub Callback_str_replacerx_sal
function# REPLACERXCB&x

sel x.match 2
	case ["^''_In_*","^''_Inout_*","^''_Out_*","^''_Outptr_*"]
	case "^''__RPC_*"
	sel x.match+8 2
		case ["*_in''","*_in_*"] x.match.insert("_In_" 2)
		case ["*_inout''","*_inout_*"] x.match.insert("_Inout_" 2)
		case ["*_deref_out''","*_deref_out_*"] x.match.insert("_Outptr_" 2)
		case ["*_out''","*_out_*"] x.match.insert("_Out_" 2)
		case ["unique_pointer*","string*"] ret 1 ;;filter garbage to make easier to debug
		case else goto ge
	case "^''__*"
	sel x.match+4 2
		case "out_data_source*" ret 1
		case ["in''","in_*"] x.match.insert("_In_" 2)
		case ["inout''","inout_*"] x.match.insert("_Inout_" 2)
		case ["out''","out_*"] x.match.insert("_Out_" 2)
		case ["field_*","drv_*","kernel_entry*","control_entrypoint*"] ret 1 ;;filter garbage to make easier to debug
		case else goto ge
	case ["^''_COM_Outptr_*"] x.match.insert("_Outptr_" 2)
	case ["^''_Deref_*_range_''"] ret 1 ;;used only after _Out_ etc, therefore useless and would be removed later anyway
	case ["^''_Null*","^''_Ret*","^''_Field_*","^''_Reserved_*","^''_Success_*","^''_Must_*","^''_Post_*","^''_Pre_*","^''_Printf_*","^''_Struct_*","^''_Check_return_*","^''_Function_class_*","^''_Frees_ptr_*","^''_Analysis_noreturn_*","^''_IRQL_requires_*","^''_Strict_type_match*","^''_Translates_Win32_to_HRESULT_*"] ret 1 ;;filter garbage to make easier to debug
	case else
	 ge
	out x.match
	ret 1


#sub Callback_str_replacerx_sal2
function# REPLACERXCB&x

