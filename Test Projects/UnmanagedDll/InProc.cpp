#include "stdafx.h"
#include "util.h"

LRESULT TestUia(HWND w);
LRESULT TestAcc(HWND w, LPCWSTR name);

HMODULE s_moduleHandle;
const LPCWSTR c_injectName = L"AuCppInProc"; //agent window class name. Also can be used for eg registered message, window property, mutex.
const int c_injectWparam = -1'572'289'143;
static ATOM s_atomAgent;
thread_local HWND t_hwndAgent;
static long s_nAgents;

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch(ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
		//Printf(L"P+  %i %i", GetCurrentProcessId(), GetCurrentThreadId());
		s_moduleHandle = hModule;
		break;
	case DLL_PROCESS_DETACH:
		//Printf(L"P-  %i %i", GetCurrentProcessId(), GetCurrentThreadId());
		break;
	//case DLL_THREAD_ATTACH:
	//	Printf(L"T+  %i %i", GetCurrentProcessId(), GetCurrentThreadId());
	//	break;
	case DLL_THREAD_DETACH:
		//Printf(L"T-  %i %i", GetCurrentProcessId(), GetCurrentThreadId());
		if(t_hwndAgent) DestroyWindow(t_hwndAgent);
		break;
	}
	return TRUE;
}

struct _AccFindParams
{
	HWND w;
	wchar_t name[1];
};

void TestClrHost(OUT int& r);
void UnloadClr();

LRESULT _AccFind1(_AccFindParams* p, DWORD sizeOf)
{
	//int r = 0;
	//TestClrHost(OUT r);
	////Print(r);
	//return r;

	//Print(p->name);
	////return TestAcc(p->w, p->name);
	return TestAcc(p->w, L"Untitled");
	//return -1;
	//Perf.First();
	IAccessiblePtr a;
	auto hr = AccessibleObjectFromWindow(p->w, OBJID_WINDOW, IID_IAccessible, (void**)&a);
	//Perf.Next();
	if(hr != 0) return hr < 0 ? hr : E_FAIL;
	//return LresultFromObject(IID_IAccessible, 0, a);
	auto R = LresultFromObject(IID_IAccessible, 0, a);
	//a.Release();
	//Perf.NW();
	return R;
}

DWORD WINAPI UnloadThreadProc(LPVOID lpParameter) {
	//MSDN: "No window classes registered by a DLL are unregistered when the DLL is unloaded. A DLL must explicitly unregister its classes when it is unloaded."
	while(s_atomAgent && !UnregisterClassW((LPCWSTR)s_atomAgent, s_moduleHandle) && GetLastError() != ERROR_CLASS_DOES_NOT_EXIST) {
		Printf(L"UnregisterClass failed. Error 0x%X", GetLastError());
		Sleep(100);
	}
	s_atomAgent = 0;
	//UnloadClr();
	Sleep(100);
	FreeLibraryAndExitThread(s_moduleHandle, 0);
}

LRESULT WINAPI AgentWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	//Printx(msg);
	if(msg == WM_COPYDATA && wParam == c_injectWparam) {
		try {
			COPYDATASTRUCT* x = (COPYDATASTRUCT*)lParam;
			switch(x->dwData) {
			case 1: return _AccFind1((_AccFindParams*)x->lpData, x->cbData);
			}
		}
		catch(...) { Print(L"exception in " __FUNCTIONW__); }

		return 0;
	} //else Printx(msg);

	auto R = DefWindowProc(hwnd, msg, wParam, lParam);

	if(msg == WM_NCDESTROY) {
		//this normally happens in two cases:
		//	1. This thread is ending. Then our DllMain calls DestroyWindow.
		//	2. Called a_unload_from_all_processes. It finds and closes all agent windows.
		t_hwndAgent = 0;
		if(0 == InterlockedDecrement(&s_nAgents)) {
			//UnloadClr();

			//unload dll
			CloseHandle(CreateThread(nullptr, 64000, UnloadThreadProc, nullptr, 0, nullptr));
		}
	}

	return R;
}

HWND CreateAgentWindow()
{
	if(!t_hwndAgent) {
		if(1 == InterlockedIncrement(&s_nAgents)) {
			//increment dll refcount to prevent unloading this dll when the caller process unhooks this hook
			HMODULE hm = 0;
			GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, (LPCWSTR)s_moduleHandle, &hm);

			WNDCLASS c = {};
			c.hInstance = s_moduleHandle;
			c.lpfnWndProc = AgentWndProc;
			c.lpszClassName = c_injectName;
			s_atomAgent = RegisterClass(&c);
			if(!s_atomAgent) Printf(L"failed to register class. Error 0x%X", GetLastError());
		}

		wchar_t name[12]; _itow(GetCurrentThreadId(), name, 10);
		t_hwndAgent = CreateWindowExW(WS_EX_NOACTIVATE, (LPCWSTR)s_atomAgent, name, WS_POPUP, 0, 0, 0, 0, HWND_MESSAGE, 0, 0, 0);
	}
	//Printf(L"s_nAgents=%i t_hwndAgent=%i s_atomAgent=%i", s_nAgents, (int)t_hwndAgent, s_atomAgent);
	return t_hwndAgent;
}

LRESULT WINAPI HookCallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	CWPSTRUCT& m = *(CWPSTRUCT*)lParam;

	if(m.message == 0 && m.wParam == c_injectWparam) {
		ReplyMessage((LRESULT)CreateAgentWindow());
		return 1;
	} //else Printx(m.message);

	return CallNextHookEx(0, nCode, wParam, lParam);
}

