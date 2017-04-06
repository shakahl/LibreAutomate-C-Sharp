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

byte* data; int dataLength
 #if EXE
#if EXE=1
data=+ExeGetResourceData(252 100 0 &dataLength)
#else
data=_s.getfile("$qm$\CsScriptLibrary.dll"); dataLength=_s.len
#endif
ARRAY(byte) ar.create(dataLength); memcpy &ar[0] data dataLength ;;nevermind speed, it's just ~0.2 % of total loading time. Size is <100 KB.
_s.all

 PN
int fl=WINAPI2.STARTUP_LOADER_SAFEMODE|WINAPI2.STARTUP_CONCURRENT_GC
if(WINAPI2.CorBindToRuntimeEx(L"v2.0.50727" L"svr" fl WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &___cs_host)<0) end ERR_FAILED
 if(WINAPI2.CorBindToRuntimeEx(0 L"svr" fl WINAPI2.CLSID_CorRuntimeHost WINAPI2.IID_ICorRuntimeHost &___cs_host)<0) end ERR_FAILED
___cs_host.Start
IUnknown _u; ___cs_host.GetDefaultDomain(&_u); ___cs_ad=_u;;out _u
 PN
___cs_asm=___cs_ad.Load_3(ar)
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
