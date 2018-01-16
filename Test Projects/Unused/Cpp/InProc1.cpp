#include "stdafx.h"
#include "cpp.h"
#if IPC_VERSION==1

void TestAccFindInWnd(AccCallback* callback, bool findAll, HWND w, STR role, STR name);
//void TestAccFindInAcc(AccCallback* callback, bool findAll, IAccessible* iaccParent, STR role, STR name);


HMODULE s_moduleHandle;
const int c_injectWparam = -1'572'289'143;
const STR c_agentWindowClassName = L"AuIPC1";
//const int c_sharedMemorySize = 64 * 1024;
//const int c_sharedMemorySize = 2000; //TODO
const int c_sharedMemorySize = 1024 * 1024; //TODO

enum class eAgentAction
{
	AccFindInWnd = 1,
	//AccFromWnd,
	//AccFromPoint,
};

namespace inproc
{
	ATOM s_agentWindowClassAtom;
	thread_local HWND t_agentWnd;
	long s_nAgentThreads;
}

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
		HWND wa = inproc::t_agentWnd;
		if(wa) DestroyWindow(wa);
		break;
	}
	return TRUE;
}

static SharedMemory s_sm;
static KernelHandle s_searchMutexOfClient, s_searchMutexOfServer, s_resultMutex, s_resultEvent;

bool InitIPC()
{
	static int s_result;
	if(s_result == 0) {
		s_result = -1;

		s_sm.Create(L"AuInProcMemory", c_sharedMemorySize);
		//Printf(L"s_sm: %i %i", GetCurrentProcessId(), !s_sm.Is0());
		assert(!s_sm.Is0()); if(s_sm.Is0()) return false;

		s_searchMutexOfClient = CreateMutexW(SecurityAttributes::Common(), false, L"AuInProcSearchMutexC");
		//Printf(L"s_searchMutexOfClient: %i %i", GetCurrentProcessId(), !s_searchMutexOfClient.Is0());
		assert(!s_searchMutexOfClient.Is0()); if(s_searchMutexOfClient.Is0()) return false;

		s_searchMutexOfServer = CreateMutexW(SecurityAttributes::Common(), false, L"AuInProcSearchMutexS");
		//Printf(L"s_searchMutexOfServer: %i %i", GetCurrentProcessId(), !s_searchMutexOfServer.Is0());
		assert(!s_searchMutexOfServer.Is0()); if(s_searchMutexOfServer.Is0()) return false;

		//TODO: remove if all results will be at the end
		s_resultMutex = CreateMutexW(SecurityAttributes::Common(), false, L"AuInProcResultMutex");
		//Printf(L"s_resultMutex: %i %i", GetCurrentProcessId(), !s_resultMutex.Is0());
		assert(!s_resultMutex.Is0()); if(s_resultMutex.Is0()) return false;

		s_resultEvent = CreateEventW(SecurityAttributes::Common(), false, false, L"AuInProcResultEvent");
		//Printf(L"s_resultEvent: %i %i", GetCurrentProcessId(), !s_resultEvent.Is0());
		assert(!s_resultEvent.Is0()); if(s_resultEvent.Is0()) return false;

		s_result = 1;
	}
	return s_result > 0;
}

struct SM_HEADER
{
	int connectionId;
	int resultCount;
	int resultSize;
};