extern "C" __declspec(dllexport)
void WINAPI AuCpp_RunDllW(HWND hwnd, HINSTANCE hinst, LPCWSTR lpszCmdLine, int nCmdShow) //note: with W
{
	//Printf(L"AuCpp_RunDllW, %i, %s", IsThisProcess64Bit(), lpszCmdLine);
	HWND w = (HWND)_wtoi(lpszCmdLine);
	DWORD tid = GetWindowThreadProcessId(w, nullptr); if(tid == 0) return;
	auto hh = SetWindowsHookEx(WH_CALLWNDPROC, HookCallWndProc, s_moduleHandle, tid); if(hh == 0) return;
	SendMessage(w, 0, c_injectWparam, 0);
	UnhookWindowsHookEx(hh);
}

bool _GAW_RunDll(HWND w)
{
#ifdef _WIN64
	//tested on Win10 and 7: rundll32 supports dll path enclosed in ".
	static const LPCWSTR rundll = L"\\SysWOW64\\rundll32.exe \"";
	static const LPCWSTR bits = L"32";
#else
	static const LPCWSTR rundll = L"\\SysNative\\rundll32.exe \"";
	static const LPCWSTR bits = L"64";
#endif

	WCHAR b[300]; GetWindowsDirectory(b, 300);
	std::wstringstream s;
	s << b << rundll;
	GetModuleFileName(s_moduleHandle, b, 300);
	auto t = wcsrchr(b, '\\') - 5; t[0] = bits[0]; t[1] = bits[1];
	if(GetFileAttributes(b) == INVALID_FILE_ATTRIBUTES) return false; //avoid messagebox when our antimatter dll does not exist
	s << b << "\",AuCpp_RunDll " << (int)w; //note: without W

	//Print(s.str()); return false;

	STARTUPINFOW si = { sizeof(si) }; si.dwFlags = STARTF_FORCEOFFFEEDBACK;
	PROCESS_INFORMATION pi;
	bool ok = CreateProcessW(0, (LPWSTR)s.str().c_str(), 0, 0, 0, 0, 0, 0, &si, &pi);
	if(!ok) return false;
	CloseHandle(pi.hThread);
	ok = WAIT_OBJECT_0 == WaitForSingleObject(pi.hProcess, INFINITE);
	CloseHandle(pi.hProcess);
	return ok;
}

bool GetAgentWindow(HWND w, OUT HWND& wa)
{
	wa = 0;
	DWORD pid, tid = GetWindowThreadProcessId(w, &pid); if(tid == 0) return false;
	if(tid == GetCurrentThreadId()) return false;
	wchar_t name[12]; _itow(tid, name, 10);
	wa = FindWindowEx(HWND_MESSAGE, 0, c_injectName, name);
	if(!wa) {
		auto hh = SetWindowsHookEx(WH_CALLWNDPROC, HookCallWndProc, s_moduleHandle, tid); if(hh == 0) return false;
		wa = (HWND)SendMessage(w, 0, c_injectWparam, 0);
		UnhookWindowsHookEx(hh);
		if(!wa) {
			bool is64bit;
			if(IsProcess64Bit(pid, OUT is64bit) && is64bit != IsThisProcess64Bit()) {
				if(!_GAW_RunDll(w)) return false; //speed: 60 ms
				wa = FindWindowEx(HWND_MESSAGE, 0, c_injectName, name);
			}
		}
	}
	return (bool)wa;
}

struct AuCpp_AccFindParams
{
	HWND w;

};

struct AuCpp_AccFindResults
{
	IAccessible* iacc;
	long elem;
};

extern "C" __declspec(dllexport)
BOOL AuCpp_AccFind(bool inject, HWND w, IAccessible*& a, long& elem)
{
	LPCWSTR name = L"Untitled";

	a = nullptr; elem = 0;
	HWND wAgent = 0;
	LRESULT R;
	if(inject && GetAgentWindow(w, OUT wAgent)) {
		auto memSize = sizeof(_AccFindParams) + wcslen(name) * 2;
		auto p = (_AccFindParams*)malloc(memSize);
		p->w = w;
		wcscpy(p->name, name);

		COPYDATASTRUCT x;
		x.dwData = 1;
		x.lpData = p;
		x.cbData = (DWORD)memSize;
		R = SendMessage(wAgent, WM_COPYDATA, c_injectWparam, (LPARAM)&x);

		free(p);
	} else {
		R = TestAcc(w, name);
	}
	if(R <= 0) return false;
	int hr = ObjectFromLresult(R, IID_IAccessible, 0, (void**)(IAccessible**)&a);
	if(hr != 0 || !a) return false;
	return true;
}

extern "C" __declspec(dllexport)
void AuCpp_Unload()
{
	for(HWND wPrev = 0; ; ) {
		HWND w = FindWindowEx(HWND_MESSAGE, 0, c_injectName, nullptr);
		if(w == 0 || w == wPrev) break;
		wPrev = w;
		DWORD_PTR res;
		SendMessageTimeout(w, WM_CLOSE, 0, 0, SMTO_ABORTIFHUNG, 5000, &res);
	}
}

extern "C" __declspec(dllexport)
void AuCpp_Test()
{
	//Print(IsOS64Bit());

	HWND w = FindWindow(L"Chrome_WidgetWin_1", nullptr);
	//HWND w = FindWindow(L"QM_Editor", nullptr);
	if(!w) {
		Print("window not found");
		return;
	}
	DWORD pid, tid = GetWindowThreadProcessId(w, &pid);
	bool is64bit, ok = IsProcess64Bit(pid, is64bit);
	Printf(L"ok=%i is64=%i", ok, is64bit);
}
