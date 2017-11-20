#include "stdafx.h"
#include "util.h"

#pragma region print

HWND s_QM2;

void Print(LPCWSTR s)
{
#if true
	if(!IsWindow(s_QM2)) {
		s_QM2 = FindWindow(L"QM_Editor", 0); if(!s_QM2) return;
	}
	SendMessage(s_QM2, WM_SETTEXT, -1, (LPARAM)(s ? s : L""));
#else
	_cputws(s);
	_cputws(L"\r\n");
#endif
}

void Printf(LPCWSTR frm, ...)
{
	wchar_t b[1032];
	wvsprintf(b, frm, (va_list)(&frm + 1));
	//_vsnwprintf(b, 1027, frm, (va_list)(&frm + 1));
	Print(b);
}

#pragma endregion


#if _DEBUG

//class SharedMemory
//{
//
//};

void Perf_Inst::First()
{
	QueryPerformanceCounter((LARGE_INTEGER*)&_time0);
	_counter = 0;
	_nMeasurements++;
}

void Perf_Inst::Next(char cMark/* = '\0'*/)
{
	int n = _counter; if(n >= _nElem) return;
	_counter++;
	__int64 t; QueryPerformanceCounter((LARGE_INTEGER*)&t);
	t -= _time0;
	if(_incremental) _a[n] += t; else _a[n] = t;
	_aMark[n] = cMark;
}

void Perf_Inst::Write()
{
	int i, n = _counter;
	if(n == 0) return;
	if(n > _nElem) n = _nElem;
	__int64 f; QueryPerformanceFrequency((LARGE_INTEGER*)&f); double freq = 1000000.0 / f;
	bool average = false; int nMeasurements = 1;

	std::wstringstream s;
	s << L"speed:";

g1:
	double t = 0.0, tPrev = 0.0;
	for(i = 0; i < n; i++) {
		s << "  ";
		if(_aMark[i] != '\0') {
			s << _aMark[i];
			s << '=';
		}
		t = freq * _a[i];
		double d = t - tPrev;
		if(average) d /= nMeasurements;
		s << (__int64)d;
		tPrev = t;
	}

	if(n > 1) {
		s << "  (";
		if(average) t /= nMeasurements;
		s << (__int64)t;
		s << ")";
	}

	if(!average && _incremental && (nMeasurements = _nMeasurements) > 1) {
		average = true;
		s << ";  measured "; s << nMeasurements; s << " times, average";
		goto g1;
	}

	Print(s.str());
}

#pragma comment(linker, "/SECTION:.shared,RWS")
#pragma data_seg (".shared")
Perf_Inst Perf = {};
#pragma data_seg()

#endif

bool IsOS64Bit()
{
#ifdef _WIN64
	return true;
#else
	static int r;
	if(!r) {
		if(IsWow64Process(GetCurrentProcess(), &r)) r = r ? 1 : -1;
	}
	return r > 0;
#endif
}

bool IsProcess64Bit(DWORD pid, OUT bool& is)
{
	is = false;
	if(!IsOS64Bit()) return true;
	HANDLE hp = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, false, pid);
	if(!hp) return false;
	BOOL is32, ok = IsWow64Process(hp, &is32);
	CloseHandle(hp);
	if(!ok) return false;
	is = !is32;
	return true;
}
