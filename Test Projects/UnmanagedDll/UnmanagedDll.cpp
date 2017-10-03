// UnmanagedDll.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include <OCIdl.h>
#include <DispEx.h>


#pragma region UTIL

void zz(LPCWSTR s)
{
	//return;

	//HWND QM = FindWindow(L"QM_Editor", 0); if (!QM) return;
	//SendMessage(QM, WM_SETTEXT, -1, (LPARAM)s);

	_cputws(s);
	_cputws(L"\r\n");
}
void zp(LPCWSTR frm, ...)
{
	wchar_t b[1032];
	wvsprintf(b, frm, (va_list)(&frm + 1));
	//_vsnwprintf(b, 1027, frm, (va_list)(&frm + 1));
	zz(b);
}
void zz(__int64 i) { zp(L"%I64i", i); }
void zx(__int64 i) { zp(L"0x%I64X", i); }

#pragma endregion

extern "C" __declspec(dllexport)
void TestSimple()
{
	//throw 5;
	int* p = 0;
	zz(*p);
}

extern "C" __declspec(dllexport)
void TestUnmanaged(int& k, int& g)
{
//MessageBox(0, L"", L"bb", 0);
}

extern "C" __declspec(dllexport)
void TestStructBlit(POINT& p)
{
	zz(p.x);
	p.y = 8;
}

struct STSTR
{
	int k;
	LPWSTR s;
};

extern "C" __declspec(dllexport)
void TestStructString(STSTR& p)
{
	zz(p.s);
	zz(p.k);
	//p.s = L"retstr";
	//p.s = SysAllocString(L"retstr");
	//p.s = (LPWSTR)CoTaskMemAlloc(10); CopyMemory(p.s, L"retstr", sizeof(L"retstr"));
	p.s[0] = 'A';
	p.k = 8;
	zz(L"returning");
}

extern "C" __declspec(dllexport)
void TestArray(int* p)
{
	zz(p[0]);
	p[0] = 5;
}

extern "C" __declspec(dllexport)
void TestArrayStr(LPWSTR* p)
{
	zz(p[0]);
	*p[0] = 'A';
}

__interface __declspec(uuid("3AB5235E-2768-47A2-909A-B5852A9D1868"))
	ITest :IUnknown
{
	HRESULT STDMETHODCALLTYPE Test1(int i);
	//HRESULT STDMETHODCALLTYPE Test2(int* p);
	HRESULT STDMETHODCALLTYPE TestOL(int* p);
	HRESULT STDMETHODCALLTYPE TestNext(char* p);
};

class CTest :public ITest
{
	int _refCount;

public:
	CTest() {
		_refCount = 1;
	}

	~CTest() {
		zz(L"dtor");
	}

	HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void **ppvObject) {
		wchar_t* name = L"?"; //info: QM2 has DebugGetInterfaceName
		if (riid == __uuidof(ITest)) name = L"ITest"; //called
		else if (riid == __uuidof(IUnknown)) name = L"IUnknown"; //called
		else if (riid == __uuidof(IProvideClassInfo)) name = L"IProvideClassInfo"; //called
		else if (riid == __uuidof(IDispatch)) name = L"IDispatch";
		else if (riid == __uuidof(IDispatchEx)) name = L"IDispatchEx";
		else if (riid == __uuidof(IErrorInfo)) name = L"IErrorInfo";
		else if (riid == __uuidof(IEnumVARIANT)) name = L"IEnumVARIANT";
		else if (riid == __uuidof(IConnectionPoint)) name = L"IConnectionPoint";
		else if (riid == __uuidof(IConnectionPointContainer)) name = L"IConnectionPointContainer";

		zp(L"QueryInterface: %s", name);

		if (riid != __uuidof(ITest) && riid != __uuidof(IUnknown)) {
			*ppvObject = 0;
			return E_NOINTERFACE;
		}
		_refCount++;
		*ppvObject = this;
		return 0;
	}
	ULONG STDMETHODCALLTYPE AddRef() {
		zp(L"AddRef, %i", ++_refCount);
		return _refCount;
	}
	ULONG STDMETHODCALLTYPE Release() {
		_refCount--;
		zp(L"Release, %i", _refCount);
		if (_refCount == 0) delete this;
		return _refCount;
	}

	HRESULT STDMETHODCALLTYPE Test1(int i)
	{
		zz(L"Test1");
		return 0;
	}

	//HRESULT STDMETHODCALLTYPE Test2(int* p)
	//{
	//	zz(p[0]);
	//	return 0;
	//}

	HRESULT STDMETHODCALLTYPE TestOL(int* p)
	{
		zz(1);
		return 0;
	}

	HRESULT STDMETHODCALLTYPE TestNext(char* p)
	{
		zz(2);
		return 0;
	}
};

extern "C" __declspec(dllexport)
ITest* CreateTestInterface()
{
	return new CTest();
}

extern "C" __declspec(dllexport)
void CreateTestInterface2(ITest*& t)
{
	t = new CTest();
}






#include <OleAuto.h>
#include <oleacc.h>
#pragma comment(lib, "oleacc.lib")


extern "C" __declspec(dllexport)
void DllTestAcc()
{
	POINT p = { 257, 1138 };
	IAccessible* a = 0; VARIANT vc = {};
	HRESULT hr = AccessibleObjectFromPoint(p, &a, &vc);
	if (hr != 0) {
		zx(hr); return;
	}
	try {
		BSTR s = 0;
		hr = a->get_accName(vc, &s);
		if (hr != 0) {
			zx(hr); return;
		}

		zz(s);
		//zz((__int64)s);
		//SysFreeString(s);

	}
	catch (...) {
		a->Release();
		zz(L"exception");
	}
}

