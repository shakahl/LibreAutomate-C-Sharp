// UnmanagedDll.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "util.h"

//#include <OCIdl.h>
//#include <DispEx.h>
//
//
//extern "C" __declspec(dllexport)
//void TestSimple()
//{
//	//throw 5;
//	int* p = 0;
//	Print(*p);
//}
//
//extern "C" __declspec(dllexport)
//void TestUnmanaged(int& k, int& g)
//{
////MessageBox(0, L"", L"bb", 0);
//}
//
//extern "C" __declspec(dllexport)
//void TestStructBlit(POINT& p)
//{
//	Print(p.x);
//	p.y = 8;
//}
//
//struct STSTR
//{
//	int k;
//	LPWSTR s;
//};
//
//extern "C" __declspec(dllexport)
//void TestStructString(STSTR& p)
//{
//	Print(p.s);
//	Print(p.k);
//	//p.s = L"retstr";
//	//p.s = SysAllocString(L"retstr");
//	//p.s = (LPWSTR)CoTaskMemAlloc(10); CopyMemory(p.s, L"retstr", sizeof(L"retstr"));
//	p.s[0] = 'A';
//	p.k = 8;
//	Print(L"returning");
//}
//
//extern "C" __declspec(dllexport)
//void TestArray(int* p)
//{
//	Print(p[0]);
//	p[0] = 5;
//}
//
//extern "C" __declspec(dllexport)
//void TestArrayStr(LPWSTR* p)
//{
//	Print(p[0]);
//	*p[0] = 'A';
//}
//
//__interface __declspec(uuid("3AB5235E-2768-47A2-909A-B5852A9D1868"))
//	ITest :IUnknown
//{
//	HRESULT STDMETHODCALLTYPE Test1(int i);
//	//HRESULT STDMETHODCALLTYPE Test2(int* p);
//	HRESULT STDMETHODCALLTYPE TestOL(int* p);
//	HRESULT STDMETHODCALLTYPE TestNext(char* p);
//};
//
//class CTest :public ITest
//{
//	int _refCount;
//
//public:
//	CTest() {
//		_refCount = 1;
//	}
//
//	~CTest() {
//		Print(L"dtor");
//	}
//
//	HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void **ppvObject) {
//		wchar_t* name = L"?"; //info: QM2 has DebugGetInterfaceName
//		if (riid == __uuidof(ITest)) name = L"ITest"; //called
//		else if (riid == __uuidof(IUnknown)) name = L"IUnknown"; //called
//		else if (riid == __uuidof(IProvideClassInfo)) name = L"IProvideClassInfo"; //called
//		else if (riid == __uuidof(IDispatch)) name = L"IDispatch";
//		else if (riid == __uuidof(IDispatchEx)) name = L"IDispatchEx";
//		else if (riid == __uuidof(IErrorInfo)) name = L"IErrorInfo";
//		else if (riid == __uuidof(IEnumVARIANT)) name = L"IEnumVARIANT";
//		else if (riid == __uuidof(IConnectionPoint)) name = L"IConnectionPoint";
//		else if (riid == __uuidof(IConnectionPointContainer)) name = L"IConnectionPointContainer";
//
//		Printf(L"QueryInterface: %s", name);
//
//		if (riid != __uuidof(ITest) && riid != __uuidof(IUnknown)) {
//			*ppvObject = 0;
//			return E_NOINTERFACE;
//		}
//		_refCount++;
//		*ppvObject = this;
//		return 0;
//	}
//	ULONG STDMETHODCALLTYPE AddRef() {
//		Printf(L"AddRef, %i", ++_refCount);
//		return _refCount;
//	}
//	ULONG STDMETHODCALLTYPE Release() {
//		_refCount--;
//		Printf(L"Release, %i", _refCount);
//		if (_refCount == 0) delete this;
//		return _refCount;
//	}
//
//	HRESULT STDMETHODCALLTYPE Test1(int i)
//	{
//		Print(L"Test1");
//		return 0;
//	}
//
//	//HRESULT STDMETHODCALLTYPE Test2(int* p)
//	//{
//	//	Print(p[0]);
//	//	return 0;
//	//}
//
//	HRESULT STDMETHODCALLTYPE TestOL(int* p)
//	{
//		Print(1);
//		return 0;
//	}
//
//	HRESULT STDMETHODCALLTYPE TestNext(char* p)
//	{
//		Print(2);
//		return 0;
//	}
//};
//
//extern "C" __declspec(dllexport)
//ITest* CreateTestInterface()
//{
//	return new CTest();
//}
//
//extern "C" __declspec(dllexport)
//void CreateTestInterface2(ITest*& t)
//{
//	t = new CTest();
//}
//
//
//
//
//
//
//#include <OleAuto.h>
//#include <oleacc.h>
//#pragma comment(lib, "oleacc.lib")
//
//
//extern "C" __declspec(dllexport)
//void DllTestAcc()
//{
//	POINT p = { 257, 1138 };
//	IAccessible* a = 0; VARIANT vc = {};
//	HRESULT hr = AccessibleObjectFromPoint(p, &a, &vc);
//	if (hr != 0) {
//		Printx(hr); return;
//	}
//	try {
//		BSTR s = 0;
//		hr = a->get_accName(vc, &s);
//		if (hr != 0) {
//			Printx(hr); return;
//		}
//
//		Print(s);
//		//Print((__int64)s);
//		//SysFreeString(s);
//
//	}
//	catch (...) {
//		a->Release();
//		Print(L"exception");
//	}
//}

