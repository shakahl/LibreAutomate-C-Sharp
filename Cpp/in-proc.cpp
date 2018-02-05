//This file contains DllMain and code that injects this dll into other processes and calls functions there.

//Dll injection is used for finding accessible objects (AO). Much faster when runs in the target thread.
//For example, can find an AO in Chrome web page about 60 times faster.
//note: with Firefox by default slow anyway, only maybe 30% faster than out-process.
//	To make fast, need to disable the Firefox multiprocess feature:
//	In about:config set this to false: browser.tabs.remote.force-enable
//	Firefox sets this to true when upgrading, eg from version 57 to 58. Even if both installed, if you run 57 and then 58.
//It seems UI Automation also searches inproc, but somehow it manages to find the same object only 2-3 times faster (instead of 60) than outproc MSAA, and in some cases slower. Also its loading is very slow.
//	Tested: UI Automation is not much faster when we call it inproc. Depends on window. Can be eg 2 times faster, or same speed.

//The main entry functions to find/get AO are Cpp_AccFind and Cpp_AccFromWindow (in "acc bridge.cpp").
//They call InjectDllAndGetAgent, which injects this dll into the target process (if not already done). Then they call the real 'find AO' etc function in the target thread.

//To inject dll, temporarily sets a WH_CALLWNDPROC hook and sends a message to the target window.
//When injected, creates a message-only window ("agent"), in each target thread. It is used for several purposes:
//	To detect whether the dll is injected into the target process and is ready to call functions in the target thread.
//	To get a proxy COM object that is used to call functions in that process/thread.
//	To unload this dll from processes when need (dll development, reinstallation).

//To call functions in the target process/thread, we hook and call IAccessible::get_accHelpTopic.
//It is probably the easiest way.
//	COM automatically marshals parameters and the returned data (as BSTR).
//	It does not marshal IAccessible objects. For it we use CoMarshalInterface/CoUnmarshalInterface.
//		We don't use LresultFromObject/ObjectFromLresult because it is slower and unreliable (fails when returning multiple objects).
//Another possible way - send message. I tested and rejected it. Slightly faster, but has 2 problems:
//	1. Quite big code, need PostMessage (not SendMessage), shared memory, event, 2-3 mutexes, etc, and therefore can be less reliable. Better let COM do all it.
//	2. It can be used to find AO in window. But to find AO in AO would need the hook anyway (I could not find another way, or it would be too complicated).
//	Also, I expected to make 'find all' much faster, because then can search and send/unmarshal results at the same time on different CPU cores. But it made faster only by 15%. Also, calling the final callback function before finishing searching is not a good idea, because then the finder must DoEvents because the callback would probably call object's methods.

//Cannot inject dll into some processes, including:
//	Windows Store apps (including the Edge web browser);
//	Console (not useful anyway);
//	Protected processes (eg antivirus);
//	Processes of higher UAC integrity level (unless this process is uiAccess);
//Tested: works with processes running as another user, if UAC allows it.

//When 'find all', the slowest part is releasing all found AO (if many), because calls the server process for each AO.
//	Partial solution: when used in C#, let GC release later in other thread.
//	However when using 'also' callback, and it says 'stop', remaining objects are released synchronously, which makes much slower.

//For more details, read the code below and code in "acc bridge.cpp".

#include "stdafx.h"
#include "cpp.h"

//#ifdef _DEBUG
//void InProcAccTest(IAccessible* a);
//#endif

HMODULE s_moduleHandle; //module handle of this dll

namespace
{
const STR c_agentWindowClassName = L"AuCpp_IPA_1"; //in-proc agent window class name
const int c_agentWndExtra = 200; //size of agent window's extra memory, which contains its AO marshal data
}

