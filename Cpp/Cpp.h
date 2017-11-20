#pragma once
#include "stdafx.h"

#define PRINT_ALWAYS 1 //TODO
#if _DEBUG || PRINT_ALWAYS

void Print(STR s);
void Printf(STR frm, ...);

inline void Print(LPCSTR s) { Printf(L"%S", s); }
inline void Print(const std::wstring& s) { Print(s.c_str()); }
inline void Print(int i) { Printf(L"%i", i); }
inline void Print(unsigned int i) { Print((int)i); }
inline void Print(long i) { Print((int)i); }
inline void Print(unsigned long i) { Print((int)i); }
inline void Print(__int64 i) { Printf(L"%I64i", i); }
inline void Print(unsigned __int64 i) { Print((__int64)i); }
inline void Print(void* i) { Printf(sizeof(void*) == 8 ? L"%I64i" : L"%i", i); }

inline void Printx(int i) { Printf(L"0x%X", i); }
inline void Printx(__int64 i) { Printf(L"0x%I64X", i); }

#define PRINT(x) Printf(L"debug: " __FILEW__ "(" _CRT_STRINGIZE(__LINE__) "):  %i", x)
#define PRINTX(x) Printf(L"debug: " __FILEW__ "(" _CRT_STRINGIZE(__LINE__) "):  0x%X", x)
#define PRINTF(formatString, ...) Printf(L"debug: " __FILEW__ "(" _CRT_STRINGIZE(__LINE__) "):  " formatString, __VA_ARGS__)

inline void PrintComRefCount(IUnknown* u) {
	if(u) {
		u->AddRef();
		int i = u->Release();
		Printf(L"%p  %i", u, i);
	} else Print(L"null");
}

#else
#define Print __noop
#define Printf __noop
#define Printx __noop
#define PRINT __noop
#define PRINTX __noop
#define PRINTF __noop
#define PrintComRefCount __noop
#endif

#if _DEBUG || PRINT_ALWAYS

struct Perf_Inst
{
//private: //somehow then this does not work: '#pragma data_seg (".shared")'
	int _counter;
	bool _incremental;
	int _nMeasurements; //used with incremental to display n measurements and average times
	__int64 _time0;
	static const int _nElem = 16;
	__int64 _a[_nElem];
	wchar_t _aMark[_nElem];

public:
	void First();
	void Next(char cMark = '\0');
	void Write();

	void NW(char cMark = '\0') { Next(cMark); Write(); }

	void SetIncremental(bool yes)
	{
		if(_incremental = yes) {
			for(int i = 0; i < _nElem; i++) _a[i] = 0;
			_nMeasurements = 0;
		}
	}
};

extern Perf_Inst Perf;

#endif


#define ZEROTHIS memset(this, 0, sizeof(*this))

#include "cpp_string.h"


bool IsOS64Bit();
bool IsProcess64Bit(DWORD pid, out bool& is);

inline bool IsThisProcess64Bit()
{
#ifdef _WIN64
	return true;
#else
	return false;
#endif
}

//SECURITY_ATTRIBUTES that allows UAC low integrity level processes to open the kernel object.
//can instead use CSecurityAttributes/CSecurityDesc, but this added before including ATL, and don't want to change now.
class SecurityAttributes
{
	DWORD nLength;
	LPVOID lpSecurityDescriptor;
	BOOL bInheritHandle;
public:
	
	SecurityAttributes()
	{
		nLength = sizeof(SecurityAttributes);
		bInheritHandle = false;
		lpSecurityDescriptor = null;
		BOOL ok = ConvertStringSecurityDescriptorToSecurityDescriptorW(L"D:NO_ACCESS_CONTROLS:(ML;;NW;;;LW)", 1, &lpSecurityDescriptor, null);
		assert(ok);
	}

	~SecurityAttributes()
	{
		LocalFree(lpSecurityDescriptor);
	}

	//SECURITY_ATTRIBUTES* operator&() {
	//	return (SECURITY_ATTRIBUTES*)this;
	//}

	static SECURITY_ATTRIBUTES* Common()
	{
		static SecurityAttributes s_sa;
		return (SECURITY_ATTRIBUTES*)&s_sa;
	}
};