HRESULT _ObjectToSM(SM_HEADER* h, IAccessible* iacc, long elem)
{
	if(h->resultCount == 0) h->resultSize = sizeof(*h);
	int oldSize = h->resultSize, newSize = oldSize + 4, streamSize = 0;
	if(iacc) {
		//streamSize[4]+stream+elem[4]
		IStreamPtr x;
		if(CreateStreamOnHGlobal(0, true, &x)) return -1;
		if(CoMarshalInterface(x, IID_IAccessible, iacc, MSHCTX_LOCAL, null, MSHLFLAGS_NORMAL)) return -2;

		//__int64 size64; if(x->Seek(LI(0), STREAM_SEEK_END, (ULARGE_INTEGER*)&size64)) return -3; //get stream size
		//streamSize = (int)size64;

		STATSTG stat; if(x->Stat(&stat, STATFLAG_NONAME)) return -3; //get stream size
		streamSize = stat.cbSize.LowPart;

		//Print(streamSize); //68

		newSize += streamSize + 4;
		if(newSize > c_sharedMemorySize) return 1;
		if(x->Seek(LI(0), STREAM_SEEK_SET, null)) return -5;
		LPBYTE b = (LPBYTE)h; ULONG nread;
		if(x->Read(b + oldSize + 4, streamSize, &nread) || nread != streamSize) return -6;
		*(int*)(b + newSize - 4) = elem;
	} else {
		//0[4]
		if(newSize > c_sharedMemorySize) return 1;
	}
	*(int*)((LPBYTE)h + oldSize) = streamSize;
	h->resultSize = newSize;
	h->resultCount++;
	return 0;
}

HRESULT ObjectToSM(SM_HEADER* h, IAccessible* iacc, long elem)
{
	//Perf.First();
	HRESULT hr = _ObjectToSM(h, iacc, elem);
	//Perf.NW();
	if(hr) {
		Printf(L"ObjectToSM: %i", hr);
	}
	return hr;
}

HRESULT ObjectFromSM(SM_HEADER* h, ref int& start, out IAccessible*& iacc, out long& elem)
{
	iacc = null; elem = 0;

	assert(start < h->resultSize); if(start >= h->resultSize) return 1;

	int streamSize = *(int*)((LPBYTE)h + start);
	start += 4;
	if(streamSize == 0) return 0;

	HGLOBAL g = GlobalAlloc(GMEM_MOVEABLE, streamSize); if(g == 0) return -40;
	LPVOID p = GlobalLock(g); if(p == null) { GlobalFree(g); return -41; }
	memcpy(p, (LPBYTE)h + start, streamSize);
	GlobalUnlock(p);
	start += streamSize;

	IStreamPtr x;
	if(CreateStreamOnHGlobal(g, true, &x)) { GlobalFree(g); return -1; }
	if(CoUnmarshalInterface(x, IID_IAccessible, (void**)&iacc)) return -2; //never mind: if fails, should call CoReleaseMarshalData?

	elem = *(long*)((LPBYTE)h + start);
	start += 4;

	//Printf(L"from SM: %p", iacc);
	return 0;
}

namespace inproc
{
	HRESULT STDMETHODCALLTYPE Hook_get_accHelpTopic(IAccessible* iacc, out BSTR* pszHelpFile_ret, VARIANT varChild_params, long* pidTopic_unused)
	{
		//Print("hook");
		//Printf(L"vt=%i, pszHelpFile_ret=%p", varChild_params.vt, pszHelpFile_ret);
		if(varChild_params.vt == VT_BSTR && varChild_params.bstrVal != null) {
			BSTR b = varChild_params.bstrVal;
			//Print(b);
			//*pszHelpFile_ret = SysAllocString(L"ret");
			//*pszHelpFile_ret = SysAllocStringLen(L"ret\0" L"more", 8);
			HRESULT hr = iacc->get_accName(_variant_t(0L), pszHelpFile_ret);
			Print(hr);
			return 0;
		}
		return E_NOTIMPL;
		//never mind: we don't call the old method (_RESTORE::p). Nobody implement or use it.
	}

	class AccHooks
	{
		struct _RESTORE {
			LPVOID* place; //address of the function pointer in the interface table
			LPVOID oldFunc;
		};

		CSimpleArray<_RESTORE> _a; //usually there are 1 or 2 interface tables in a process, but eg Firefox with multiple tabs can have ~10