//Namespace inproc contains code used only in the server process.
//Namespace outproc contains code used only in the client process.
namespace inproc
{
long s_nAgentThreads; //how many threads in this process have our agent window
ATOM s_agentWindowClassAtom;
thread_local HWND t_agentWnd;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch(ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
		//Printf(L"P+  %i %s    tid=%i", GetCurrentProcessId(), GetCommandLineW(), GetCurrentThreadId());
		s_moduleHandle = hModule;
		break;
	case DLL_PROCESS_DETACH:
		//Printf(L"P-  %i %s    tid=%i", GetCurrentProcessId(), GetCommandLineW(), GetCurrentThreadId());
		break;
		//case DLL_THREAD_ATTACH:
		//	Printf(L"T+  %i %i", GetCurrentProcessId(), GetCurrentThreadId());
		//	break;
	case DLL_THREAD_DETACH:
		//Printf(L"T-  %i %i", GetCurrentProcessId(), GetCurrentThreadId());
		HWND wAgent = inproc::t_agentWnd;
		if(wAgent) DestroyWindow(wAgent);
		break;
	}
	return TRUE;
}

HRESULT AccGetProps(Cpp_Acc a, STR props, out BSTR& sResult);
HRESULT AccGetProp(Cpp_Acc a, WCHAR prop, out BSTR& sResult);
HRESULT AccWeb(IAccessible* iacc, STR what, out BSTR& sResult);

namespace inproc
{
HRESULT AccFindOrGet(MarshalParams_Header* h, IAccessible* iacc, out BSTR& sResult);
HRESULT AccEnableChrome2(MarshalParams_AccElem* p);

//Our hook of get_accHelpTopic.
HRESULT STDMETHODCALLTYPE Hook_get_accHelpTopic(IAccessible* iacc, out BSTR& sResult, VARIANT vParams, long* pMagic)
{
	if(vParams.vt == VT_BSTR) {
		//try {
		auto size = SysStringByteLen(vParams.bstrVal);
		if(size >= sizeof(MarshalParams_Header)) {
			*pMagic = c_magic;
			auto h = (MarshalParams_Header*)vParams.bstrVal;
			auto p = (MarshalParams_AccElem*)h;
			if(h->magic == c_magic) {
				sResult = null;
				HRESULT hr = 0;
				switch(h->action) {
//#ifdef _DEBUG
//				case InProcAction::IPA_AccTest:
//					InProcAccTest(iacc);
//					break;
//#endif
				case InProcAction::IPA_AccFind:
				case InProcAction::IPA_AccFromWindow:
				case InProcAction::IPA_AccFromPoint:
				case InProcAction::IPA_AccNavigate:
					hr = AccFindOrGet(h, iacc, sResult);
					break;
				case InProcAction::IPA_AccGetProps:
					hr = AccGetProps(Cpp_Acc(iacc, p->elem, h->miscFlags), (STR)(p + 1), out sResult);
					break;
				case InProcAction::IPA_AccGetWindow:
					hr = AccGetProp(Cpp_Acc(iacc, 0, h->miscFlags), 'w', out sResult);
					break;
				case InProcAction::IPA_AccGetHtml:
					hr = AccWeb(iacc, (STR)(h + 1), sResult);
					break;
				case InProcAction::IPA_AccEnableChrome:
					hr = AccEnableChrome2(p);
					break;
				}
				return hr;
			}
		}
		//} catch(...) { PRINTS(L"exception"); }
		//don't need try/catch. COM catches SEH and C++ exceptions and returns hresult "The server threw an exception".
		//	Also don't need /EHa. We don't throw and don't expect any exceptions.
	}
	return E_NOTIMPL;
	//never mind: we don't call the old method. Nobody implement or use it. MSDN: "is deprecated and should not be used".
}
} //namespace inproc

#pragma region marshal unmarshal agent IAccessible

