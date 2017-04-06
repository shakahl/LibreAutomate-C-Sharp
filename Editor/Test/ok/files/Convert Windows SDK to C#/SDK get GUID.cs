 Gets GUID names/data from .lib files.
 Saves in "Q:\app\Catkeys\Api\GuidMap.txt". Format:
 FuncX dllX.dll
 FuncZ dllX.dll|#ordinal
 FuncY dllX.dll|FuncNameInDll

out

str libDir="Q:\SDK10\Lib\10.0.10586.0\um\x64"
str lib_list=
F
 {libDir}\*.lib

str s
sub.CreateGuidMap F"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\dumpbin.exe" lib_list s 0
out s
s.setfile("Q:\app\Catkeys\Api\GuidMap.txt")


#sub CreateGuidMap
function ~dumpbin_exe $lib_list str&sout [flags] ;;flags: 1 include subfolders

 Extracts GUID names/data from .lib files.

 dumpbin_exe - full path of dumpbin.exe. It is in Visual C subfolder "bin".
   Use dumpbin.exe from VS14. If from VS9, it fails because mspdb80.dll is missing in its folder. If we copy the dll to the folder, VS9 fails to compile (error C1902).
 lib_list - one or more lib files. Can contain wildcard characters.
 sout - receives the list of extracted GUID names/data.


str s ss
int i
ARRAY(str) a

dumpbin_exe.expandpath
foreach ss lib_list
	if(!ss.len) continue
	Dir d
	foreach(d ss FE_Dir iif(flags&1 4 0))
		str fn=d.FileName
		sel fn 3
			case "api-ms-*" continue ;;invalid copies of normal dll
			case "ntstc_*" continue ;;large, no uuid
		str path=d.FileName(1)
		int e=RunConsole2(F"''{dumpbin_exe}'' /HEADERS /RAWDATA ''{path}''" s)
		if(e)
			if(e=-1073741515) run dumpbin_exe; end F"dumpbin.exe failed, {e}" 1;; show "mspdb80.dll is missing"
			end F"dumpbin.exe failed, {e}, {fn}" 1|8
		
		int n=findrx(s "(?m) +COMDAT; sym= *((?:[A-Z]|guid)\w+)\r\n(?: .+\r\n)+\r\nRAW DATA #\w+\r\n  \w+: (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w) (\w\w)  .*\n\r\n" 0 4 a)
		if(!n) continue
		out F"{fn}  {n}"
		for(i 0 n) sout+F"{a[1 i]} 0x{a[5 i]}{a[4 i]}{a[3 i]}{a[2 i]}, 0x{a[7 i]}{a[6 i]}, 0x{a[9 i]}{a[8 i]}, 0x{a[10 i]}, 0x{a[11 i]}, 0x{a[12 i]}, 0x{a[13 i]}, 0x{a[14 i]}, 0x{a[15 i]}, 0x{a[16 i]}, 0x{a[17 i]}[]"
