 /Macro2585
function $dllFile str&s str&sErrors [flags] ;;flags: 1 skip C++ functions, 2 skip COM functions (DllRegisterServer etc), 4 skip "_*" functions, 0x100 not error if 0 exports, 0x200 skip dlls ["*-ms-*","msvc*","*d3d*","*d2d*"]


#compile "____Hmodule"

s="<>"
sErrors.all
Dir d
foreach(d dllFile FE_Dir)
	str path=d.FullPath
	str name=d.FileName
	if(flags&0x200) sel(name 3) case ["*-ms-*","msvc*","*d3d*","*d2d*"] continue
	 out name
	 __Hmodule _x.Init(LoadLibraryEx(path 0 DONT_RESOLVE_DLL_REFERENCES)) ;;dangerous, displays messageboxes etc, crashes, etc
	__Hmodule _x.Init(LoadLibraryEx(path 0 LOAD_LIBRARY_AS_DATAFILE|LOAD_LIBRARY_AS_IMAGE_RESOURCE))
	int x=_x~15
	if(!x) sErrors.addline(F"cannot load: {name}, size={GetFileOrFolderSize(path)}"); continue
	IMAGE_DOS_HEADER* dos=+x
	if(dos.e_magic!=IMAGE_DOS_SIGNATURE) sErrors.addline(F"bad image signature: {name}"); continue
	IMAGE_NT_HEADERS* header = +(x+dos.e_lfanew)
	if(header.Signature != IMAGE_NT_SIGNATURE) sErrors.addline(F"bad NT signature: {name}"); continue
	if(header.OptionalHeader.NumberOfRvaAndSizes<1) goto gNoExports
	IMAGE_DATA_DIRECTORY& dd=header.OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_EXPORT]
	if(!dd.Size) goto gNoExports
	IMAGE_EXPORT_DIRECTORY* exports = +(x + dd.VirtualAddress)
	int* names = +(x + exports.AddressOfNames)
	int outDllName(1) i n=exports.NumberOfNames
	 out n
	for i 0 n
		lpstr fn=+(x+names[i])
		if(names[i]<4096 or names[i]>100000000 or IsBadReadPtr(fn 1))
			sErrors.addline(F"bad function offset: {name}: {names[i]}")
			continue
		if(flags&4) if(fn[0]='_') continue
		if(flags&1) if(!iscsym(fn[0]) or (fn[0]='_' and findc(fn '@')>0)) continue
		if(flags&2) sel(fn) case ["DllCanUnloadNow","DllGetClassObject","DllInstall","DllMain","DllRegisterServer","DllUnregisterServer","DllGetActivationFactory","GetProxyDllInfo","DllGetVersion","CreateInstance","KbdLayerDescriptor","KbdNlsLayerDescriptor","NlsDllCodePageTranslation"] continue
		if(outDllName) outDllName=0; s.formata("<Z 0x80C080> %s</Z>[]" name)
		s.formata("%s[]" fn)
	
	 if(s.len>100000) break
	continue
	 gNoExports
	if(flags&0x100=0) sErrors.addline(F"no exports: {name}")
	continue
