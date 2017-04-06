out

 common variables
int i j
 lpstr rx
REPLACERX rr
str& r

str s.getfile("Q:\app\Catkeys\Api\_Api.cs")

 correct newlines
s.replacerx("(?<!\r)\n" "[]") ;;0
s.replacerx("\r(?!\n)" "[]") ;;0

 trim space
if(!s.replacerx("(?m) +$")) end "failed" 1
if(!s.replacerx("(?m) +;$" ";")) end "failed" 1
if(!s.findreplace("[][][]" "[][]" 8)) end "failed" 1

 get constants, types, methods, util
str c t m util ;;constants, types, methods, utils
ARRAY(str) actmu
_s=
 (?ms)^public partial class NativeConstants \{
 (.+?)^\}
 (
 (?:public enum|\[System\.Runtime\.InteropServices\.StructLayout).+?
 )
 public partial class NativeMethods \{
 (.+)^\}
 (
 public class .+)
if(findrx(s _s 0 0 actmu)<0) end "failed" 1
c.swap(actmu[1])
t.swap(actmu[2])
m.swap(actmu[3])
util.swap(actmu[4])

#region CONSTANTS

 out findrx(c "(?m)^ *public const " 0 4)

 unwrap lines
if(!c.replacerx("[]         *" " ")) end "failed"
 unindent
if(!c.replacerx("(?m)^ +")) end "failed"

 get aliases like Func -> FuncW, STRUCT -> STRUCTW
IStringMap map; sub.GetAliases c map

 remove errors and warnings
if(!c.replacerx("(?m)^[]///.+[]/// Error generating expression: .+[](?:/.+[])*public const string .+[]")) end "failed"
if(!c.replacerx("(?m)^[]/// Warning: .+[](?:/.+[])*public const string .+[]")) end "failed"
 remove NativeConstants.
if(!c.replacerx("\bNativeConstants\.")) end "failed"

 restore simple 0x uint constants
rr.frepl=&sub.replacerxConstHex
if(!c.replacerx("(?m)^///.+-> .*?0x\w.*[]public const int (\w+ = )(-?\d+);" rr)) end "failed" ;;const
 restore expression 0x uint constants like (OTHER_CONST | 32), also with <<, >>, WM_USER +, _FIRST +
if(!c.replacerx("(?m)^public const (int \w+ = .+?(?:\||<<|>>|WM_USER \+|(?<!_E)_FIRST \+))" "public const u$1")) end "failed"
 restore expression 0x uint constants like OTHER_CONST, because most of them are uint
if(!c.replacerx("(?m)^public const (int \w+ = [A-Z]\w+)" "public const u$1")) end "failed"
 note: some int/uint will be incorrect.

 remove comments
if(!c.replacerx("(?m)/// (\w+) ->.+[](public const \w+ @?\1\b)" "$2")) end "failed"

 remove @__ garbage
if(!c.replacerx("(?m)^public const \w+ @__.+[][]")) end "failed"
 remove some SAL left (most removed together with errors)
if(!c.replacerx("(?m)^public const \w+ _\w+_ = _.+[][]")) end "failed"

 remove empty lines between const with same prefix
if(!c.replacerx("(?m)(^public const \w+ \w+?_)(\w+.+[])[](?=\1)" "$1$2")) end "failed"

#endregion

#region CLEAN TYPES AND METHODS

 unindent methods
if(!m.replacerx("(?m)^    ")) end "failed" 1

str rxCommAttr="[](?:///.+[])*(?:\[.+[])+" ;;0 or more lines of doc comments and 1 or more lines of attributes
&r=t
for i 0 2
	 out i
	if(!r.replacerx("\bSystem\.[\w\.]+?\.(\w+)Attribute\b(\(\))?" "$1")) end "failed" 1
	if(!r.replacerx("\bSystem\.Runtime\.InteropServices\.\b")) end "failed" 1
	if(!r.replacerx("\bSystem.Text\.")) end "failed" 1
	if(!r.replacerx("\bSystem.IntPtr\b" "IntPtr")) end "failed" 1
	 remove MarshalAs(UnmanagedType.LPWStr|Bool)
	if(!r.replacerx("\[MarshalAs\(UnmanagedType\.(?:LPWStr|Bool)\)\] (string|bool|StringBuilder)" "$1")) end "failed" 1 ;;param
	if(i=0 and !r.replacerx("(?m)^ +\[MarshalAs\(UnmanagedType\.(?:LPWStr|Bool)\)\][]( +public (?:string|bool) )" "$1")) end "failed" 1 ;;field
	  replace IntPtr to Wnd where need
	 if(i=0 and !t.replacerx(F"(?m)^ */// HWND->HWND__\*[]( *public) IntPtr " "$1 Wnd ")) end "failed" 1
	 if(i=1 and !m.replacerx(F"(?m)(^/// *Return Type: HWND->HWND__\*{rxCommAttr}public static extern +)IntPtr\b" "$1Wnd")) end "failed" 1
	 if(i=1 and !m.replacerx(F"(?m)(^///(\w+): HWND->HWND__\*{rxCommAttr}public static extern .+)\bIntPtr \2" "$1Wnd $2")) end "failed" 1
	 remove 'tag' prefix from STRUCT and ENUM.  note: the converter removes NAME if it does not match the tagNAME, eg struct tagNMCUSTOMDRAWINFO{...}NMCUSTOMDRAW; becomes tagNMCUSTOMDRAWINFO and there is no NMCUSTOMDRAW
	if(!r.replacerx("\btag([A-Z]\w+)" "$1")) end "failed" 1
	 converter adds @ prefix to names that begin with __
	if(!r.replacerx("@(__\w+)" "$1")) end "failed"
	
	&r=m

#endregion

#region TYPES

 remove struct attributes
if(!t.findreplace("[][StructLayout(LayoutKind.Sequential)]")) end "failed" 1
if(!t.findreplace("[][StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]")) end "failed" 1
 remove delegate attributes
if(!t.findreplace("[][UnmanagedFunctionPointer(CallingConvention.StdCall)]")) end "failed" 1

ARRAY(str) ae at ad; str se st sd ;;enum, struct, delegate

 get enum array
if(!findrx(t "(?ms)^public enum (\w+) \{.+?^}[]" 0 4 ae)) end "failed" 1
 out ae.len
if(ae.len!=findrx(t "(?m)^public enum " 0 4)) end "failed"

 get struct array
if(!findrx(t "(?m)^(?>(?:\[.+[])*public struct (\w+) \{)(?s).+?^}[]" 0 4 at)) end "failed" 1
 out at.len
if(at.len!=findrx(t "(?m)^public struct " 0 4)) end "failed" 1

 get delegate array
if(!findrx(t "(?m)^/// Return Type: .+(?:[][\[/].+)*[]public delegate \w+ (\w+)\(.+?[]" 0 4 ad)) end "failed" 1
 out ad.len
if(ad.len!=findrx(t "(?m)^public delegate " 0 4)) end "failed" 1

 ENUM

 out ae.len
for i 0 ae.len
	&r=ae[0 i]
	 out r
	rr.frepl=&sub.replacerxEnumHex
	if r.replacerx("(?m)^ */// (\w+) -> .*?0x\w.*[]( *\1 = )(-?\d+)," rr)
		r.findreplace("{" " :uint {" 4)
		r-"[Flags][]"
	r.replacerx("(?m)^ *///.+[]") ;;remove comments
	r.replacerx("[]([] *)" "$1") ;;remove empty lines
	 out r
	se.addline(r)
 out se

 STRUCT

 remove A structs
sub.ApplyAliases at map 4

for i 0 at.len
	&r=at[0 i]
	 out r
	 out r
	st.addline(r)
 out st

 correct incorrectly converted types
sub.CorrectTypes st 1

#region remove some comments
str comm=
 DWORD->unsigned int
 WORD->unsigned short
 BYTE->unsigned char
 DWORD64->unsigned __int64
 LONG->int
 ULONG->unsigned int
 ULONG64->unsigned __int64
 USHORT->unsigned short
 UINT->unsigned int
 UINT32->unsigned int
 UINT64->unsigned __int64
 UINT16->unsigned short
 SHORT->short
 BOOL->int
 LPWSTR->WCHAR*
 LPCWSTR->WCHAR*
 PCWSTR->WCHAR*
 PWSTR->WCHAR*
 LPCTSTR->LPCWSTR->WCHAR*
 OLECHAR*
 LPOLESTR->OLECHAR*
 FLOAT->float
 unsigned char
 INT->int
 int
 unsigned int
 short

foreach(_s comm) if(!st.findreplace(F"[]    /// {_s}[]" "[]")) end F"failed, {_s}"
if(!st.replacerx("(?m)^ */// (\w+)->_?\1[]")) end "failed"
if(!st.replacerx("(?m)^ */// Anonymous_\w+[]")) end "failed"
 if(!st.replacerx("(?m)^ */// H(\w+)->H\1__\*[]")) end "failed" ;;don't
#endregion

 DELEGATE

 remove A delegates
sub.ApplyAliases ad map 3

for i 0 ad.len
	&r=ad[0 i]
	sd.addline(r)
 out sd

 delegate param StringBuilder -> string, unless with an attribute
if(!sd.replacerx("(?<!\] )\bStringBuilder\b" "string")) end "failed"

 correct incorrectly converted types
sub.CorrectTypes sd 0

#endregion

#region METHODS, WRAPPERS

if(!m.replacerx(", CallingConvention=CallingConvention\.StdCall\b")) end "failed" ;;__stdcall is default, why converter adds it
if(!m.replacerx("(?m)^public static extern  " "public static extern ")) end "failed" ;;converted adds two spaces after 'extern'
if(!m.replacerx("(?m)^\[return: MarshalAs\(UnmanagedType\.(?:LPWStr|Bool)\)\][]")) end "failed" ;;remove MarshalAs where it is implicit
 if(!m.replacerx("\[MarshalAs\(UnmanagedType\.SysU?Int\)\] u?int" "LPARAM")) end "failed"
if(!m.replacerx(", EntryPoint=''(\w+[^W])''(.+[]public static extern [^\r\n\(]+\b\1\()" "$2")) end "failed" ;;remove EntryPoint from DllImport attribute, except for W functions
if(!m.replacerx("\[In\] (IntPtr|string) " "$1 ")) end "failed" ;;remove [In] where it is implicit
if(!m.replacerx("\[Out\] out " "out ")) end "failed" ;;remove [Out] where it is implicit
if(!m.findreplace("[][GeneratedCode(''P/Invoke Interop Assistant'', ''1.0'')]")) end "failed" ;;remove useless attributes from wrappers
if(!m.replacerx("\bNativeMethods\.")) end "failed"
if(!m.replacerx("(?m)^[](?:[\[/].+[])+public static extern \w+ __SA_.+[]")) end "failed" ;;SAL garbage

ARRAY(str) am aw; str sm si ;;extern methods, wrapper methods, interfaces

 get method array
if(!findrx(m F"(?m)^[](?:[\[/].+[])+public static extern \w+ (\w+)\(.+?$" 0 4 am)) end "failed" 1
 out am.len
 out findrx(m "(?m)^public static extern \w+ (\w+)" 0 4)
if(am.len!=findrx(m "(?m)^public static extern " 0 4))
	ARRAY(str) _a; findrx(m "(?m)^public static extern \w+ (\w+)" 0 4 _a)
	for(_i 0 am.len) if(_a[1 _i]!=am[1 _i]) out am[0 _i-1]; out am[0 _i]; break ;;show the last matching and the first nonmatching
	end "failed"

 get wrapper array
if(!findrx(m F"(?ms)^[]\[DebuggerStepThrough\][]public static \w+ (\w+).+?^\}(?=[][]|[]\z)" 0 4 aw)) end "failed" 1
 out aw.len
 out findrx(m "\[DebuggerStepThrough\]" 0 4)
if(aw.len!=findrx(m "\[DebuggerStepThrough\]" 0 4)) end "failed" 1

 extract interfaces
int nInterfaces nInterfaceMethods nCoclasses nStructRemoved
sub.Interfaces am si nInterfaces nInterfaceMethods nCoclasses st nStructRemoved

 remove A methods and wrappers, remove W in names
sub.ApplyAliases am map 1
sub.ApplyAliases aw map 2

 dll names
int nRemovedMethods=am.len
sub.DllNames am
nRemovedMethods-am.len

 array -> string
int nWrap
for i 0 am.len
	&r=am[0 i]
	sm.addline(r)
	 add wrapper after it
	&r=am[1 i]
	for j 0 aw.len
		if aw[1 j]=r
			 out aw[1 j]
			nWrap+1
			sm.addline(aw[0 j])
 out aw.len ;;163
 out nWrap ;;160 (some non-existing functions removed from am when processing dll names above)
if(nWrap<aw.len-20) end "failed"
 out sm

 correct incorrectly converted types
sub.CorrectTypes sm 0

#endregion

#region finally
 g1

 add usings
str usings=
 using System;
 using System.Runtime.InteropServices;
 using System.Text;
 
 //add this to projects that will use these API
 [module: DefaultCharSet(CharSet.Unicode)]
 
 public static class _Api
 {
;
s.from(usings "[]// CONST[]" c "[]// ENUM[][]" se "// STRUCT[][]" st si "// DELEGATE[][]" sd "// METHOD[]" sm "[]} //class _Api[]" util)

out s.len ;;3945376
int nConst=findrx(c "(?m)^public const " 0 4)
out F" enum={ae.len}, struct={at.len-nStructRemoved}, delegate={ad.len}, method={am.len} ({nRemovedMethods} removed), wrapper={aw.len}, const={nConst}, iface={nInterfaces}, imember={nInterfaceMethods}, coclass={nCoclasses}"
 enum=599, struct=1879, delegate=210, method=4749 (737 removed), wrapper=183, const=19005, iface=535, imember=3617, coclass=284

s.setfile("Q:\app\Catkeys\Api\Api.cs")

#endregion

#sub replacerxConstHex
function# REPLACERXCB&x
int v=val(x.subject+x.vec[2].cpMin)
x.match.format("public const uint %.*s0x%08X;" (x.vec[1].cpMax-x.vec[1].cpMin) (x.subject+x.vec[1].cpMin) v)


#sub replacerxEnumHex
function# REPLACERXCB&x
int v=val(x.subject+x.vec[3].cpMin)
x.match.format("%.*s0x%08X," (x.vec[2].cpMax-x.vec[2].cpMin) (x.subject+x.vec[2].cpMin) v)


#sub GetAliases
function str&c IStringMap&m

m._create
ARRAY(str) a; int i
if(!findrx(c "(?m)^/// Error generating expression: Value \w+W is not resolved[]public const string (\w+) = ''(\w+W)'';$" 0 4 a)) end "failed"
 out a.len
for i 0 a.len
	 out F"{a[2 i]} = {a[1 i]}"
	m.Add(a[2 i] a[1 i])
	err
		lpstr k(a[2 i]) vNew(a[1 i]) vOld(m.Get(k))
		 out F"{k} = {vNew}, was {vOld}"
		if(findc(vOld '_')>=0) m.Set(k vNew)


#sub ApplyAliases
function ARRAY(str)&a IStringMap&m typeKind ;;typeKind: 1 extern method, 2 wrapper method, 3 delegate, 4 struct
int i
for i a.len-1 -1 -1
	str& r=a[1 i]
	if r.end("W")
		if(typeKind>=3) continue ;;because also would need to replace names everywhere
		lpstr alias=m.Get(r); if(!alias) continue
		if(typeKind=1) if(1!=a[0 i].replacerx(F"(?<= ){r}(?=\()" alias)) end "failed" 1 ;;don't replace EntryPoint attribute
		else a[0 i].findreplace(r alias 2)
	else if r.end("A")
		_s=r; _s[_s.len-1]='W'
		if(m.Get(_s)) a.remove(i)

 remove A that are not in the map, eg using indirect #define/typedef. Not many. Usually they are adjacent, A then W.
for i a.len-2 -1 -1
	str& sa(a[1 i]) sw(a[1 i+1])
	if sa.end("A") and sw.end("W") and sw.len=sa.len and sw.beg(sa (sa.len-1))
		 out sa
		a.remove(i); i-1


#sub CorrectTypes
function str&s isStructMember

lpstr rx2="(?:\w+->)?wchar_t->unsigned short"
lpstr rx3="(?:\w+->)?BSTR->OLECHAR\*"
if isStructMember
	if(!s.replacerx(F"(?m)^ */// {rx2}[](?: *\[.+[])?( *public) ushort" "$1 char")) end "failed"
	if(!s.replacerx(F"(?m)^ */// {rx3}[]( *)public string" "$1[MarshalAs(UnmanagedType.BStr)][]$1public string")) end "failed"
 else
	 int n
	 n=0; rep() if(s.replacerx(F"(?m)^(///(\w+): {rx2}[](?:[\[/].+[])*public .+?) ushort \2" "$1 char $2")) n+1; else break
	 out n ;;0. Such parameters are converted correctly, although the comment is incorrect.
	 if(!n) end "failed" 1
	 n=0; rep() if(s.replacerx(F"(?m)^(///(\w+): {rx3}[](?:[\[/].+[])*public .+?[^\]]) string \2" "$1 [MarshalAs(UnmanagedType.BStr)] string $2")) n+1; else break
	 out n ;;0. Such parameters are converted correctly, although the comment is incorrect.
	 if(!n) end "failed" 1
	 out 2


#sub DllNames
function ARRAY(str)&am

str s.getfile("Q:\app\Catkeys\Api\DllMap.txt")
IStringMap dm._create; dm.AddList(s)
int i j
for i am.len-1 -1 -1
	lpstr DLL=dm.Get(am[1 i])
	if !DLL
		 out F"{am[1 i]}    {DLL}"
		am.remove(i) ;;in the map we don't have only functions that actually don't exist as dll exports. Some are inline, some documented as deprecated/removed, some msvcrt functions have alias with/without _, etc. Also we don't have functions with "Dll" prefix, eg DllRegisterServer, because they are common to many dll.
		continue
	 out F"{am[1 i]}    {DLL}"
	str& r=am[0 i]
	j=findc(DLL '|')
	if j>0
		lpstr alias=F", EntryPoint=''{DLL+j+1}''"
		if(r.replacerx(", EntryPoint *= *''.+?''" alias 4)<0) if(r.replacerx("\[DllImport\(''.+?''" F"$0{alias}" 4)<0) end "failed" 1
		 else out r
		DLL[j]=0
	if(r.replacerx("(?<=DllImport\('').+?(?='')" DLL 4)<0) end "failed" 1


#sub Interfaces
function ARRAY(str)&a str&si int&nInterfaces int&nInterfaceMethods int&nCoclasses str&st int&nStructRemoved

 a - methods, including interface members and interface begin/end markers.
 si - receives fully formatted coclasses and interfaces.
 nX - number of interfaces, members, coclasses, removed struct.
 st - struct strings. This func removes empty struct used for interfaces.

 array a contains extern methods and interface methods with markers:
 ExternMethod1
 ExternMethod2
 ...
 interface_begin_Interface1
 InterfaceMethod1
 InterfaceMethod2
 ...
 interface_end_Interface1
 ExternMethod3
 ...

lpstr name

 COCLASSES

si="// COCLASS[]"

 get CLSIDs from SDK
if(!g_SDK_Include.len) end "run macro 'Preprocess SDK headers with GCC', its sets SDK path" ;;actually this will be compile-time eror, g_SDK_Include unknown
Dir d
foreach(d F"{g_SDK_Include}\*.idl" FE_Dir 4)
	str path=d.FullPath ;;out path
	str data.getfile(d.FullPath);; err ...
	ARRAY(str) u; int k
	 CLSID
	findrx(data "\buuid\(([\w\-]+)\)\s*\]\s*coclass (\w+)" 0 4 u)
	for k 0 u.len
		nCoclasses+1
		name=u[2 k]
		str& clsid=u[1 k]
		str cc=
		F
		;
		 {sub.Guid(clsid "CLSID" name)}
		 [ComImport, Guid("{clsid}"), ClassInterface(ClassInterfaceType.None)]
		 public class {name}{{}
		;
		si+cc
 out si
 out nCoclasses
if(nCoclasses<200) end "failed" 1

 INTERFACES

si+F"[]// INTERFACE[][]{sub.Guid(`00000000-0000-0000-C000-000000000046` `IID` `IUnknown`)}[][]"

 get interface methods from array a
str s
ARRAY(POINT) arem
int i j n nMustBe memberNameOffset
for i 0 a.len
	if a[1 i].beg("interface_begin_")
		n=0
		POINT p.x=i
		name=a[1 i]+16
		memberNameOffset=len(name)+2
		str iid
		if(!findrx(a[0 i] "\(int _G_U_I_D_(\w+)" 0 0 iid 1)) end "failed" 1
		iid.decrypt(8)
		s=
		F
		 {sub.Guid(iid "IID" name)}
		 [ComImport, Guid("{iid}"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		 public interface {name}
		 {{
		for i i+1 a.len
			if a[1 i].beg("interface_end_")
				 converter drops methods if some member types undefined (eg failed to parse) etc
				nMustBe=val(a[1 i]+14)
				if n!=nMustBe
					_s=""; for(_i p.x+1 i) _s+F"  {a[1 _i]+memberNameOffset}"
					out F"{name}: {n}/{nMustBe}{_s}"
				
				p.y=i; arem[]=p
				s+"}[][]"
				si+s
				nInterfaces+1
				break
			else
				n+1
				nInterfaceMethods+1
				str& r=a[0 i]
				 out r
				if(r.replacerx("(?m)^\[.+?\]" "[PreserveSig]")!=1) end "failed" 1
				if(!r.replacerx(F"(?m)^public static extern (\w+ )_{name}_" "$1")) end "failed" 1
				if(!r.replacerx("(?m)^(?![\r\n])" "    ")) end "failed" 1
				
				 if(findrx(r "MarshalAs\(UnmanagedType\.(?!BStr)")>=0) out name; out r
				
				s+r; s+"[]"
		

 remove interface methods and markers from a
for j arem.len-1 -1 -1
	p=arem[j]
	for(i p.y p.x-1 -1) a.remove(i)

 remove empty struct used for interfaces
nStructRemoved=st.replacerx("(?m)^public struct \w+ \{[][]    public int _R_E_M_O_V_E_;[]\}[][]")
if(nStructRemoved!=nInterfaces+1) end "failed" 1

 out si


#sub Guid
function'str $guid $prefix $name
 Creates new Guid string like "public static readonly Guid {prefix}_{name} = new Guid(0x...);
 This is the fastest way of creating interface/coclass Guid.
 typeof({name}).GUID is 4 times slower. String ctor many times slower.

ARRAY(str) g
if(findrx(guid "^(\w{8})-(\w{4})-(\w{4})-(\w\w)(\w\w)-(\w\w)(\w\w)(\w\w)(\w\w)(\w\w)(\w\w)$" 0 0 g)) end F"failed: {guid}" 1
ret F"public static readonly Guid {prefix}_{name} = new Guid(0x{g[1]}, 0x{g[2]}, 0x{g[3]}, 0x{g[4]}, 0x{g[5]}, 0x{g[6]}, 0x{g[7]}, 0x{g[8]}, 0x{g[9]}, 0x{g[10]}, 0x{g[11]});"