//can instead use CHandle, but this is easier to use, eg has operator KernelHandle=HANDLE, and knows about INVALID_HANDLE_VALUE.
class KernelHandle
{
protected:
	HANDLE _h;

public:
	KernelHandle() noexcept { _h = 0; }

	KernelHandle(HANDLE h) noexcept {
		_h = h == INVALID_HANDLE_VALUE ? 0 : h;
	}

	~KernelHandle() {
		if(_h) CloseHandle(_h);
	}

	operator HANDLE() { return _h; }
	HANDLE* operator &() { return &_h; }

	void operator=(HANDLE h) {
		if(_h) CloseHandle(_h);
		_h = h == (HANDLE)(-1) ? 0 : h;
	}

	bool Is0() {
		return _h == 0;
	}

	//HANDLE Detach() {
	//	auto r = _h; _h = 0;
	//	return r;
	//}
};

class AutoReleaseMutex
{
	HANDLE _mutex;
public:
	AutoReleaseMutex(HANDLE mutex) noexcept {
		_mutex = mutex;
	}

	~AutoReleaseMutex() {
		if(_mutex) ReleaseMutex(_mutex);
	}

	void ReleaseNow() {
		if(_mutex) ReleaseMutex(_mutex);
		_mutex = 0;
	}
};

//can instead use CAtlFileMappingBase from atlfile.h, but this added before including ATL, and don't want to change now.
class SharedMemory
{
	HANDLE _hmapfile;
	LPBYTE _mem;
public:
	SharedMemory() { _hmapfile = 0; _mem = 0; }
	~SharedMemory() { Close(); }

	bool Create(STR name, DWORD size)
	{
		Close();
		_hmapfile = CreateFileMappingW((HANDLE)(-1), SecurityAttributes::Common(), PAGE_READWRITE, 0, size, name);
		if(!_hmapfile) return false;
		_mem = (LPBYTE)MapViewOfFile(_hmapfile, FILE_MAP_ALL_ACCESS, 0, 0, 0);
		return _mem != null;
	}

	bool Open(STR name)
	{
		Close();
		_hmapfile = OpenFileMappingW(FILE_MAP_ALL_ACCESS, 0, name);
		if(!_hmapfile) return false;
		_mem = (LPBYTE)MapViewOfFile(_hmapfile, FILE_MAP_ALL_ACCESS, 0, 0, 0);
		return _mem != null;
	}

	void Close()
	{
		if(_mem) { UnmapViewOfFile(_mem); _mem = 0; }
		if(_hmapfile) { CloseHandle(_hmapfile); _hmapfile = 0; }
	}

	LPBYTE Mem() { return _mem; }

	bool Is0() { return _mem == null; }
};

//TODO: currently not used, remove if don't need
template<class T>
class AutoResetVariable
{
	T* _b;
public:
	AutoResetVariable(T* b, T value) { _b = b; *b = value; }
	~AutoResetVariable() { *_b = 0; }
};

const int c_accVersion = 1;

struct _AccFindParams
{
	int size;
	int hwnd; //not HWND, because it must be of same size in 32 and 64 bit process
private:
	int _roleLen; //with '\0', or 0 if null
	int _nameLen; //with '\0', or 0 if null
public:
	int flags;
	bool findAll;
	int skip;

	STR Role() { return _roleLen ? (STR)(this + 1) : null; }
	STR Name() { return _nameLen ? (STR)(this + 1) + _roleLen : null; }

	static int CalcMemSize(int roleLen, int nameLen) { return sizeof(_AccFindParams) + (roleLen + nameLen) * 2; }
	void SetRole(STR role, int roleLen) { _roleLen = roleLen; if(roleLen) memcpy((void*)Role(), role, roleLen * 2); }
	void SetName(STR name, int nameLen) { _nameLen = nameLen; if(nameLen) memcpy((void*)Name(), name, nameLen * 2); }
};

struct Cpp_Acc
{
	IAccessible* iacc;
	long elem;

	Cpp_Acc() {
		iacc = null;
		elem = 0;
	}

	Cpp_Acc(IAccessible* iacc_, long elem_) {
		iacc = iacc_;
		elem = elem_;
	}
};

class AccCallback
{
public:
	virtual bool IsClientStillWaiting(bool fullCheck) = 0;
	virtual void WriteResult(IAccessible* iacc, long elem) = 0;
	virtual void Finished() = 0;
};