		//place - address of the function pointer in the interface table.
		//func - the hook function or the old function (to unhook).
		bool _ReplaceFunctionInTable(LPVOID* place, LPVOID func) {
			DWORD oldProt = 0;
			if(!VirtualProtect(place, sizeof(LPVOID), PAGE_EXECUTE_READWRITE, &oldProt)) return false;
			*place = func;
			if(oldProt != PAGE_EXECUTE_READWRITE) VirtualProtect(place, sizeof(LPVOID), oldProt, &oldProt);
			return true;
		}

	public:
		bool Hook(IAccessible* iacc) {
			LPVOID* place = *(LPVOID**)iacc + 16; //&get_accHelpTopic
			LPVOID func = *place; //get_accHelpTopic
			if(func != Hook_get_accHelpTopic) {
				//Printf(L"%p", func);
				static CComAutoCriticalSection _cs;
				CComCritSecLock<CComAutoCriticalSection> lock(_cs);
				for(int i = 0, n = _a.GetSize(); i < n; i++) if(_a[i].place == place) return true; //some unknown hook may be installed after ours
				if(!_ReplaceFunctionInTable(place, Hook_get_accHelpTopic)) return false;
				_RESTORE r = { place, func };
				_a.Add(r);
			}
			return true;
		}

		~AccHooks() {
			for(int i = _a.GetSize() - 1; i >= 0; i--) {
				_RESTORE r = _a[i];
				bool restored = _ReplaceFunctionInTable(r.place, r.oldFunc);
				//Printf(L"restored=%i, func=%p", restored, r.oldFunc);
			}
		}
	};

	AccHooks _accHooks;

	class AccCallbackInProc : public AccCallback
	{
		SM_HEADER* _sm;
		int _connectionId;
		bool _findAll;
		DWORD _time;
	public:
		AccCallbackInProc(SM_HEADER* sm, bool findAll) : _sm(sm), _connectionId(sm->connectionId), _findAll(findAll) {
			_time = GetTickCount();
		}

		//fullCheck - do a full check, which is slower. If false, does not detect when the client thread terminated.
		bool IsClientStillWaiting(bool fullCheck) {
			if(_connectionId != _sm->connectionId) return false; //client abandoned the search. Possibly terminated. Possibly another thread entered s_searchMutexOfClient and waiting for s_searchMutexOfServer.

			switch(WaitForSingleObject(s_searchMutexOfClient, 0)) {
			case WAIT_TIMEOUT: return true;
			case 0: case WAIT_ABANDONED: ReleaseMutex(s_searchMutexOfClient);
			}
			return false;
		}

		int _WriteMultiResult(IAccessible* iacc, long elem) {
			HRESULT hr = 0;
			for(;;) {
				if(0 != WaitForSingleObject(s_resultMutex, INFINITE)) { //TODO: timeout
					assert(!"failed WaitForSingleObject(s_resultMutex)");
					hr = -30; break;
				}
				AutoReleaseMutex arResultMutex(s_resultMutex);
				hr = ObjectToSM(_sm, iacc, elem);
				arResultMutex.ReleaseNow();

				if(hr != 1) break; //hr 1 - full shared memory
				if(!IsClientStillWaiting(true)) break;
				SetEvent(s_resultEvent);
				Sleep(15);

				//never mind: if elem, should write object once. Difficult to implement.
				//	Now works correctly (ref counting etc, even same proxy pointer), but slower.
			}

			if(hr) {
				_sm->resultCount = 1;
				_sm->resultSize = sizeof(SM_HEADER) + 4;
				*(int*)(_sm + 1) = 0;
				return 0;
			}
			return _sm->resultCount;
		}

		void WriteResult(IAccessible* iacc, long elem) {

			_accHooks.Hook(iacc);

			if(!_findAll) {
				ObjectToSM(_sm, iacc, elem);
			} else {
				int n = _WriteMultiResult(iacc, elem);

				//don't notify client too frequently. It makes slower if single CPU and client's thread priority is higher.
				if(iacc != null && n != 0) {
					//return;
					DWORD time = GetTickCount(), td = time - _time;
					if(td == 0 && n < 100) return;
					_time = time;
				}
			}
			//Perf.First();
			SetEvent(s_resultEvent);
			//Perf.NW();
		}

