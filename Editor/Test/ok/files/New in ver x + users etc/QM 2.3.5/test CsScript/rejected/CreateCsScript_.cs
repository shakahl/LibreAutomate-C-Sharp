function'ICsScript

typelib MSCOREE "$windows$\Microsoft.NET\Framework\v2.0.50727\mscorlib.tlb"

interface# ICsScriptFactory :IDispatch
	ICsScript'CreateCsScript
	Init(cbFuncOut cbFuncPerfNext)
	{ea25c876-028e-4e58-a12a-f8a8143df661}

#opt nowarnings 1
WINAPI2.ICorRuntimeHost ___cs_host
MSCOREE._AppDomain ___cs_ad
MSCOREE._Assembly ___cs_asm
ICsScriptFactory+ ___cs_f
#opt nowarnings 0

if(___cs_f) goto g1
lock
if(___cs_f) goto g1

#if EXE=1
byte* data; int dataLength
data=+ExeGetResourceData(252 100 0 &dataLength)
ARRAY(byte) ar.create(dataLength); memcpy &ar[0] data dataLength ;;nevermind speed, it's just ~0.2 % of total loading time. Size is <100 KB.
#endif

 PN
int fl=WINAPI2.STARTUP_CONCURRENT_GC
 int fl=0
 if(WINAPI2.CorBindToRuntimeEx(L"v2.0.50727" L"svr" fl WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &___cs_host)<0) end ERR_FAILED
if(WINAPI2.CorBindToRuntimeEx(0 L"svr" fl WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &___cs_host)<0) end ERR_FAILED
 if(WINAPI2.CorBindToRuntime(L"v2.0.50727" L"svr" WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &___cs_host)<0) end ERR_FAILED
___cs_host.Start
IUnknown _u; ___cs_host.GetDefaultDomain(&_u); ___cs_ad=_u;;out _u
 PN
#if EXE=1
___cs_asm=___cs_ad.Load_3(ar)
#else
___cs_asm=___cs_ad.Load_2("CsScriptLibrary")
#endif
 PN
VARIANT v=___cs_asm.CreateInstance("CSScriptLibrary.CsScriptFactory")
 ARRAY(MSCOREE._Type) at=___cs_asm.GetExportedTypes; out at[0].GetMember
___cs_f=v.pdispVal
 PN
___cs_f.Init(&CsScript_OutCallback &CsScript_PerfNextCallback)
 ___cs_f.Init(0 0)
 IDispatch _d=___cs_f; _d.Init(0 0)
 PN

 g1
ret ___cs_f.CreateCsScript

 err+ end _error