namespace inproc
{
HookIAccessible s_hookIAcc;

//Gets wAgent AO, hooks its IAccessible interface, calls CoMarshalInterface, writes the data to the window extra memory (SetWindowLong).
IStream* MarshalAgentIAccessible(HWND wAgent) {
	//Perf.First();
	Smart<IAccessible> iacc;
	if(AccessibleObjectFromWindow(wAgent, OBJID_WINDOW, IID_IAccessible, (void**)&iacc)) return null;
	if(!s_hookIAcc.Hook(iacc)) return null;

	Smart<IStream> stream;
	CreateStreamOnHGlobal(0, true, &stream);
	if(CoMarshalInterface(stream, IID_IAccessible, iacc, MSHCTX_LOCAL, null, MSHLFLAGS_TABLESTRONG)) return null;

	DWORD streamSize, readSize;
	if(!istream::GetSize(stream, out streamSize)) return null;
	//Print(streamSize); //68
	assert(streamSize <= c_agentWndExtra / 2); //c_agentWndExtra=200
	if(streamSize > c_agentWndExtra - 4) return null;

	long b[c_agentWndExtra / 4];
	if(!istream::ResetPos(stream) || stream->Read(b, streamSize, &readSize) || readSize != streamSize || !istream::ResetPos(stream)) return null;

	SetWindowLongW(wAgent, 0, streamSize);
	for(DWORD i = 0; i < streamSize; i += 4) SetWindowLongW(wAgent, i + 4, b[i / 4]);

	return stream.Detach();
	//Perf.NW(); //190
}
}

namespace outproc
{
//Reads wAgent AO marshal data from the window extra memory (GetWindowLong), calls CoUnmarshalInterface.
bool UnmarshalAgentIAccessible(HWND wAgent, out IAccessible*& iacc) {
	iacc = null;

	DWORD streamSize = GetWindowLongW(wAgent, 0); if(streamSize == 0) return false;
	long b[c_agentWndExtra / 4];
	for(DWORD i = 0; i < streamSize; i += 4) b[i / 4] = GetWindowLongW(wAgent, i + 4);

	HGLOBAL hg = GlobalAlloc(GMEM_MOVEABLE, streamSize); if(hg == 0) return false;
	LPVOID mem = GlobalLock(hg); memcpy(mem, b, streamSize); GlobalUnlock(mem);

	Smart<IStream> stream;
	CreateStreamOnHGlobal(hg, true, &stream);
	if(CoUnmarshalInterface(stream, IID_IAccessible, (void**)&iacc)) return false;

	return true;
}
}
#pragma endregion

namespace inproc
{
//Thread proc that unloads this dll.
//Waits while all agent windows closed, unregisters agent window class, calls FreeLibraryAndExitThread.
DWORD WINAPI UnloadDllThreadProc(LPVOID lpParameter) {
	Sleep(15);
	//MSDN: "No window classes registered by a DLL are unregistered when the DLL is unloaded. A DLL must explicitly unregister its classes when it is unloaded."
	while(s_agentWindowClassAtom && !UnregisterClassW((STR)s_agentWindowClassAtom, s_moduleHandle) && GetLastError() != ERROR_CLASS_DOES_NOT_EXIST) {
		Printf(L"UnregisterClass failed. Error 0x%X", GetLastError());
		Sleep(50);
	}
	s_agentWindowClassAtom = 0;
	Sleep(100);
	FreeLibraryAndExitThread(s_moduleHandle, 0);
	return 0;
}

//Agent window procedure.
LRESULT WINAPI AgentWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	//agent window's AO marshal data, to pass to CoReleaseMarshalData on WM_NCDESTROY, because marshaled with MSHLFLAGS_TABLESTRONG
	static thread_local IStream* t_agentStream;

	switch(msg) {
	case WM_CREATE: {
		if(!(t_agentStream = MarshalAgentIAccessible(hwnd))) return -1; //-1 destroys the window
	} break;
	} //else Printx(msg);

	auto R = DefWindowProcW(hwnd, msg, wParam, lParam);

	if(msg == WM_NCDESTROY) {
		//this normally happens in two cases:
		//	1. This thread is ending. Then our DllMain calls DestroyWindow.
		//	2. Called Cpp_Unload. It closes all agent windows.

		if(t_agentStream) {
			//release marshal data, because marshaled with MSHLFLAGS_TABLESTRONG
			HRESULT hr = CoReleaseMarshalData(t_agentStream); if(hr) PRINTX(hr);
			t_agentStream->Release(); t_agentStream = null;
		}
		t_agentWnd = 0;

		if(0 == InterlockedDecrement(&s_nAgentThreads)) {
			//unload dll
			CloseHandle(CreateThread(null, 64 * 1024, UnloadDllThreadProc, null, 0, null));
		}
	}

