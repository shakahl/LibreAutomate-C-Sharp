// UnmanagedDll.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "cpp.h"

#if _DEBUG
//#if 1

//#include <atlstr.h>
//#include <atlfile.h>

extern HMODULE s_moduleHandle;

void TestStringBuilder()
{
	Perf.First();
	str::StringBuilder b;
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

	b<<L"kkk"<<L' '<<5<<',';
	b.AppendChar(' ');
	b.AppendChar(L' ');

	LPWSTR t; int n;
	t = b.GetBufferToAppend(out n); b.FixBuffer(GetWindowsDirectoryW(t, n));
	b << rundll;
	t = b.GetBufferToAppend(out n); b.FixBuffer(GetModuleFileNameW(s_moduleHandle, t, n));
	auto u = wcsrchr(t, '\\') - 5; u[0] = bits[0]; u[1] = bits[1]; //"32" to "64" or vice versa
	if(GetFileAttributes(t) == INVALID_FILE_ATTRIBUTES) return; //avoid messagebox when our antimatter dll does not exist
	b << L"\",Cpp_RunDll " << (__int64)ww; //note: without W

	b << 'A';
	b.AppendChar('.', 4);

	Perf.Next();
	Print(b);
	Perf.NW();
}

EXPORT void Cpp_TestWildex(STR s, STR w)
{
	auto lenS = wcslen(s);
	auto lenW = wcslen(w);

	Perf.First();
	str::Wildex x;
	Bstr es;
	if(!x.Parse(w, lenW, false, &es)) { Print(es); return; }
	Perf.Next();

	bool yes;
	for(int i = 0; i < 1000; i++) yes = x.Match(s, lenS);
	Perf.NW();

	Print(yes);
}

EXPORT void Cpp_TestPCRE(STR s, STR p, DWORD flags)
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


//class TestTL {
//public:
//	TestTL() {
//		Printf(L"+ %p %i", this, GetCurrentThreadId());
//	}
//
//	~TestTL() {
//		Printf(L"- %p %i", this, GetCurrentThreadId());
//	}
//};
//
//void Voo(LPWSTR s) {
//	//std::wstring k(s);
//	//Print(k.c_str() == null);
//	CString g(s);
//	//Print(g.GetString() == null);
//	//Print((STR)g == null);
//}
//
//enum
//	//class
//	eTest {
//	One = 1,
//	Two = 2,
//	Four = 4,
//};
//ENABLE_BITMASK_OPERATORS(eTest);
//
//#include <vector>
//#include <array>
//
//STR s_testSTR = L"te";
//
//struct eKKK {
//	enum {
//		One = 1,
//		Two = 2,
//		Four = 4,
//	};
//
//	//static const int One = 1;
//	//static const int Two = 2;
//	//static const int Four = 4;
//};
//
//enum class eKKK {
//	One = 1,
//	Two = 2,
//	Four = 4,
//	Eight = 8,
//};
//ENABLE_BITMASK_OPERATORS(eKKK);

extern "C" __declspec(dllexport)
int Cpp_TestInt(int a)
{
	return 1;
}

extern "C" __declspec(dllexport)
int Cpp_TestString(STR a)
{
	return 1;
}


