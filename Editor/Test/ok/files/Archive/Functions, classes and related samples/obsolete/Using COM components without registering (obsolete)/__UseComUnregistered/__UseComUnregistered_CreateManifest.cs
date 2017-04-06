 /usecomdll create manifest
function $comDllFile

 Creates manifest file for a COM component.
 Error if fails.
 Places the file in qm folder.
 Call this function once. Also after component version changes.

 comDllFile - COM component file. Usually dll or ocx.
   The file must be in qm folder or its subfolder.
   comDllFile can be full path or relative to qm folder, like "com.dll" or "subfolder\com.dll".
   Can be list of files if you want to use several components in single thread.


str sx line fullpath relpath manname
foreach line comDllFile
	if(!fullpath.searchpath(line "$qm$")) end "file not found"
	if(!fullpath.begi(_qmdir)) end "the file must be in qm folder or its subfolder"
	relpath=fullpath+_qmdir.len
	
	if(!sx.len)
		manname.getfilename(relpath); manname+".X"
		sx=
		 <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
		 <assembly xmlns="urn:schemas-microsoft-com:asm.v1" manifestVersion="1.0">
		 <assemblyIdentity type="win32" name="%1" version="1.0.0.0"/>
		;
		sx.findreplace("%1" manname)
	
	sx.formata("<file name = ''%s''>[]" relpath)
	
	 add typelib
	ITypeLib tl
	int hr=LoadTypeLibEx(@fullpath REGKIND_NONE &tl); if(hr) end _s.dllerror("" "" hr)
	TLIBATTR* la
	if(tl.GetLibAttr(&la)) end ES_FAILED
	sx.formata("<typelib tlbid=''%s'' version=''%i.%i'' helpdir=''''/>[]" _s.FromGUID(la.guid) la.wMajorVerNum la.wMinorVerNum)
	tl.ReleaseTLibAttr(la)
	
	 add all classes
	int tk i n=tl.GetTypeInfoCount
	for i 0 n
		tl.GetTypeInfoType(i &tk)
		if(tk!=TKIND_COCLASS) continue
		ITypeInfo ti=0
		tl.GetTypeInfo(i &ti)
		TYPEATTR* ta
		ti.GetTypeAttr(&ta)
		sx.formata("<comClass clsid=''%s'' threadingModel=''Apartment''/>[]" _s.FromGUID(ta.guid))
		ti.ReleaseTypeAttr(ta)
	
	sx+"</file>[]"

sx+"</assembly>"
 out sx

str manfile.from(_qmdir manname ".manifest")
 out manfile
sx.setfile(manfile)

out "Created manifest file: '%s'. Add this in macros that use this component:[]#compile ''____UseComUnregistered''[]__UseComUnregistered ucu.Activate(''%s.manifest'')" manfile manname

err+ end _error

 note:
 Windows XP and 7 differently seaches for the dll that is specified in the manifest (it must be relative path).
 XP - relative to application's folder. 7 - relative to manifest's folder. Although nowhere documented.
 To work on both OS, the manifest file must be in qm folder.