	return R;
}

//Creates agent window.
//First time in process registers window class and increments dll refcount.
HWND CreateAgentWindow()
{
	if(!t_agentWnd) {
		if(1 == InterlockedIncrement(&s_nAgentThreads)) {
			//increment dll refcount to prevent unloading this dll when the caller process unhooks this hook
			HMODULE hm = 0;
			GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, (STR)s_moduleHandle, &hm);

			WNDCLASS c = { };
			c.hInstance = s_moduleHandle;
			c.lpfnWndProc = AgentWndProc;
			c.lpszClassName = c_agentWindowClassName;
			c.cbWndExtra = c_agentWndExtra; //for IAccessible marshaling data
			s_agentWindowClassAtom = RegisterClassW(&c);
			if(!s_agentWindowClassAtom) PRINTF(L"failed to register class. Error 0x%X", GetLastError());
		}

		wchar_t name[12]; _itow(GetCurrentThreadId(), name, 10);
		t_agentWnd = CreateWindowExW(WS_EX_NOACTIVATE, (STR)s_agentWindowClassAtom, name, WS_POPUP, 0, 0, 0, 0, HWND_MESSAGE, 0, 0, 0);
	}
	//Printf(L"s_nAgentThreads=%i t_agentWnd=%i s_agentWindowClassAtom=%i", s_nAgentThreads, (int)t_agentWnd, s_agentWindowClassAtom);
	return t_agentWnd;
}

//SetWindowsHookEx(WH_CALLWNDPROC) hook procedure.
//If message==WM_NULL and wParam==c_magic, creates agent window and returns its handle.
LRESULT WINAPI HookCallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	CWPSTRUCT& m = *(CWPSTRUCT*)lParam;

	if(m.message == WM_NULL && m.wParam == c_magic) {
		HWND wAgent = CreateAgentWindow();
		ReplyMessage((LRESULT)wAgent);
		return 1;
		//tested: not faster if we unhook before creating window
	} //else Printx(m.message);

	return CallNextHookEx(0, nCode, wParam, lParam);
}
} //namespace inproc

