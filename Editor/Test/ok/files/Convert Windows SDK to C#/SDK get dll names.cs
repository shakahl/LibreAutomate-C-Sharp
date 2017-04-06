 Gets dll function names and their dll file names from .lib and .dll files. Also gets ordinal or export name, if need.
 Saves in "Q:\app\Catkeys\Api\DllMap.txt". Format:
 FuncX dllX.dll
 FuncZ dllX.dll|#ordinal
 FuncY dllX.dll|FuncNameInDll

out

str libDir="Q:\SDK10\Lib\10.0.10586.0\um\x64"
str lib_list=
F
 {libDir}\*.lib
 $system$\hhctrl.ocx
 $system$\ieframe.dll
 $system$\pla.dll
 $system$\msvcrt.dll
 $system$\ntdll.dll
 $system$\kernelbase.dll

 $system$\*.dll

str s
sub.CreateDllMap F"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\dumpbin.exe" lib_list s 2|0
 out s
s.setfile("Q:\app\Catkeys\Api\DllMap.txt")


#sub CreateDllMap
function ~dumpbin_exe $lib_list str&sout [flags] ;;flags: 1 include subfolders, 2 include system dlls for libs, 4 use cached dumpbin results (makes cache even without this flag)

 Extracts dll function names and their dll file names from lib or/and dll file(s).

 dumpbin_exe - full path of dumpbin.exe. It is in Visual C subfolder "bin".
   Use dumpbin.exe from VS14. If from VS9, it fails because mspdb80.dll is missing in its folder. If we copy the dll to the folder, VS9 fails to compile (error C1902).
 lib_list - one or more lib or/and dll files. Can contain wildcard characters. Example: "C:\mylibdir\file.lib[]$system$\*.dll".
 sout - receives the list of extracted functions-dlls.

 REMARKS
 Shows warnings "no exports" etc. It's normal when processing all lib or dll in a folder.


str sLib sDll 
str s ss
int i j
ARRAY(str) a

 cache dumpbin results when developing post-dumpbin code
str cacheLib="$temp qm$\CreateDllMap lib.txt"
str cacheDll="$temp qm$\CreateDllMap dll.txt"
if flags&4
	if FileExists(cacheLib) and FileExists(cacheDll)
		sLib.getfile(cacheLib)
		sDll.getfile(cacheDll)
		out "NOTE: using cached dumpbin results (flag 4)."
		goto gSkipDumpbin


#compile "____Wow64DisableWow64FsRedirection"
__Wow64DisableWow64FsRedirection dr.DisableRedirection

IStringMap mDllOfLib=CreateStringMap(2) ;;at first process lib, and add dll to this

dumpbin_exe.expandpath
 PF
foreach ss lib_list
	if(!ss.len) continue
	Dir d
	foreach(d ss FE_Dir iif(flags&1 4 0))
		str fn=d.FileName; fn.lcase
		str path=d.FullPath; path.lcase
		if(!matchw(fn "*.lib" 1)) mDllOfLib.Add(path fn); continue
		sel fn 2
			case ["api-ms-*"] continue ;;invalid copies of normal dll
			case ["ntstc_*","mspbase.lib","strmbase.lib","uuid.lib","dnslib.lib","netlib.lib","dnsrpc.lib","clfsmgmt.lib"] continue ;;large, no dll func
		 out fn
		
		 /HEADERS - functions and dll names, but slow and requires much memory. /EXPORTS - just function names.
		int e=RunConsole2(F"''{dumpbin_exe}'' /HEADERS ''{path}''" s)
		if(e)
			if(e=-1073741515) run dumpbin_exe; end F"dumpbin.exe failed, {e}" 1;; show "mspdb80.dll is missing"
			end F"dumpbin.exe failed, {e}, {fn}" 1|8
			continue
		
		 extract just dll/func names + some more info, because all info is very big
		if(!findrx(s "(?m)^  DLL name *: (?!api-ms-)(.+)[](?s).+?[][]" 0 4 a)) end F"no exports in {fn}" 8|1
		if a.len
			sLib+F"[][]{fn}[][]";
			for(i 0 a.len)
				sLib+a[0 i]
				 add unique dlls to mDllOfLib
				if i=0 or a[1 i]!=a[1 i-1]
					 out a[1 i]
					if(path.searchpath(a[1 i])) mDllOfLib.Add(path.lcase a[1 i])
		
		if flags&2
			fn.set("dll" fn.len-3)
			if(path.searchpath(fn)) mDllOfLib.Add(path.lcase fn)

mDllOfLib.EnumBegin
rep
	if(!mDllOfLib.EnumNext(path fn)) break
	e=RunConsole2(F"''{dumpbin_exe}'' /EXPORTS ''{path}''" s)
	if(e)
		end F"dumpbin.exe failed, {e}, {fn}" 1|8
		continue
		
	if(!findrx(s "\bordinal hint RVA.+[](?s).+?[][]" 0 0 _s)) end F"no exports in {fn}" 8|1
	sDll+F"DLL: {fn}[]{_s}"
 PN;PO ;;35 s without /HEADERS, 235 s with /HEADERS

