// UnmanagedDll.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"


#pragma region UTIL

void zz(LPCWSTR s)
{
	HWND QM = FindWindow(L"QM_Editor", 0); if (!QM) return;
	SendMessage(QM, WM_SETTEXT, -1, (LPARAM)s);
}
void zp(LPCWSTR frm, ...)
{
	wchar_t b[1028]; wvsprintf(b, frm, (va_list)(&frm + 1));
	zz(b);
}
void zz(int i) { zp(L"%i", i); }
void zz(void* v) { zp(L"%i", v); }

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
	HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void **ppvObject) {
		zp(L"QueryInterface: IUnknown=%i ITest=%i", riid == __uuidof(IUnknown), riid == __uuidof(ITest));
		if (riid != __uuidof(ITest) && riid != __uuidof(IUnknown)) {
			*ppvObject = 0;
			return E_NOINTERFACE;
		}
		*ppvObject = this;
		return 0;
	}
	ULONG STDMETHODCALLTYPE AddRef() {
		zz(L"AddRef");
		return 1;
	}
	ULONG STDMETHODCALLTYPE Release() {
		zz(L"Release");
		return 1;
	}

	HRESULT STDMETHODCALLTYPE Test1(int i)
	{
		zz(i);
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
