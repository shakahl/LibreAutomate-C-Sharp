/exe
 typelib MSCOREE "$windows$\Microsoft.NET\Framework\v1.1.4322\mscorlib.tlb"
typelib MSCOREE "$windows$\Microsoft.NET\Framework\v2.0.50727\mscorlib.tlb"

interface# ICsScriptFactory :IDispatch
	ICsScript'CreateCsScript
	Init(cbFuncOut cbFuncPerfNext)
	{ea25c876-028e-4e58-a12a-f8a8143df661}

#opt nowarnings 1
MSCOREE._AppDomain+ ___cs_ad
 IDispatch+ ___cs_f
ICsScriptFactory+ ___cs_f
#opt nowarnings 0

if !___cs_ad
	_s.getfile("q:\app\CsScriptLibrary.dll")
	ARRAY(byte) adata.create(_s.len); memcpy &adata[0] _s _s.len

PF
if !___cs_ad
	WINAPI2.ICorRuntimeHost h
	if(WINAPI2.CorBindToRuntimeEx(L"v2.0.50727" L"svr" WINAPI2.STARTUP_LOADER_SAFEMODE|WINAPI2.STARTUP_CONCURRENT_GC WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &h)<0) end ERR_FAILED
	 if(WINAPI2.CorBindToRuntimeEx(0 L"svr" WINAPI2.STARTUP_LOADER_SAFEMODE|WINAPI2.STARTUP_CONCURRENT_GC WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &h)<0) end ERR_FAILED
	h.Start
	IUnknown _u; h.GetDefaultDomain(&_u); ___cs_ad=_u;;out _u
	PN
	MSCOREE._Assembly a=___cs_ad.Load_3(adata)
	PN
	VARIANT v=a.CreateInstance("CSScriptLibrary.CsScriptFactory")
	 out v.vt ;;VT_DISPATCH
	___cs_f=v.pdispVal
	PN
rep 2
	ICsScript x=___cs_f.CreateCsScript
	 IDispatch x=___cs_f.CreateCsScript
	 out x
	PN
	rep 2
		_i=x.Test
		PN
PO
out _i
 h.Stop

 BEGIN PROJECT
 main_function  CorBindToRuntimeEx
 exe_file  $my qm$\CorBindToRuntimeEx.qmm
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {ED456F38-E9BD-4F05-B96B-AF03A7C5BA4D}
 END PROJECT

 DllExporter.exe $(TargetFileName)
 move $(TargetName).Exports$(TargetExt) $(TargetFileName)