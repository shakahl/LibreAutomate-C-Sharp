out

out
 str sigimp="$program files$\InteropSignatureToolkit\sigimp.exe"
str sigimp="Q:\Downloads\PInvokeTool\UnmanagedToManaged\ConsoleTool\bin\Release\sigimp.exe"
str cppFile="Q:\app\Catkeys\Api\_Api.cpp"

sub.Preprocess cppFile

str cl=
F
 "{sigimp}" /nologo /lang:cs
 /out:"Q:\app\Catkeys\Api\_Api.cs"
 "{cppFile}_preproc.cpp"

cl.findreplace("[]" " ")
int R=RunConsole2(cl _s)
if _s.len
	_s.replacerx("(?m)^Warning:.+[]") ;;many inline functions and several vararg functions
	_s.replacerx("(?m)^Error: Error processing procedure \w+: Expected token of type ParenClose but found IntKeyword instead\.[]") ;;macro DEFINE_ENUM_FLAG_OPERATORS, not useful
	_s.replacerx("(?m)^Error: Error processing procedure \w+: Expected token of type ParenClose but found Ampersand instead\.[]") ;;inline functions, not useful
out _s
out R
 /includePath:"{g_SDK_Include}","{g_VS_Include}"


#ret

 note:
 Must NOT exist <sigimp.exe folder>\Data\windows.xml. Else removes most that is in it.

 note:
 There are about 100 errors and warnings when just #include <windows.h>. Never mind. Some useless errors/warnings now hidden.


 sigimp [options] [Header File Names]
	/genCode:[yes/no]	Whether or not to generate a code file (default yes)
	/genPreProc:[yes/no]	Generate the preprocessor code
	/lang[uage]:lang	Language to generate into; vb (default) or cs
	/out:filename		Output file name (default NativeMethods.vb)
	/useSdk:[yes/no]	Whether or not to add common SDK include paths (default yes)
	/lib:name,name		List of libraries to resolve DLL's against
	/includePath:path,...	Include File Path
	/nologo			Prevent logo display


 s.getfile(cs)
 out findw(s "struct IXMLElementVtbl")
 out findw(s "AccessibleObjectFromWindow")
 out findrx(s "\w_Stub\b")


#sub Preprocess
function $cppFile

str s.getfile(cppFile)

 remove some to make simpler
s.replacerx("\b__declspec\((?:nothrow|novtable|deprecated)\)" " ")
s.replacerx("\[(?:SA_|return|source|repeat).+?\]" " ")

 convert interfaces to functions, and add begin/end markers, because converter does not support it
 str rx="(?m)^[ \t]*struct\s+(\w+)\s*(?::\s*(?:public\s+)?(\w+)\s*)?\{\s*(?:public\s*:\s+)?(virtual\s(?s).+?\)\s*=\s*0\s*;)\s*\}\s*;" ;;note: assume there are no 'typedef struct...'
str rx=
 (?xm)^[\ \t]*struct\s+
 (?:__declspec\(uuid\("    (.+?)    "\)\)\s*)?
 (\w+)  \s*    #name
 (?: :\s*(?:public\s+)?  (\w+)  \s*)?    #base
 \{\s*  (?:public\s*:\s+)?
 (virtual\s(?s).+?\)\s*=\s*0\s*;)\s*
 (?:\}\s*;|template)    #some interfaces contain a template member function after virtual functions

 ARRAY(str) a
 out findrx(s rx 0 4 a)
 int i
 for i 0 a.len
	 out "<><Z 0x80c080></Z>"
	 out a[0 i]

REPLACERX rr
rr.frepl=&sub.replacerxInterface
IStringMap m._create; rr.paramr=&m
 PF
int nMustBe=findrx(s "\bstruct\s+[^{};*]+\{[\s\w:]*\bvirtual\b" 0 4)
 out nMustBe ;;540
if(nMustBe<500) end "failed" 1
 if(nMustBe!=s.replacerx(rx)) end "failed" 1
 end
if(nMustBe!=s.replacerx(rx rr)) end "failed" 1
 PN;PO

 converter bug: drops functions that have & parameters. Replace & to *.
if(!s.replacerx("(\b[A-Za-z_]\w+\s*)&\s*([A-Za-z_]\w+\b)(?=\s*(?:,|\)\s*;))" "$1* $2")) end "failed" 1
if(!s.replacerx("(?m)^([ \t]*typedef\s[\w\s]+?)&" "$1")) end "failed" 1

 replace typedef to #define, to remove A struct/delegate later, because typedefs are not preserved in converter output. Some use #define, some typedef.