dr.Revert

sLib.setfile(cacheLib)
sDll.setfile(cacheDll)
 ret

 gSkipDumpbin

 list item formats:
 symbol dll
 symbol dll|#ordinal
 symbol dll|funcNameInDll

IStringMap m(CreateStringMap()) mDll(CreateStringMap())
lpstr rx v vv
str& DLL sym

 LIB

rx=
 (?m)^  DLL name *: (.+)
   Symbol name *: (\w.+)
   Type *: code
   Name type *: (.+)
   (?:Ordinal *: (.+)|Hint .+\r\n  Name *: (.+))\r\n\r\n

if(!findrx(sLib rx 0 4 a)) end "failed" 1
for i 0 a.len
	&DLL=a[1 i]; &sym=a[2 i]
	str& name(a[5 i]) ord(a[4 i])
	DLL.lcase
	if(sym[0]='_') sym.gett(sym+1 0 "@")
	if ord.len
		if(DLL="mapi32.dll") v=DLL ;;two ord, one of which incorrect
		else v=F"{DLL}|#{val(ord)}"
	else if name.len
		if(name=sym) v=DLL
		else v=F"{DLL}|{name}";; out F"{sym} {DLL}|{name}"
	else end "failed" 1
	
	vv=m.Get(sym)
	if(!vv) m.Add(sym v); else if(sub.ChooseDll(vv v sym)) m.Set(sym v)

 DLL

 out "---------- DLL ------------"

rx="(?m)(?>^DLL: (.+)[]ordinal hint RVA .+[][])(?s)(.+?)[][]"
if(!findrx(sDll rx 0 4 a)) end "failed" 1
ARRAY(str) b
for i 0 a.len
	&DLL=a[1 i]; DLL.lcase
	sel DLL
		case ["hhsetup.dll"] continue ;;all func "?..."
	if(!findrx(a[2 i] "(?m)^      (.{20})(\w+)" 0 4 b)) end F"failed, {DLL}" 1|8; continue
	 out "%s %i" DLL b.len
	for j 0 b.len
		&sym=b[2 j]
		if(m.Get(sym)) continue
		if(sym.beg("Dll")) continue ;;DllGetClassObject etc
		v=DLL
		vv=mDll.Get(sym)
		if(!vv) mDll.Add(sym v); else if(sub.ChooseDll(vv v sym)) mDll.Set(sym v)
 
m.GetList(sout)
mDll.GetList(s); sout+s
 end

out F"Found {numlines(sout)} functions."


#sub ChooseDll
function! $dllOld $dllNew $sym

if(!StrCompare(dllOld dllNew)) ret

sel F"{dllOld} {dllNew}" 2
	case "bcrypt.dll ncrypt.dll" ret 
	case "bluetoothapis.dll bthprops.cpl" ret 1
	case "bthprops.cpl irprops.cpl" ret 
	case "chakra.dll jscript9.dll" ret 1 ;;chakra newer but Win10
	case "jscript9.dll chakra.dll" ret
	case "crypt32.dll wintrust.dll" ret 
	case "d3d10.dll d3d10_1.dll" ret 
	case "d3d11.dll gdi32.dll" ret 1
	case "dbghelp.dll imagehlp.dll" ret 
	case "dsparse.dll ntdsapi.dll" ret 1
	case "ntdsapi.dll dsparse.dll" ret
	case "evr.dll mfplat.dll" ret 1
	case "iprop.dll ole32.dll" ret 1
	case "mf.dll mfcore.dll" ret 
	case "mfplat.dll mf.dll" ret 
	case "mscoree.dll mscorsn.dll" ret 
	case "mstask.dll|* mstask.dll" ret 
	case "netapi32.dll *" ret
	case "* netapi32.dll" ret 1
	case "kernel32.dll normaliz.dll" ret 1
	case "normaliz.dll kernel32.dll" ret
	case "oleaut32.dll* olepro32.dll*" ret 
	case "ole32.dll olecli32*" ret 
	case "shell32.dll shfolder.dll" ret 
	case "* secur32.dll" ret 1
	case "secur32.dll *" ret 
	case "ntdll.dll kernel32.dll" ret
	case "kernel32.dll ntdll.dll" ret 1
	case "msvcrt.dll ntdll.dll" ret 
	case "ntdll.dll msvcrt.dll" ret 1
	case "* wsock32.dll*" ret 
	case "spoolss.dll winspool.drv*" ret 1
	case "cfgmgr32.dll setupapi.dll" ret 
	case "advapi32.dll wmi.dll" ret
	case "winsta.dll wtsapi32.dll" ret 1
	case "certca.dll certcli.dll" ret
	case "kernel32.dll kernelbase.dll" ret
	case "kernelbase.dll msvcrt.dll" ret 1
	case "kernelbase.dll version.dll" ret 1

 out "%-40s:  %s %s" sym dllOld dllNew
