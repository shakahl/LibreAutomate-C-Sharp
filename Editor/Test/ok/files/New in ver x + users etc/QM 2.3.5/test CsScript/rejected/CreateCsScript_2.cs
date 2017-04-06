 /Macro2018
function'ICsScript

 Tried to create host with ICLRRuntimeHost (recommended in .NET 2), but it seems it's impossible if we don't want to use assembly files (need an extra appdomainmanager dll file).
 Currently using ICorRuntimeHost. It works with .NET 2 too.

typelib MSCOREE "$windows$\Microsoft.NET\Framework\v2.0.50727\mscorlib.tlb"

interface# ICsScriptFactory :IDispatch
	ICsScript'CreateCsScript
	Init(cbFuncOut cbFuncPerfNext)
	{ea25c876-028e-4e58-a12a-f8a8143df661}

#opt nowarnings 1
WINAPI2.ICLRRuntimeHost ___cs_host
MSCOREE._AppDomain ___cs_ad
MSCOREE._Assembly ___cs_asm
ICsScriptFactory+ ___cs_f
#opt nowarnings 0

 if(___cs_f) goto g1
 lock
 if(___cs_f) goto g1

byte* data; int dataLength
#if EXE
data=+ExeGetResourceData(252 100 0 &dataLength)
#else
data=_s.getfile("q:\app\CsScriptLibrary.dll"); dataLength=_s.len
#endif
ARRAY(byte) ar.create(dataLength); memcpy &ar[0] data dataLength ;;nevermind speed, it's just ~0.2 % of total loading time. Size is <100 KB.
_s.all

 PN
int fl=WINAPI2.STARTUP_LOADER_SAFEMODE|WINAPI2.STARTUP_CONCURRENT_GC
int hr=WINAPI2.CorBindToRuntimeEx(L"v2.0.50727" L"svr" fl WINAPI2.CLSID_CLRRuntimeHost WINAPI2.IID_ICLRRuntimeHost &___cs_host)
 int hr=WINAPI2.CorBindToRuntimeEx(0 L"svr" fl WINAPI2.CLSID_CLRRuntimeHost WINAPI2.IID_ICLRRuntimeHost &___cs_host)
if(hr<0) end F"{ERR_FAILED}. {_s.dllerror(`` `` hr)}"

WINAPI2.ICLRControl clr
___cs_host.GetCLRControl(clr)
out clr.GetCLRManager
___cs_host.
#ret

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
