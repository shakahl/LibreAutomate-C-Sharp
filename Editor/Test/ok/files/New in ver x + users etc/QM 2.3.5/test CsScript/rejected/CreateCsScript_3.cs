 /Macro1447
function'ICsScript

typelib MSCOREE "$windows$\Microsoft.NET\Framework\v2.0.50727\mscorlib.tlb"

	 Init(icb)
interface# ICsScriptFactory :IDispatch
	Init(cbFuncOut cbFuncPerfNext)
	ICsScript'CreateCsScript
	{ea25c876-028e-4e58-a12a-f8a8143df661}

#opt nowarnings 1
WINAPI2.ICorRuntimeHost ___cs_host
MSCOREE._AppDomain ___cs_ad
MSCOREE._Assembly ___cs_asm
ICsScriptFactory+ ___cs_f
#opt nowarnings 0

 PN
int fl=WINAPI2.STARTUP_CONCURRENT_GC
 int fl=0
 if(WINAPI2.CorBindToRuntimeEx(L"v2.0.50727" L"svr" fl WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &___cs_host)<0) end ERR_FAILED
if(WINAPI2.CorBindToRuntimeEx(0 L"svr" fl WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &___cs_host)<0) end ERR_FAILED
 if(WINAPI2.CorBindToRuntime(L"v2.0.50727" L"svr" WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &___cs_host)<0) end ERR_FAILED
___cs_host.Start
IUnknown _u; ___cs_host.GetDefaultDomain(&_u); ___cs_ad=_u;;out _u
 PN
 ___cs_asm=___cs_ad.Load_2("CSScriptLibrary")
_s.getfile("q:\app\CSScriptLibrary.dll"); ARRAY(byte) ab.create(_s.len); memcpy &ab[0] _s _s.len
___cs_asm=___cs_ad.Load_3(ab)
 PN
VARIANT v=___cs_asm.CreateInstance("csscript.Host")
 ARRAY(MSCOREE._Type) at=___cs_asm.GetExportedTypes; out at[0].GetMember
___cs_f=v.pdispVal
 PN
___cs_f.Init(&CsScript_OutCallback &CsScript_PerfNextCallback)
 ___cs_f.Init(0)
 IDispatch _d=___cs_f; _d.Init(0 0)
 PN

 g1
ret ___cs_f.CreateCsScript

 err+ end _error