		void Finished() {
			if(_findAll) _WriteMultiResult(null, 0);
			SetEvent(s_resultEvent);
		}
	};

	void _AccFind1(eAgentAction action, int connectionId)
	{
		int wr = WaitForSingleObject(s_searchMutexOfServer, 4000);
		assert(wr == 0 || wr == WAIT_ABANDONED);
		AutoReleaseMutex arMutex(s_searchMutexOfServer);

		//SetThreadAffinityMask(GetCurrentThread(), 1); //for testing single CPU

		auto h = (SM_HEADER*)s_sm.Mem();

		if(h->connectionId != connectionId) return;

		auto pSM = (AccFindParams*)(h + 1);
		int size = pSM->size;
		CHeapPtr<AccFindParams> p; p.AllocateBytes(size); memcpy(p, pSM, size);

		//IAccessiblePtr acc;
		//if(action == 2) {
		//}

		HWND w = (HWND)(LPARAM)p->hwnd;

		ResetEvent(s_resultEvent);
		h->resultCount = 0;

		AccCallbackInProc callback(h, p->findAll);

		switch(action) {
		case eAgentAction::AccFindInWnd:
			TestAccFindInWnd(&callback, p->findAll, w, p->Role(), p->Name());
			break;
		//case 2:
		//	TestAccFindInAcc(&callback, p->findAll, acc, p->Name());
		//	break;
		}
		//Sleep(500);
	}

	DWORD WINAPI UnloadDllThreadProc(LPVOID lpParameter) {
		Sleep(15);
		//MSDN: "No window classes registered by a DLL are unregistered when the DLL is unloaded. A DLL must explicitly unregister its classes when it is unloaded."
		while(s_agentWindowClassAtom && !UnregisterClassW((STR)s_agentWindowClassAtom, s_moduleHandle) && GetLastError() != ERROR_CLASS_DOES_NOT_EXIST) {
			Printf(L"UnregisterClass failed. Error 0x%X", GetLastError());
			Sleep(50);
		}
		s_agentWindowClassAtom = 0;
		//UnloadClr();
		Sleep(100);
		FreeLibraryAndExitThread(s_moduleHandle, 0);
		return 0;
	}

	void _AgentAction(UINT msg, WPARAM wParam, LPARAM lParam)
	{
		switch(msg) {
		case WM_USER: {
			if(HIWORD(wParam) != c_accVersion) break;
			eAgentAction action = (eAgentAction)LOWORD(wParam);
			switch(action) {
			case eAgentAction::AccFindInWnd:
				_AccFind1(action, (int)lParam);
				break;
			}
		}break;
		}
	}

	LRESULT WINAPI AgentWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
	{
		//Printf(L"%i 0x%X", (int)hwnd, msg);
		//Printx(msg);
		if(msg >= WM_USER && msg < WM_USER * 2) {
			try { _AgentAction(msg, wParam, lParam); }
			catch(...) { Print(L"exception in " __FUNCTIONW__); }
			return 0;
		}

		//switch(msg) {
		//case WM_CREATE: {
		//} break;
		//} //else Printx(msg);

		auto R = DefWindowProcW(hwnd, msg, wParam, lParam);

		if(msg == WM_NCDESTROY) {
			//this normally happens in two cases:
			//	1. This thread is ending. Then our DllMain calls DestroyWindow.
			//	2. Called Cpp_Unload. It closes all agent windows.

			t_agentWnd = 0;

			if(0 == InterlockedDecrement(&s_nAgentThreads)) {
				//unload dll
				CloseHandle(CreateThread(null, 64 * 1024, UnloadDllThreadProc, null, 0, null));
			}
		}

		return R;
	}

