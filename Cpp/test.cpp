// UnmanagedDll.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "cpp.h"

#include <atlfile.h>

extern HMODULE s_moduleHandle;

extern "C" __declspec(dllexport)
void Cpp_Test()
{
	//CAtlList<int> _a;
	//_a.AddTail(1);
	//_a.AddTail(2);
	//for(auto pos = _a.GetHeadPosition(); pos != null;) {
	//	Print(_a.GetNext(ref pos));
	//}

	//PRINTX(10);
	//PRINTF(L"%s %i", L"MOO", 10);

	////Print(IsOS64Bit());

	////HWND w = FindWindow(L"Chrome_WidgetWin_1", null);
	//HWND w = FindWindow(L"QM_Editor", null);
	//if(!w) {
	//	Print("window not found");
	//	return;
	//}
	//DWORD pid, tid = GetWindowThreadProcessId(w, &pid);
	////bool is64bit, ok = IsProcess64Bit(pid, is64bit);
	////Printf(L"ok=%i is64=%i", ok, is64bit);

	//CComBSTR s;
}


//extern "C" __declspec(dllexport)
//int Cpp_TestAdd(int a, int b)
//{
//	return a + b;
//}
//
//extern "C" __declspec(dllexport)
//bool Cpp_TestAdd2(STR a, STR b)
//{
//	return a==b;
//}
//
//extern "C" __declspec(dllexport)
//bool Cpp_TestAdd3(STR a, STR b, size_t len)
//{
//	return a==b;
//}
//
//
//
//#define IID_ICppTest __uuidof(ICppTest)
//__interface  __declspec(uuid("3426CF3C-F7C2-4322-A292-463DB8729B54"))
//	ICppTest :IUnknown
//{
//	bool One(STR a, STR b);
//};
//
//#define STD_IUNKNOWN_METHODS_SIMPLE(iface) \
//STDMETHODIMP QueryInterface(REFIID iid, void** ppv)\
//{\
//	if(iid == IID_IUnknown || iid == IID_##iface) { *ppv = this; return S_OK; }\
//	else { *ppv = 0; return E_NOINTERFACE; }\
//}\
//STDMETHODIMP_(ULONG) AddRef() { return 1; }\
//STDMETHODIMP_(ULONG) Release() { return 1; }
//
//class CppTest :public ICppTest
//{
//	STD_IUNKNOWN_METHODS_SIMPLE(ICppTest)
//public:
//	bool One(STR a, STR b)
//	{
//		return a == b;
//	}
//};
//
//extern "C" __declspec(dllexport)
//ICppTest* Cpp_Interface()
//{
//	return new CppTest();
//}



extern "C" __declspec(dllexport)
void Cpp_TestSimpleStringBuilder(STR s, STR w)
{
	Perf.First();
	SimpleStringBuilder b;
	//b.Append(L"one ");
	//b.Append(50);
	//b.Append(L" four", 2);
	//b.Append(L" 0x");
	//b.Append(10, 16);
	//b.AppendChar(' ');
	//b.AppendChar('A');
	//b.AppendChar(' ', 4);
	//b.AppendChar('A');

	HWND ww = 0;

	static const STR rundll = L"\\SysWOW64\\rundll32.exe \"";
	static const STR bits = L"32";

	LPWSTR t; int n;
	t = b.GetBufferToAppend(out n); b.FixBuffer(GetWindowsDirectoryW(t, n));
	b << rundll;
	t = b.GetBufferToAppend(out n); b.FixBuffer(GetModuleFileNameW(s_moduleHandle, t, n));
	auto u = wcsrchr(t, '\\') - 5; u[0] = bits[0]; u[1] = bits[1]; //"32" to "64" or vice versa
	if(GetFileAttributes(t) == INVALID_FILE_ATTRIBUTES) return; //avoid messagebox when our antimatter dll does not exist
	b << L"\",Cpp_RunDll " << (__int64)ww; //note: without W

	Perf.Next();
	Print(b.str());
	Perf.NW();
}

extern "C" __declspec(dllexport)
void Cpp_TestWildex(STR s, STR w)
{
	auto lenS = wcslen(s);
	auto lenW = wcslen(w);

	Perf.First();
	CppWildex x;
	LPWSTR es = null;
	if(!x.Parse(w, lenW, false, &es)) { Print(es); free(es); return; }
	Perf.Next();

	bool yes;
	for(int i = 0; i < 1000; i++) yes = x.Match(s, lenS);
	Perf.NW();

	Print(yes);
}

extern "C" __declspec(dllexport)
void Cpp_TestPCRE(STR s, STR p, DWORD flags)
{
	int rc = 0;

	Perf.First();
	int errNum; size_t errOffs;
	auto re = pcre2_compile(p, -1, flags | PCRE2_UTF, &errNum, &errOffs, null);
	if(re == null) {
		PCRE2_UCHAR buffer[256];
		pcre2_get_error_message(errNum, buffer, _countof(buffer));
		Printf(L"error %s at %i", buffer, errOffs);
		return;
	}

#if false
	Perf.Next();
	LPBYTE ser = null; size_t serSize = 0;
	pcre2_code* a[2] = { re,re };
	rc = pcre2_serialize_encode((const pcre2_code_16 **)a, 2, &ser, &serSize, null);
	if(rc <= 0) {
		Print(L"pcre2_serialize_encode error");
		return;
	}
	pcre2_code_free(re); re = null;
	Perf.Next();

	Print(serSize);

	rc = pcre2_serialize_decode(&re, 1, ser, null);
	if(rc <= 0) {
		Print(L"pcre2_serialize_decode error");
		return;
	}

	pcre2_serialize_free(ser);
#endif

	Perf.Next();
	auto m = pcre2_match_data_create_from_pattern(re, null);
	auto len = wcslen(s);

	for(int i = 0; i < 1000; i++) {
		rc = pcre2_match(re, s, len, 0, 0, m, null);
		if(rc <= 0) {
			Print(rc);
			return;
		}
	}

	Perf.Next();

	//auto v = pcre2_get_ovector_pointer(m);
	//for(int i = 0; i < rc; i++) {
	//	PCRE2_SPTR substring_start = s + v[2 * i];
	//	size_t substring_length = v[2 * i + 1] - v[2 * i];
	//	Printf(L"%i: %.*s", i, (int)substring_length, (char *)substring_start);
	//}

	pcre2_match_data_free(m);
	pcre2_code_free(re);
	Perf.NW();
}


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

