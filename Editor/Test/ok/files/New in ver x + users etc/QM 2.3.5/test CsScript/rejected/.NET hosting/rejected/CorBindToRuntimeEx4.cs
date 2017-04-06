/exe
 typelib MSCOREE "$windows$\Microsoft.NET\Framework\v1.1.4322\mscorlib.tlb"
typelib MSCOREE "$windows$\Microsoft.NET\Framework\v2.0.50727\mscorlib.tlb"

PF
WINAPI2.ICorRuntimeHost h
out WINAPI2.CorBindToRuntimeEx(L"v2.0.50727" L"svr" WINAPI2.STARTUP_LOADER_SAFEMODE|WINAPI2.STARTUP_CONCURRENT_GC WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &h)
 out WINAPI2.CorBindToRuntimeEx(0 L"svr" WINAPI2.STARTUP_LOADER_SAFEMODE|WINAPI2.STARTUP_CONCURRENT_GC WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &h)
 out h
h.Start
IUnknown _u
h.GetDefaultDomain(&_u)
 out _u
MSCOREE._AppDomain ad=_u
 out ad
 MSCOREE._ObjectHandle oh=ad.CreateInstance("CsScriptLibrary" "CSScriptLibrary.QmCsScript")
MSCOREE._ObjectHandle oh=ad.CreateInstanceFrom("q:\app\CsScriptLibrary.dll" "CSScriptLibrary.QmCsScript")
VARIANT v=oh.Unwrap
IDispatch x=v.pdispVal
 out x
PN
_i=x.Test
PN
PO
out _i
h.Stop

 BEGIN PROJECT
 main_function  CorBindToRuntimeEx
 exe_file  $my qm$\CorBindToRuntimeEx.qmm
 flags  6
 guid  {ED456F38-E9BD-4F05-B96B-AF03A7C5BA4D}
 END PROJECT
