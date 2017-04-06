function $assemblyFile

 Loads assembly from file.
 Then you can use <help>CsScript.Call</help> to call static functions, <help>CsScript.CreateObject</help> to call non-static functions.

 assemblyFile - assembly file (usually .dll). You can use <help>CsScript.Compile</help> to create it, or use an existing file.

 REMARKS
 Supports resource id in assemblyFile (see example). When making exe with 'Auto add files' checked, adds the file to exe resources, and in exe loads from resource instead of file. In QM loads from file.

 The inMemoryAsm option is applied (see <help>CsScript.SetOptions</help>). By default it is true. Then the loaded assembly cannot be used by other assemblies. If need it, clear the option (see example).

 Errors: <..>

 EXAMPLE
 CsScript x.SetOptions("inMemoryAsm=false")
 x.Load(":10 $my qm$\csscript.dll")
 x.Call("Func" "test")


opt noerrorshere 1
opt nowarningshere 1

Init

int resId
lpstr s=assemblyFile

if(!findrx(s "^:\d+ *" 0 0 _i)) resId=val(s+1); s+_i
#if EXE
if resId
	int resSize
	byte* b=+ExeGetResourceData(10 resId 0 &resSize)
	if(!b) end F"{ERR_FAILED} failed to load resource {resId}"
	LoadFromMemory(b resSize)
	ret
#endif

x.Load(_s.expandpath(s))