#define STD_IUNKNOWN_METHODS_SIMPLE(iface) \
STDMETHODIMP QueryInterface(REFIID iid, void** ppv)\
{\
	if(iid == IID_IUnknown || iid == IID_##iface) { *ppv = this; return S_OK; }\
	else { *ppv = 0; return E_NOINTERFACE; }\
}\
STDMETHODIMP_(ULONG) AddRef() { return 1; }\
STDMETHODIMP_(ULONG) Release() { return 1; }

#define IID_ICppTest __uuidof(ICppTest)
__interface  __declspec(uuid("3426CF3C-F7C2-4322-A292-463DB8729B54"))
	ICppTest :IUnknown
{
	STDMETHODIMP TestInt(int a, int b, int c);
	STDMETHODIMP TestString(STR a, int b, int c);
	STDMETHODIMP TestBSTR(BSTR a, int b, int c);
};

class CppTest :public ICppTest
{
	STD_IUNKNOWN_METHODS_SIMPLE(ICppTest)
public:
	STDMETHODIMP TestInt(int a, int b, int c)
	{
		return E_NOTIMPL;
	}
	STDMETHODIMP TestString(STR a, int b, int c)
	{
		return E_NOTIMPL;
	}
	STDMETHODIMP TestBSTR(BSTR a, int b, int c)
	{
		return E_NOTIMPL;
	}
};

extern "C" __declspec(dllexport)
ICppTest* Cpp_Interface()
{
	return new CppTest();
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
//		Printf(L"dtor thread: %i", GetCurrentThreadId());
//		//Print(L"dtor");
//	}
//
//	HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void **ppvObject) {
//		//wchar_t* name = L"?"; //info: QM2 has DebugGetInterfaceName
//		//if (riid == __uuidof(ITest)) name = L"ITest"; //called
//		//else if (riid == __uuidof(IUnknown)) name = L"IUnknown"; //called
//		//else if (riid == __uuidof(IProvideClassInfo)) name = L"IProvideClassInfo"; //called
//		//else if (riid == __uuidof(IDispatch)) name = L"IDispatch";
//		//else if (riid == __uuidof(IDispatchEx)) name = L"IDispatchEx";
//		//else if (riid == __uuidof(IErrorInfo)) name = L"IErrorInfo";
//		//else if (riid == __uuidof(IEnumVARIANT)) name = L"IEnumVARIANT";
//		//else if (riid == __uuidof(IConnectionPoint)) name = L"IConnectionPoint";
//		//else if (riid == __uuidof(IConnectionPointContainer)) name = L"IConnectionPointContainer";
//
//		//Printf(L"QueryInterface: %s", name);
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
//		++_refCount;
//		//Printf(L"AddRef, %i", _refCount);
//		return _refCount;
//	}
//	ULONG STDMETHODCALLTYPE Release() {
//		_refCount--;
//		//Printf(L"Release, %i", _refCount);
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
//EXPORT void CreateTestInterface2(ITest*& t)
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

class One
{
public:
	int m;
	One() {
		m = 1;
	}
	virtual ~One() {
		Printf(L"~One, %i", m);
	}
};

class Two : public One
{
public:
	~Two() {
		Print(L"~Two");
		m = 0;
	}
};

class Move
{
public:
	int m;

	Move() {
		m = 1;
	}

	//~Move() {
	//	Printf(L"~Move, %i", m);
	//}

	Move(Move& x) {
		m = x.m;
	}

	Move(Move&& x) {
		m = x.m;
		x.m = 0;
	}

	Move& operator=(Move& x) {
		m = x.m;
		return *this;
	}

	Move& operator=(Move&& x) {
		m = x.m;
		x.m = 0;
		return *this;
	}
};

class Move2 :public Move
{
public:
	Move2() : Move() {}

	~Move2() {
		Printf(L"~Move2, %i", m);
	}

	Move2(Move2& x) {
		m = x.m;
	}

	Move2(Move2&& x) {
		m = x.m;
		x.m = 0;
	}

	Move2& operator=(Move2& x) {
		m = x.m;
		return *this;
	}

	Move2& operator=(Move2&& x) {
		Move::operator=(std::move(x));
		return *this;
	}
};

EXPORT void Cpp_Test()
{

	//str::StringBuilder t;
	////STR s = L"TEST";
	//LPWSTR s = L"TEST";
	//t.Append(s);
	//Print(t);

	//Move2 t1;
	//t1.m = 5;

	////Move2 t2 = t1;
	////Move2 t2; t2 = t1;

	////Move2 t2 = std::move(t1);
	//Move2 t2; t2 = std::move(t1);

	//Printf(L"%i %i", t1.m, t2.m);

	//str::StringBuilder b;
	////b.Append(L"---");
	////b.Append(L"123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 123456789 ");
	//for(int i = 0; i < 1000; i++) {
	//	b << i;
	//	b << L"    ";
	//}
	//
	//Print(b);

	//for(int i = 0; i < 5; i++) {
	//	Sleep(100);
	//	Perf.First();
	//	IStreamPtr x;
	//	CreateStreamOnHGlobal(0, true, &x);
	//	for(int j = 0; j < 100000; j++) if(x->Write("abcdefgh", 8, null)) { Print("failed"); return; }

	//	Perf.NW();

	//	DWORD size = 0; istream::GetSize(x, size); Print(size);
	//}


	//eKKK e = eKKK::One | eKKK::Two;
	//eKKK e = eKKK::One;

	//Print(!!e);

	////if(e&eKKK::One) Print(1);
	//if((bool)(e & eKKK::One)) Print(1);
	//if(ALL(e, eKKK::One)) Print(2);
	//if((bool)(e & (eKKK::One | eKKK::Four))) Print(3);
	////if(ANY(e, eKKK::One, eKKK::Four)) Print(4);
	//if(ANY(e, eKKK::One | eKKK::Four)) Print(4);

	//eAF2 f = eAF::eAF2::InWebPage| eAF::eAF2::Java;

	//Printf(L"%*sa", 4, L"");

	//HWND w = FindWindowW(L"QM_Editor", null);
	////int x = 7;
	////wnd::EnumChildWindows(w, [](HWND c) { wnd::PrintWnd(c); return true; });

	//STR s_testSTR = L"thames";

	//Print(str::Switch(s_testSTR, 6, { L"moo", L"te", L"thames", L"skipRoles" }));

	////Print(wnd::ClassNameIs(w, { L"moo", L"qm_*itor", L"khy" }));

	//TestStringBuilder();

	//Bstr b1;
	//Print((void*)b1.m_str);
	//Bstr b2(L"test");
	//Print(b2);

	//Bstr s;
	//Print(wnd::ClassName(w, s));
	//Print(s);
	//Print(wnd::Name(w, s));
	//Print(s);

	//Print(sizeof(CSimpleArray<int>));
	//Print(sizeof(CAtlArray<int>));
	//Print(sizeof(std::vector<int>));
	//Print(sizeof(std::array<int, 100>));

	//Print(str::Switch(L"two", 3, L"one", L"two", L"three", null));

	//#if true
	//	eTest t = eTest::One | eTest::Two;
	//	//eTest t = eTest::Two;
	//	//t = (eTest)0;
	//	Print(t); //error if 'enum class'
	//	Print((int)t);
	//
	//	if(t) Print("t is not 0"); //error if 'enum class'
	//	if(t&eTest::Two) Print("has Two"); //error if 'enum class'
	//	if(!t) Print("t is 0");
	//	if(!!t) Print("t is not 0");
	//	if(!!(t&eTest::Two)) Print("has Two");
	//
	//	if((t&eTest::Two) == eTest::Two) Print("has Two");
	//	t |= eTest::Four;
	//	if((t&(eTest::Two | eTest::Four)) == (eTest::Two | eTest::Four)) Print("has (Two|Four)");
	//	Print((int)(t&~eTest::Two));
	//#else
	//	eTest t = One | Two;
	//	//eTest t = Two;
	//	Print(t);
	//	if(t&Two) Print("has Two");
	//	if(t&Two == Two) Print("has Two");
	//	t |= Four;
	//	if(t&(Two | Four) == (Two | Four)) Print("has (Two|Four)");
	//	Print(t&~Two);
	//#endif
	//
	//	POINT p = {}, pp=p;
	//	//int p = 5, pp = p;
	//	//p |= pp;
	//	//if(!p) Print(1);

	//Print(sizeof(std::wstring));
	//Print(sizeof(CString));

	//Perf.First();
	//for(size_t i = 0; i < 100000; i++) {
	////for(size_t i = 0; i < 1; i++) {
	//	Voo(L"1234567890");
	//	//Voo(null);
	//}
	//Perf.NW();

	//static TestTL s;
	//static thread_local TestTL t;

	//CSimpleMap<int, int> m;
	//m.Add(1, 10);
	//m.Add(2, 20);
	//Print(m.FindKey(2));

	//void* p = null;
	//for(int i = 1; i < 20; i++) {
	//	p = _recalloc(p, i, 8);
	//	Printf(L"%i %p", i, p);
	//}

	//CAtlMap<int, int> m;


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

	//Bstr s;
}

#include "IAccessible2.h"

_COM_SMARTPTR_TYPEDEF(IAccessible2, __uuidof(IAccessible2));

void InProcAccTest(IAccessible* a)
{
	//Print(L"TEST");
	IAccessible2Ptr a2;
	if(!QueryService(a, &a2, &IID_IAccessible)) { Print("QS failed"); return; }

	//AccessibleStates states;
	//if(a2->get_states(&states)) { Print("get_states failed"); return; }
	//Printx(states);

	//long n;
	//if(a2->get_nExtendedStates(&n)) { Print("get_nExtendedStates failed"); return; } //fails
	//Print(n);
}

#endif //#if _DEBUG