	HWND CreateAgentWindow()
	{
		if(!t_agentWnd) {
			if(!InitIPC()) return 0;

			if(1 == InterlockedIncrement(&s_nAgentThreads)) {
				//increment dll refcount to prevent unloading this dll when the caller process unhooks this hook
				HMODULE hm = 0;
				GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, (STR)s_moduleHandle, &hm);

				WNDCLASS c = {};
				c.hInstance = s_moduleHandle;
				c.lpfnWndProc = AgentWndProc;
				c.lpszClassName = c_agentWindowClassName;
				//c.cbWndExtra = 4;
				s_agentWindowClassAtom = RegisterClass(&c);
				if(!s_agentWindowClassAtom) Printf(L"failed to register class. Error 0x%X", GetLastError());
			}

			wchar_t name[12]; _itow(GetCurrentThreadId(), name, 10);
			t_agentWnd = CreateWindowExW(WS_EX_NOACTIVATE, (STR)s_agentWindowClassAtom, name, WS_POPUP, 0, 0, 0, 0, HWND_MESSAGE, 0, 0, 0);
		}
		//Printf(L"s_nAgentThreads=%i t_agentWnd=%i s_agentWindowClassAtom=%i", s_nAgentThreads, (int)t_agentWnd, s_agentWindowClassAtom);
		return t_agentWnd;
	}
} //namespace inproc

LRESULT WINAPI HookCallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	CWPSTRUCT& m = *(CWPSTRUCT*)lParam;

	//Print(m.message); //TODO: unhook before creating agent window
	if(m.message == 0 && m.wParam == c_injectWparam) {
		ReplyMessage((LRESULT)inproc::CreateAgentWindow());
		return 1;
	} //else Printx(m.message);

	return CallNextHookEx(0, nCode, wParam, lParam);
}

bool InjectDll(HWND w, out HWND& wa, DWORD tid, DWORD pid)
{
	auto hh = SetWindowsHookExW(WH_CALLWNDPROC, HookCallWndProc, s_moduleHandle, tid); if(hh == 0) return false;
	wa = (HWND)SendMessage(w, 0, c_injectWparam, 0);
	UnhookWindowsHookEx(hh);
	if(wa) return true;

	//problem: does not work with windows of class "Windows.UI.Core.CoreWindow", ie Windows Store app processes.
	//	SetWindowsHookEx succeeds, but the hook proc is never called, and the dll is not injected.
	//	But SendMessage works, eg WM_CLOSE closes the app.
	//	The CreateRemoteThread(LoadLibrary) method fails too. LoadLibrary returns 0.
	//	Found some info on the internet. The dll must be signed etc. Or it can be in system directory, and with LoadLibrary use just file name.
	//	Also there are other protected processes, for example some system processes, antivirus, protected processes of web browsers.

	return false;
}

extern "C" __declspec(dllexport)
void WINAPI Cpp_RunDllW(HWND hwnd, HINSTANCE hinst, STR lpszCmdLine, int nCmdShow) //note: with W
{
	//Printf(L"Cpp_RunDllW, %i, %s", IsThisProcess64Bit(), lpszCmdLine);
	HWND w = (HWND)(LRESULT)_wtoi(lpszCmdLine), wa;
	DWORD pid, tid = GetWindowThreadProcessId(w, &pid); if(tid == 0) return;
	if(!InjectDll(w, out wa, tid, pid)) return;
}

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

	SimpleStringBuilder b; LPWSTR t; int n;
	t = b.GetBufferToAppend(out n); b.FixBuffer(GetWindowsDirectoryW(t, n));
	b << rundll;
	t = b.GetBufferToAppend(out n); b.FixBuffer(GetModuleFileNameW(s_moduleHandle, t, n));
	auto u = wcsrchr(t, '\\') - 5; u[0] = bits[0]; u[1] = bits[1]; //"32" to "64" or vice versa
	if(GetFileAttributes(t) == INVALID_FILE_ATTRIBUTES) return false; //avoid messagebox when our antimatter dll does not exist
	b << L"\",Cpp_RunDll " << (__int64)w; //note: without W

	//Print(s.str()); return false;

	STARTUPINFOW si = { sizeof(si) }; si.dwFlags = STARTF_FORCEOFFFEEDBACK;
	PROCESS_INFORMATION pi;
	bool ok = CreateProcessW(0, b.str(), 0, 0, 0, 0, 0, 0, &si, &pi);
	if(!ok) return false;
	CloseHandle(pi.hThread);
	ok = WAIT_OBJECT_0 == WaitForSingleObject(pi.hProcess, INFINITE);
	CloseHandle(pi.hProcess);
	return ok;
}