namespace outproc
{
//Injects this dll into the target process/thread.
//Uses the SetWindowsHookEx/SendMessage method.
//w - a window in the target thread.
//wAgent - receives agent window handle.
//tid, pid - target thread/process id. This func could call GetWindowThreadProcessId, but the caller already did it.
bool InjectDll(HWND w, out HWND& wAgent, DWORD tid, DWORD pid)
{
	auto hh = SetWindowsHookExW(WH_CALLWNDPROC, inproc::HookCallWndProc, s_moduleHandle, tid);
	if(hh == 0) PRINTX(GetLastError());
	if(hh == 0) return false;
	wAgent = (HWND)SendMessageW(w, 0, c_magic, 0);
	UnhookWindowsHookEx(hh);
	if(wAgent) return true;

	//problem: does not work with windows of class "Windows.UI.Core.CoreWindow", ie Windows Store app processes.
	//	SetWindowsHookEx succeeds, but the hook proc is never called, and the dll is not injected.
	//	But SendMessage works, eg WM_CLOSE closes the app.
	//	The CreateRemoteThread(LoadLibrary) method fails too. LoadLibrary returns 0.
	//	Found some info on the internet. The dll must be signed etc. Or it can be in system directory, and with LoadLibrary use just file name.
	//	Also there are other protected processes, for example some system processes, antivirus, protected processes of web browsers.

	//Also does not work with console windows. Not a problem, because there are no useful AO.
	//	SetWindowsHookEx sets error 0x57, "The parameter is incorrect".
	//	The console window actually belongs to the conhost process, and GetWindowThreadProcessId lies.

	return false;
}

#pragma region 64/32 bit transition

//Rundll32.exe calls this.
//Calls InjectDll.
EXPORT void WINAPI Cpp_RunDllW(HWND hwnd, HINSTANCE hinst, STR lpszCmdLine, int nCmdShow) //note: with W
{
	//Printf(L"Cpp_RunDllW, %i, %s", IsThisProcess64Bit(), lpszCmdLine);
	LPWSTR s2;
	HWND w = (HWND)(LPARAM)wcstol(lpszCmdLine, &s2, 10), wAgent;
	DWORD pid, tid = GetWindowThreadProcessId(w, &pid); if(tid == 0) return;
	if(!InjectDll(w, out wAgent, tid, pid)) return;
	HANDLE ev = (HANDLE)(LPARAM)wcstol(s2, null, 10);
	SetEvent(ev);
}

//Executes rundll32.exe of different bitness.
//It calls Cpp_RunDllW which calls InjectDll.
bool RunDll(HWND w)
{
#ifdef _WIN64
	//tested on Win10 and 7: rundll32 supports dll path enclosed in ".
	static const STR rundll = L"\\SysWOW64\\rundll32.exe \"";
	static const STR bits = L"32";
#else
	static const STR rundll = L"\\SysNative\\rundll32.exe \"";
	static const STR bits = L"64";
#endif

	SECURITY_ATTRIBUTES sa = {}; sa.bInheritHandle = true;
	CHandle ev(CreateEventW(&sa, false, false, null)); //not necessary. Makes faster by 5 ms.

	str::StringBuilder b; LPWSTR t; int n;
	t = b.GetBufferToAppend(out n); b.FixBuffer(GetWindowsDirectoryW(t, n));
	b << rundll;
	t = b.GetBufferToAppend(out n); b.FixBuffer(GetModuleFileNameW(s_moduleHandle, t, n));
	auto u = wcsrchr(t, '\\') - 5; u[0] = bits[0]; u[1] = bits[1]; //"32" to "64" or vice versa
	if(GetFileAttributes(t) == INVALID_FILE_ATTRIBUTES) return false; //avoid messagebox when our antimatter dll does not exist
	b << L"\",Cpp_RunDll "; //note: without W
	b << (__int64)w << ' ' << (__int64)ev.m_h;

	STARTUPINFOW si = { }; si.cb = sizeof(si); si.dwFlags = STARTF_FORCEOFFFEEDBACK;
	PROCESS_INFORMATION pi;
	bool ok = CreateProcessW(null, b, null, null, true, 0, null, null, &si, &pi);
	if(!ok) return false;
	CloseHandle(pi.hThread);
	HANDLE ha[] = { ev, pi.hProcess };
	ok = WAIT_OBJECT_0 == WaitForMultipleObjects(2, ha, false, INFINITE);
	CloseHandle(pi.hProcess);
	return ok;
}

#pragma endregion

//Simple cache of a window, its thread id and optionally AO.
//Can make faster, for example when waiting.
//Use thread_local.
class HwndTidCache {
	DWORD _tid, _hwnd;
	IAccessible* _iaccAgent;
	ULONGLONG _time;
public:
	HwndTidCache() noexcept {
		ZEROTHIS;
	}

	~HwndTidCache() {
		if(_iaccAgent) _iaccAgent->Release();
	}

	bool Get(DWORD tid, out HWND& w, out IAccessible** iaccAgent = null)
	{
		if(tid != _tid) return false;
		ULONGLONG time = GetTickCount64(); if(time - _time > 10000) return false;
		HWND wCached = (HWND)(LPARAM)_hwnd;
		if(tid != GetWindowThreadProcessId(wCached, null)) return false;
		_time = time;
		w = wCached;
		if(iaccAgent) *iaccAgent = _iaccAgent;
		return true;
	}

