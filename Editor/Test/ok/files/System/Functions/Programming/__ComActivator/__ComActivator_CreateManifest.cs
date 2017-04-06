 /
function $dllFile [flags] ;;flags: 1 .NET, 2 create type library (.NET only)

 Creates manifest file for a COM component or .NET COM-visible component.
 Error if fails.
 Places the output file(s) in QM folder.
 QM should be running as administrator.

 dllFile - component file. Usually dll or ocx.
   The file must be in QM folder or in a subfolder.
   Can be full path, or relative to QM folder, like "com.dll" or "subfolder\com.dll".
   Can be list of files if you want to use single manifest for several components.
   If it is a .NET component, it cannot be in a subfolder, and cannot be multiple files.

 REMARKS
 May fail if the component is created for higher .NET framework version than currently installed. Error "...not a valid .NET assembly". Install newer .NET framework.

 See also: <__ComActivator help>.


IXml x._create
lpstr s=
 <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
 <assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
  <assemblyIdentity type="win32" />
 </assembly>
x.FromString(s)
IXmlNode nr nai nf n
nr=x.RootElement
nai=nr.FirstChild
int i nFiles

str line fullpath relpath manname regasm regdata tlbfile progid
foreach line dllFile
	if(!fullpath.searchpath(line "$qm$")) end "file not found"
	if(!fullpath.begi(_qmdir)) end "the file must be in QM folder or its subfolder"
	relpath=fullpath+_qmdir.len
	if !nFiles
		manname.getfilename(relpath); manname+".X"
		nai.SetAttribute("name" manname)
	
	if flags&1 ;;.NET
		if(nFiles) end "multiple .NET components not supported"
		
		 use regasm /regfile
		if(!GetNetRuntimeFolder(regasm)) end "failed to get .NET runtime folder"
		regasm+"\RegAsm.exe"; if(!FileExists(regasm)) end "regasm.exe not found"
		__TempFile tf.Init
		del- tf; err
		if(RunConsole2(F"''{regasm}'' ''{fullpath}'' /regfile:''{tf}'' /nologo" _s)) end F"failed to get assembly info. {_s.trim}"
		regdata.getfile(tf); err end _s.trim ;;when there are no classes to register, regasm gives warning and returns 0
		 out regdata
		
		 convert the reg file to xml
		ARRAY(str) a
		s=
		 ^\[HKEY_CLASSES_ROOT\\(.+?)\\CLSID\]
		 @="(.+?)"(?:\r\n.*)+?
		 ^\[HKEY_CLASSES_ROOT\\CLSID\\\2\\InprocServer32\](?:\r\n.+)*?
		 "Class"="(.+?)"
		 "Assembly"="(.+?), Version=(.+?), .+
		 (?:"RuntimeVersion"="(.+?)")?
		if(!findrx(regdata s 0 1|4|8 a)) end "failed to parse assembly info"
		 out a[0 0]
		for i 0 a.len ;;for each class
			n=nr.Add("clrClass")
			n.SetAttribute("clsid" a[2 i])
			n.SetAttribute("progid" a[1 i]); if(!i) progid=a[1 i]
			n.SetAttribute("name" a[3 i])
			if(a[6 i].len) n.SetAttribute("runtimeVersion" a[6 i])
			if !i
				_s.from(a[4 i] ".dll"); if(relpath~_s=0) out F"Warning: different assembly name and file name. The dll must be '{_qmdir}{_s}'." ;;will fail in this case. The <file> tag does not help.
				nai.SetAttribute("name" a[4 i])
				nai.SetAttribute("version" a[5 i])
		
		 need tlb?
		if flags&2
			int isRegistered=rget(_s "" a[1 0] HKEY_CLASSES_ROOT); str se
			tlbfile=F"$qm$\{a[4 0]}.tlb"
			if(RunConsole2(F"''{regasm}'' ''{fullpath}'' /tlb:''{_s.expandpath(tlbfile)}'' /nologo" _s)) se=_s.trim
			if(!isRegistered) RunConsole2(F"''{regasm}'' ''{fullpath}'' /u /nologo" _s) ;;unregister, because with /tlb regasm registers
			if(se.len) end F"failed to create type library. {se}[]Try to restart QM."
	else ;;COM
		if(!nFiles) nai.SetAttribute("version" "1.0.0.0")
		
		nf=nr.Add("file")
		nf.SetAttribute("name" relpath)
		
		 add typelib
		ITypeLib tl=0; TLIBATTR* la=0
		int hr=LoadTypeLibEx(@fullpath REGKIND_NONE &tl); if(hr) end "" 16 hr
		tl.GetLibAttr(&la)
		n=nf.Add("typelib")
		n.SetAttribute("tlbid" _s.FromGUID(la.guid))
		n.SetAttribute("version" F"{la.wMajorVerNum}.{la.wMinorVerNum}")
		n.SetAttribute("helpdir" "")
		tl.ReleaseTLibAttr(la)
		tlbfile=F"$qm$\{relpath}"
		
		 add all classes
		for i 0 tl.GetTypeInfoCount
			tl.GetTypeInfoType(i &_i); if(_i!=TKIND_COCLASS) continue
			ITypeInfo ti=0; TYPEATTR* ta=0
			tl.GetTypeInfo(i &ti)
			ti.GetTypeAttr(&ta)
			n=nf.Add("comClass")
			n.SetAttribute("clsid" _s.FromGUID(ta.guid))
			n.SetAttribute("threadingModel" "Apartment")
			ti.ReleaseTypeAttr(ta)
	
	nFiles+1

str manfile.from(_qmdir manname ".manifest")
 out manfile
x.ToString(_s); out "<><c 0xff0000>%s</c>" _s
x.ToFile(manfile)

if(flags&3=3) out "<>Created: %s" _s.expandpath(tlbfile)
out "<>Created: %s[]Code:[]<code>__ComActivator ca.Activate(''%s.manifest'')</code>" manfile manname
if(tlbfile.len) out "<><code>typelib TypelibName ''%s'' 1</code>" tlbfile
if(flags&3=1) out "<><code>IDispatch x._create(''%s'')</code>" progid; else out "<><code>TypelibName.ClassName x._create</code>"

err+ end _error

 note:
 Windows XP and 7 differently seaches for the dll that is specified in the manifest (it must be relative path).
 XP - relative to application's folder. 7 - relative to manifest's folder. Although nowhere documented.
 To work on both OS, the manifest file must be in qm folder.