class HwndTidCache {
	DWORD _tid, _hwnd;
	ULONGLONG _time;
public:
	bool Get(DWORD tid, out HWND& w)
	{
		if(tid != _tid) return false;
		ULONGLONG time = GetTickCount64(); if(time - _time > 10000) return false;
		HWND wCached = (HWND)(LPARAM)_hwnd;
		if(tid != GetWindowThreadProcessId(wCached, null)) return false;
		_time = time;
		w = wCached;
		return true;
	}

	void Set(DWORD tid, HWND w)
	{
		_tid = tid;
		_hwnd = (int)(LPARAM)w;
		_time = GetTickCount64();
	}
};

enum eAccFindErrors
{
	ER_NotFound = 1,
	ER_InvalidWindow,
	ER_WindowOfThisThread,
	ER_Inject,
	ER_InitIPC,
	ER_WaitSearchMutex,
	ER_WaitResultMutex, //TODO
	ER_ParamsTooBig,
	ER_PostMessage,
	ER_ServerThreadEnded,
	ER_WindowClosed,
	ER_ObjectFromSM,
	ER_ObjectToSM, //TODO
	//ER_WindowFromAccessibleObject,
};

HRESULT GetAgentWindow(HWND w, out HWND& wa)
{
	wa = 0;
	DWORD pid, tid = GetWindowThreadProcessId(w, &pid); if(tid == 0) return ER_InvalidWindow;

	//use a primitive HWND cache to make slightly faster, for example when waiting for AO in w
	static thread_local HwndTidCache t_wndCache, t_wndCacheFailed;
	if(t_wndCache.Get(tid, out wa)) return 0;
	if(t_wndCacheFailed.Get(tid, out wa)) return ER_Inject;

	if(tid == GetCurrentThreadId()) return ER_WindowOfThisThread;

	//Perf.First();
	wchar_t name[12]; _itow(tid, name, 10);
	wa = FindWindowEx(HWND_MESSAGE, 0, c_agentWindowClassName, name);
	//Perf.Next();
	if(!wa) {
		bool ok = false, is64bit, differentBits = IsProcess64Bit(pid, out is64bit) && is64bit != IsThisProcess64Bit();
		//Perf.Next();
		if(differentBits) {
			Print(L"note: process of different bitness. Make sure 64 and 32 bit dll versions are built.");

			if(RunDll(w)) { //speed: 60-100 ms
				wa = FindWindowEx(HWND_MESSAGE, 0, c_agentWindowClassName, name);
				ok = !!wa;
			}
		} else {
			ok = InjectDll(w, out wa, tid, pid);
		}
		//Perf.Next();

		if(!ok) {
			if(!IsWindow(w)) return ER_WindowClosed;
			t_wndCacheFailed.Set(tid, w);
			return ER_Inject;
		}
	}
	//Perf.Write();

	t_wndCache.Set(tid, wa);

	return 0;
}

//struct Cpp_AccFindResults
//{
//	int countOrError;
//	union {
//		Cpp_Acc a[1];
//		WCHAR errorText[1];
//	};
//};

using AccCallbackT = BOOL(__stdcall*)(Cpp_Acc& a);

class _AutoIncrementConnectionIdInDtor {
public:
	~_AutoIncrementConnectionIdInDtor() { ((SM_HEADER*)s_sm.Mem())->connectionId++; }
};