	void Set(DWORD tid, HWND w, IAccessible* iaccAgent = null)
	{
		_tid = tid;
		_hwnd = (int)(LPARAM)w;
		if(_iaccAgent) _iaccAgent->Release();
		_iaccAgent = iaccAgent;
		_time = GetTickCount64();
	}
};

//Finds agent window and gets its AO.
//If dll still not injected, injects and creates agent window.
//w - a window in the target process/thread.
//iaccAgent - receives agent's AO. Don't Release, it's cached.
//wAgent - receives agent window. Optional.
//Returns: 0, eError::WindowClosed, eError::WindowOfThisThread, eError::UseNotInProc, eError::Inject.
HRESULT InjectDllAndGetAgent(HWND w, out IAccessible*& iaccAgent, out HWND* wAgent /*= null*/)
{
	HWND wa = 0;
	DWORD pid, tid = GetWindowThreadProcessId(w, &pid); if(tid == 0) return (HRESULT)eError::WindowClosed;

	//use a simple cache to make faster, for example when waiting for AO in w
	static thread_local HwndTidCache t_agentCache, t_failedCache;
	if(t_agentCache.Get(tid, out wa, out &iaccAgent)) {
		if(wAgent) *wAgent = wa;
		return 0;
	}
	HWND wFailed; if(t_failedCache.Get(tid, out wFailed)) return (HRESULT)eError::Inject;

	if(tid == GetCurrentThreadId()) return (HRESULT)eError::WindowOfThisThread;
	if(wnd::ClassNameIs(w, { L"ApplicationFrameWindow", L"Windows.UI.Core.CoreWindow", L"ConsoleWindowClass", L"SunAwt*" }))
		return (HRESULT)eError::UseNotInProc;

	static CHandle s_mutex(CreateMutexW(SecurityAttributes::Common(), false, L"AuCpp_MutexGAW"));
	DWORD wfso = WaitForSingleObject(s_mutex, INFINITE);
	assert(wfso == 0 || wfso == WAIT_ABANDONED);
	AutoReleaseMutex arm(s_mutex);

	//Perf.First();
	wchar_t name[12]; _itow(tid, name, 10);
	wa = FindWindowEx(HWND_MESSAGE, 0, c_agentWindowClassName, name);
	//Perf.Next();
	if(!wa) {
		bool ok = false, is64bit, differentBits = IsProcess64Bit(pid, out is64bit) && is64bit != IsThisProcess64Bit();
		//Perf.Next();
		if(differentBits) {
			Print(L"note: process of different bitness. Make sure 64 and 32 bit dll versions are built.");

			if(RunDll(w)) { //speed: 40-60 ms
				wa = FindWindowEx(HWND_MESSAGE, 0, c_agentWindowClassName, name);
				ok = !!wa;
			}
		} else {
			ok = InjectDll(w, out wa, tid, pid);
		}
		//Perf.Next();

		if(!ok) {
			if(!IsWindow(w)) return (HRESULT)eError::WindowClosed;
			t_failedCache.Set(tid, w);
			return (HRESULT)eError::Inject;
		}
	}
	//Perf.Write();

	if(!UnmarshalAgentIAccessible(wa, iaccAgent)) {
		t_failedCache.Set(tid, w);
		return (HRESULT)eError::Inject;
	}

	t_agentCache.Set(tid, wa, iaccAgent);

	if(wAgent) *wAgent = wa;
	return 0;
}

//Unloads this dll from all processes except this.
//Can be used when developing this dll and when installing new version.
//The installer then should wait min 200 ms, call FreeLibrary, and retry/wait if the dll is still locked.
EXPORT void Cpp_Unload()
{
	for(HWND wPrev = 0; ; ) {
		HWND w = FindWindowEx(HWND_MESSAGE, 0, c_agentWindowClassName, null);
		if(w == 0 || w == wPrev) break;
		wPrev = w;
		DWORD_PTR res;
		SendMessageTimeout(w, WM_CLOSE, 0, 0, SMTO_ABORTIFHUNG, 5000, &res);
	}
}

#ifdef _DEBUG
EXPORT void Cpp_InProcTest(IAccessible* a)
{
	InProcCall c;
	c.AllocParams(a, InProcAction::IPA_AccTest, sizeof(MarshalParams_Header));
}
#endif

} //namespace outproc
