#pragma once
#include "stdafx.h"

#if _DEBUG

void Print(LPCWSTR s);
void Printf(LPCWSTR frm, ...);

inline void Print(LPCSTR s) { Printf(L"%S", s); }
inline void Print(const std::wstring& s) { Print(s.c_str()); }
inline void Print(const _bstr_t& s) { Print((LPWSTR)s); }
inline void Print(int i) { Printf(L"%i", i); }
inline void Print(unsigned int i) { Print((int)i); }
inline void Print(long i) { Print((int)i); }
inline void Print(unsigned long i) { Print((int)i); }
inline void Print(__int64 i) { Printf(L"%I64i", i); }
inline void Print(unsigned __int64 i) { Print((__int64)i); }
inline void Print(void* i) { Printf(sizeof(void*) == 8 ? L"%I64i" : L"%i", i); }

inline void Printx(int i) { Printf(L"0x%X", i); }
inline void Printx(__int64 i) { Printf(L"0x%I64X", i); }

#else
#define Print __noop
#define Printf __noop
#define Printx __noop
#endif

#if _DEBUG

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

bool IsOS64Bit();
bool IsProcess64Bit(DWORD pid, OUT bool& is);

inline bool IsThisProcess64Bit()
{
#ifdef _WIN64
	return true;
#else
	return false;
#endif
}
