 /
function'IUnknown $dllFile GUID*clsid GUID*iid

 Creates COM object from specified dll file, bypassing registry.
 Can be used instead of _create to create COM object on computers where it is not registered.
 Error if fails.
 QM 2.3.4: _create also supports this.

 dllFile - file where the object is. Usually dll. Typically it is the same file where its type library is. Cannot be exe.
 clsid - class id of the COM object. Typically uuidof(Typelib.Class).
 iid - interface id of the COM object. Typically uuidof(Typelib.Interface). To see interfaces, click class in editor. Status bar will show coclass Class Interface (default) ... Use the default interface.

 The function loads the dll. It will be loaded until QM exits. If want to unload when already not used, call FreeLibrary.
 Works not with all components. Possible various anomalies.
 Works on all Windows versions.

 EXAMPLE
 typelib TL "C:\F\ComDll.dll"
 TL.Class1 c=CreateComObjectUnregistered("C:\F\ComDll.dll" uuidof(TL.Class1) uuidof(TL.IClass1)) ;;use this instead of TL.Class1 c._create


BSTR sf=_s.expandpath(dllFile)
int h=GetModuleHandleW(sf)
if(!h) h=LoadLibraryW(sf); if(!h) end _s.dllerror
int fa=GetProcAddress(h "DllGetClassObject"); if(!fa) end "it is not a COM dll"

IClassFactory cf; IUnknown u
int hr=call(fa clsid IID_IClassFactory &cf); if(hr) goto ge
hr=cf.CreateInstance(0 iid &u); if(hr) goto ge

ret u
 ge
_s.dllerror("" "" hr)
end iif(_s.len _s ES_FAILED)