if(!s.replacerx("(?ms)^[ \t]*typedef\s+(\w+)W\s+\1\b(.*?);" "#define $1 $1W$2")) end "failed" 1

 converter bug: drops struct that have base struct specified. Extract base members and add to current struct.
rr.frepl=&sub.replacerxStruct
if(!s.replacerx("(\bstruct\s+\w+)\s*:\s*(?:public\s+)?(\w+)\s*\{\s*" rr)) end "failed" 1

s.setfile(F"{cppFile}_preproc.cpp")
 end


#sub replacerxInterface
function# REPLACERXCB&x

lpstr s=x.subject
POINT* p=x.vec
str guid name.get(s p[2].x (p[2].y-p[2].x)) base

 remove known A interfaces
 if(name.end("A")) out name
sel(name) case ["IShellLinkA","IExtractIconA","INewShortcutHookA","ICopyHookA"] ret 1

 get guid
if(p[1].y>0) guid.get(s p[1].x (p[1].y-p[1].x))

 get members
str members.get(s p[4].x (p[4].y-p[4].x))
 unwrap etc, to make nicer and smaller when developing this macro
members.replacerx("(?<=[\(,])[][ \t]+" " ")
members.replacerx("\b__stdcall\s*" " ")

 get base
if(p[3].y>0) base.get(s p[3].x (p[3].y-p[3].x))
else if(name="IUnknown") x.match="[]struct IUnknown{int _R_E_M_O_V_E_;};[]"; ret ;;we don't need IUnknown members
else base="IUnknown" ;;IUnknown members declared here, will need to remove

 remove IUnknown members, because don't need them in C#. Some interface declarations include base interface members even if ':IBase' specified.
 out members
int iUnkRem=members.replacerx("^virtual\s+HRESULT\s+QueryInterface\s*\(.+[]\s*virtual\s+ULONG\s+AddRef\s*\(.+[]\s*virtual\s+ULONG\s+Release\s*\(.+[]" "" 4)
 if(iUnkRem=0) out name

 remove 'virtual' and '=0'
lpstr rx="(?s)[ \t]*\bvirtual\s+(.+?\))\s*=\s*0\s*;" ;;out findrx(members rx 0 4)
int n=members.replacerx(rx "$1;"); if(!n) end "failed" 1

 add base members
IStringMap& m=+x.rrx.paramr
if base!="IUnknown"
	lpstr baseMembers=m.Get(base); if(!baseMembers) end F"failed, this={name} base={base}" 1
	int nbase=val(baseMembers+2 0 _i); if(nbase) baseMembers+_i+4; else end "failed" 1
	 don't add if declared here
	str firstBaseMember; if(findrx(baseMembers "^\w+[\s*]+(\w+)\s*\(" 0 0 firstBaseMember 1)<0) end "failed" 1
	if findrx(members F"^\w+[\s*]+{firstBaseMember}\s*\(")
		n+nbase
		members.from(baseMembers "[]" members)
m.Add(name F"//{n}[]{members}")

 out F"<><Z 0x80c080>{name} : {base}    '{guid}'</Z>[]{members}"

 assert that does not have duplicate base members
if(0=findrx(members "^\w+[\s*]+(\w+)\s*\((?ms).+?^\w+[\s*]+\1\s*\(")) end "failed" 1

 prepend interface name to member names, because it seems that converter removes duplicates
rx="(?m)^(\w+[\s*]+)(\w+\s*\()"
if(n!=members.replacerx(rx F"$1_{name}_$2")) end F"failed: n={n}, now={findrx(members rx 0 4)}" 1

_s=
F
;
 struct {name}{{int _R_E_M_O_V_E_;};
 void interface_begin_{name}(int _G_U_I_D_{guid.encrypt(8)});
 {members}
 void interface_end_{n}_{name}();
;
x.match.swap(_s)
 info: added struct because converter may need interfaces as parameter types etc
 out x.match


#sub replacerxStruct
function# REPLACERXCB&x

lpstr s=x.subject
POINT* p=x.vec
str s1.get(s p[1].x (p[1].y-p[1].x)) base.get(s p[2].x (p[2].y-p[2].x))

 out base

str baseMembers
if findrx(x.strnew F"\bstruct\s+{base}\s*\{{([^{{}]+)\}" 0 0 baseMembers 1)>=0
	 out baseMembers
	x.match=F"{s1} {{ {baseMembers}[]"
else
	out F"<not found base struct {base}>" ;;eg base contains nested struct, ie {}
	x.match=F"{s1} {{ {base} _;[]"