extern "C" __declspec(dllexport)
HRESULT Cpp_AccFind(bool inject, HWND w, IAccessible* iaccParent, STR role, STR name, int flags, AccCallbackT also, int skip, out Cpp_Acc& aResult)
{
	//TODO: in C# lib use a static configuration object, where users can define which method to use for which window (by classname etc).
	//	Methods: in-proc IAccessible, out-proc IAccessible, IUIAutomation, maybe more in the future.
	//	By defaul would set Windows Store app classnames to use UI Automation. Chrome, Firefox - in-proc. All other out-proc or in-proc.
	//	It would be default. Can be overridden with function flags.

	//note: with Firefox by default slow anyway, only maybe 30% faster than out-process.
	//	To make fast, need to disable the Firefox multiprocess feature:
	//	In about:config set this to false: browser.tabs.remote.force-enable
	//	Firefox sets this to true when upgrading, eg from version 57 to 58. Even if both installed, if you run 57 and then 58.

	//SetThreadAffinityMask(GetCurrentThread(), 1); //for testing single CPU
	//SetThreadPriority(GetCurrentThread(), 2); //for testing single CPU

	eAgentAction action = eAgentAction::AccFindInWnd;

	Perf.First();
	HRESULT R;
	HWND wAgent = 0;
	if(inject) {
		//if(iaccParent != null) {
		//	action = eAgentAction::AccFindInAcc;
		//	if(WindowFromAccessibleObject(iaccParent, &w)) return ER_WindowFromAccessibleObject;
		//	Perf.Next();
		//}

		if(R = GetAgentWindow(w, out wAgent)) return R;
		Perf.Next();

		if(!InitIPC()) return ER_InitIPC;
		Perf.Next();

		switch(WaitForSingleObject(s_searchMutexOfClient, INFINITE)) { //never mind: does not allow other threads to search in other windows while we search. To implement it, need mutex+event for each agent window.
		case 0: case WAIT_ABANDONED: break;
		default: Print(L"failed wait s_searchMutexOfClient"); return ER_WaitSearchMutex;
		}
		AutoReleaseMutex arSearchMutex(s_searchMutexOfClient);
		Perf.Next();

		auto hSM = (SM_HEADER*)s_sm.Mem();

		int connectionId = ++hSM->connectionId; //if previous client terminated and server is still searching, this will stop the search

		//if previous client abandoned its search, wait until server stops the search
		int wr = WaitForSingleObject(s_searchMutexOfServer, INFINITE);
		assert(wr == 0 || wr == WAIT_ABANDONED);
		ReleaseMutex(s_searchMutexOfServer);

		_AutoIncrementConnectionIdInDtor aiciid; //will increment hSM->connectionId. Then server will stop the search.
		Perf.Next();

		int roleLen = 0, nameLen = 0;
		if(role) roleLen = (int)wcslen(role) + 1;
		if(name) nameLen = (int)wcslen(name) + 1;

		int memSize = AccFindParams::CalcMemSize(roleLen, nameLen);

		if(memSize + sizeof(SM_HEADER) > c_sharedMemorySize) return ER_ParamsTooBig;
		auto p = (AccFindParams*)(hSM + 1);
		p->size = memSize;
		p->hwnd = (int)(LPARAM)w;
		p->SetRole(role, roleLen);
		p->SetName(name, nameLen);
		p->flags = flags;
		p->findAll = also != null;
		p->skip = skip;

		//if(iaccParent != null) {
		//	hSM->n = 1; hSM->size = memSize;
		//	HRESULT hr = ObjectToSM(hSM, iaccParent, 0);
		//	if(hr) return ER_ObjectToSM;
		//}

		//slightly faster than SendMessage/ReplyMessage. Same speed as SendNotifyMessage.
		if(!PostMessageW(wAgent, WM_USER, MAKEWPARAM(action, c_accVersion), connectionId)) return ER_PostMessage;
		Perf.Next();

		//agent sets window long, finds AO, sets event, resets window long. We wait for the event.

		CInterfaceArray<IAccessible> ar;
	g1:
		for(;;) {
			auto r = WaitForSingleObject(s_resultEvent, 1000);
			if(r == 0) break;
			if(r != WAIT_TIMEOUT) return E_FAIL;
			if(!IsWindow(wAgent)) return ER_ServerThreadEnded;
			if(!IsWindow(w)) return ER_WindowClosed;
		}

		if(also == null) {
			Perf.Next();
			SM_HEADER* h = hSM;
			if(h->resultCount == 0) {
				R = ER_NotFound;
			} else {
				assert(h->resultCount == 1);
				//Perf.First();
				int start = sizeof(*h);
				HRESULT hr = ObjectFromSM(h, ref start, out aResult.iacc, out aResult.elem);
				//Perf.NW();
				if(hr != 0) {
					Printf(L"ObjectFromSM: %i", hr);
					return ER_ObjectFromSM;
				}
				R = 0;

				//CComBSTR ret; _variant_t params(L"params"); long unused = 0;
				//aResult.iacc->get_accHelpTopic(&ret, params, &unused);
				////for(int i = 0; i < 5; i++) {
				////	Sleep(100);
				////	ret.Empty();
				////	Perf.First();
				////	aResult.iacc->get_accHelpTopic(&ret, params, &unused);
				////	Perf.NW();
				////}
				//Printf(L"%i %s", SysStringLen(ret), (STR)ret);
			}
		} else {
			if(0 != WaitForSingleObject(s_resultMutex, INFINITE)) {
				assert(!"failed WaitForSingleObject(s_resultMutex)");
				return ER_WaitResultMutex;
			}
			AutoReleaseMutex arResultMutex(s_resultMutex);

			int n = hSM->resultCount;
			if(n == 0) {
				//Print(L"0 objects");
				goto g1;
			}
			hSM->resultCount = 0;
			CAutoVectorPtr<BYTE> smCopy;
			smCopy.Allocate(hSM->resultSize); memcpy(smCopy, hSM, hSM->resultSize);
			SM_HEADER* h = (SM_HEADER*)smCopy.m_p;

			arResultMutex.ReleaseNow();

			//Printf(L"n=%i", n);

			//Perf.First();
			for(int i = 0, start = sizeof(*h); i < n; i++) {
				IAccessible* iacc; long elem;
				//Perf.First();
				HRESULT hr = ObjectFromSM(h, ref start, out iacc, out elem);
				//Perf.NW();
				if(hr != 0) {
					Printf(L"ObjectFromSM: %i", hr);
					return ER_ObjectFromSM;
				}
				if(iacc == null) { R = ER_NotFound; goto g2; }
				Cpp_Acc u(iacc, elem);
				also(ref u);
				if(u.iacc) ar[ar.Add()].Attach(iacc);
			}
			//Perf.Next();
			//Perf.Write();
			goto g1;
		g2:;
			Perf.Next();
		}
	} else {
		//hr = TestAccFindInWnd(0, w, role, name);
		R = ER_NotFound;
	}
	Perf.NW();
	return R;

	//TODO: releasing all is the slowest part, because switches context for each.
	//	Instead use single IAccessibleCollection object.
	//	Always at first get all, only then call callback. Anyway, getting properties don't work until searching finished.
	//	Can try CoDisconnectObject, but need to call it from server side.
	//	Try to release in a thread pool thread. To see whether it works, use PrintComRefCount; also somehow look whether the object released/deleted at server side.
	//		Or just create C# Acc, and let GC release later in other thread.

	//TODO: now Acc releases its IAccessible in GC thread. Maybe should release in same thread. It seems RCW does it, ie releases when thread exits.
}



extern "C" __declspec(dllexport)
void Cpp_Unload()
{
	for(HWND wPrev = 0; ; ) {
		HWND w = FindWindowEx(HWND_MESSAGE, 0, c_agentWindowClassName, null);
		if(w == 0 || w == wPrev) break;
		wPrev = w;
		DWORD_PTR res;
		SendMessageTimeout(w, WM_CLOSE, 0, 0, SMTO_ABORTIFHUNG, 5000, &res);
	}
}
#endif //!IPC_VERSION
