#include "stdafx.h"
#include "util.h"
#include <metahost.h>
#include <mscoree.h>
#pragma comment(lib, "mscoree.lib")

_COM_SMARTPTR_TYPEDEF(ICLRMetaHost, __uuidof(ICLRMetaHost));
_COM_SMARTPTR_TYPEDEF(ICLRRuntimeInfo, __uuidof(ICLRRuntimeInfo));
_COM_SMARTPTR_TYPEDEF(ICLRRuntimeHost, __uuidof(ICLRRuntimeHost));

ICLRRuntimeHostPtr s_pClrRuntimeHost;
bool s_clrHostStarted;

typedef int(__stdcall* CsFuncT)(void* x);
CsFuncT s_csFunc;

void UnloadClr()
{
	if(s_clrHostStarted) {
		s_clrHostStarted = false;

		//OleInitialize(0);

		//DWORD ad;
		//Printx(s_pClrRuntimeHost->GetCurrentAppDomainId(&ad));
		//Print(ad);

		//Printx(s_pClrRuntimeHost->UnloadAppDomain(1, 1));


		Printx(s_pClrRuntimeHost->Stop());
		s_pClrRuntimeHost.Release();

		//OleUninitialize();
	}
}

void TestClrHost(OUT int& r)
{
	r = 0;
	//Print(s_clrHostStarted);
	DWORD pReturnValue;
	Perf.First();
	HRESULT hr;

	if(!s_clrHostStarted) {
		ICLRMetaHostPtr pMetaHost;
		ICLRRuntimeInfoPtr pRuntimeInfo;
		hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));
		if(hr) { Printx(hr); return; }
		hr = pMetaHost->GetRuntime(L"v4.0.30319", IID_PPV_ARGS(&pRuntimeInfo));
		if(hr) { Printx(hr); return; }
		hr = pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&s_pClrRuntimeHost));
		if(hr) { Printx(hr); return; }

		hr = s_pClrRuntimeHost->Start();
		if(hr < 0) { Printx(hr); return; }

		WCHAR b[24];
		hr = s_pClrRuntimeHost->ExecuteInDefaultAppDomain(
			L"Q:\\app\\Catkeys\\Tests\\Catkeys.dll",
			L"Catkeys.Acc",
			L"TestClrHost",
			_i64tow((__int64)&s_csFunc, b, 10),
			&pReturnValue);
		if(hr) { Printx(hr); return; }
		//Print(pReturnValue);

		s_clrHostStarted = true;
	}
	//Print(GetCurrentThreadId());

	Perf.Next();
	for(size_t i = 0; i < 1; i++) {
		r = s_csFunc(0);
		//Print(r);
		Perf.Next();
	}
	//Perf.Write();

	//Printx(s_pClrRuntimeHost->UnloadAppDomain(1, 1));
	//Printx(s_pClrRuntimeHost->Stop());
	//s_pClrRuntimeHost.Release();
	//s_clrHostStarted = false;
}
//
//void TestClrHost()
//{
//	DWORD pReturnValue;
//	Perf.First();
//	HRESULT hr;
//
//	if(!s_clrHostStarted) {
//	// build runtime
//		hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));
//		if(hr) { Printx(hr); return; }
//		hr = pMetaHost->GetRuntime(L"v4.0.30319", IID_PPV_ARGS(&pRuntimeInfo));
//		if(hr) { Printx(hr); return; }
//		hr = pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&s_pClrRuntimeHost));
//		if(hr) { Printx(hr); return; }
//
//		// start runtime
//		hr = s_pClrRuntimeHost->Start();
//		if(hr < 0) { Printx(hr); return; }
//
//		s_clrHostStarted = true;
//	}
//	//Print(GetCurrentThreadId());
//
//	Perf.Next();
//	for(size_t i = 0; i < 5; i++) {
//		hr = s_pClrRuntimeHost->ExecuteInDefaultAppDomain(
//			L"Q:\\app\\Catkeys\\Tests\\Catkeys.dll",
//			L"Catkeys.Acc",
//			L"TestClrHost",
//			L"TEST",
//			&pReturnValue);
//		if(hr) { Printx(hr); return; }
//
//		Perf.Next();
//	}
//	Perf.Write();
//	Print(pReturnValue);
//}

extern "C" __declspec(dllexport)
void AuCpp_TestClrHost()
{
	//TestClrHost();
}
