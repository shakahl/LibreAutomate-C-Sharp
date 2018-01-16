#include "stdafx.h"
#include "cpp.h"
#if IPC_VERSION==2

void TestAccFindInAcc(AccCallback* callback, bool findAll, IAccessible* iaccParent, STR role, STR name);


HMODULE s_moduleHandle;
const int c_injectWparam = -1'572'289'143;
const STR c_agentWindowClassName = L"AuIPC1";

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

namespace inproc
{
	HRESULT STDMETHODCALLTYPE Hook_get_accHelpTopic(IAccessible* iacc, out BSTR* pbRet, VARIANT vParams, long* u1);

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

	AccHooks s_accHooks;

	class AccCallbackInProc : public AccCallback
	{
		IStreamPtr _stream;
		//int _nFound;
		bool _findAll;

	public:
		AccCallbackInProc(bool findAll) : _findAll(findAll) {
			//_nFound = 0;
		}

		//fullCheck - do a full check, which is slower. If false, does not detect when the client thread terminated.
		bool IsClientStillWaiting(bool fullCheck) {
			return true; //TODO: remove
		}

		void WriteResult(IAccessible* iacc, long elem) {

			//Print("WriteResult");
			s_accHooks.Hook(iacc);
			if(!_stream && CreateStreamOnHGlobal(0, true, &_stream)) return;
			if(CoMarshalInterface(_stream, IID_IAccessible, iacc, MSHCTX_LOCAL, null, MSHLFLAGS_NORMAL)) goto ge;
			if(_stream->Write(&elem, 4, null)) goto ge;
			//Print("write ok");

			//_nFound++;
			return;
		ge:
			_stream->SetSize(ULI(0));
			//_nFound = 0;
		}

		void Finished() {
			//if(_findAll) _WriteMultiResult(null, 0);
		}

		HRESULT ToBSTR(out BSTR& b) {
			b = null;
			if(!_stream) return 1001; //not found
			STATSTG stat; if(_stream->Stat(&stat, STATFLAG_NONAME)) return 1002; //get stream size
			DWORD streamSize = stat.cbSize.LowPart, readSize;
			if(streamSize == 0) return 1003; //failed to marshal object
			if(_stream->Seek(LI(0), STREAM_SEEK_SET, null)) return 1002;
			b = SysAllocStringByteLen(null, streamSize);
			if(_stream->Read(b, streamSize, &readSize) || readSize != streamSize) {
				SysFreeString(b);
				b = null;
				return 1002;
			}
			return 1000;
		}
	};

	HRESULT STDMETHODCALLTYPE Hook_get_accHelpTopic(IAccessible* iacc, out BSTR* pbRet, VARIANT vParams, long* u1)
	{
		if(vParams.vt == VT_BSTR && vParams.bstrVal != null) {
			//Print("hook");
			try {
				auto p = (AccFindParams*)vParams.bstrVal;

				AccCallbackInProc callback(p->findAll);

				//Print(p->Role());
				//Print(p->Name());
				//Print(p->findAll);

				TestAccFindInAcc(&callback, p->findAll, iacc, p->Role(), p->Name());

				return callback.ToBSTR(out *pbRet);
			}
			catch(...) { PRINT(L"exception"); }
		}
		return E_NOTIMPL;
		//never mind: we don't call the old method (_RESTORE::p). Nobody implement or use it.
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

	LRESULT _AgentAction(UINT msg, WPARAM wParam, LPARAM lParam)
	{
		switch(msg) {
		case WM_USER: {
			IAccessiblePtr a;
			if(AccessibleObjectFromWindow((HWND)lParam, 0, IID_IAccessible, (void**)&a)) break;
			if(!s_accHooks.Hook(a)) break;
			return LresultFromObject(IID_IAccessible, 0, a);
		}break;
		}
		return 0;
	}

	LRESULT WINAPI AgentWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
	{
		//Printf(L"%i 0x%X", (int)hwnd, msg);
		//Printx(msg);
		if(msg >= WM_USER && msg < WM_USER * 2) {
			try { return _AgentAction(msg, wParam, lParam); }
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
			if(1 == InterlockedIncrement(&s_nAgentThreads)) {
				//increment dll refcount to prevent unloading this dll when the caller process unhooks this hook
				HMODULE hm = 0;
				GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS, (STR)s_moduleHandle, &hm);

				WNDCLASS c = { };
				c.hInstance = s_moduleHandle;
				c.lpfnWndProc = AgentWndProc;
				c.lpszClassName = c_agentWindowClassName;
				//c.cbWndExtra = 4;
				s_agentWindowClassAtom = RegisterClassW(&c);
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

	if(m.message == 0 && m.wParam == c_injectWparam) {
		HWND wAgent = inproc::CreateAgentWindow();
		ReplyMessage((LRESULT)wAgent);
		return 1;
		//tested: not faster if we unhook before creating window
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
	//TODO: can cache its IAccessible here.
	//	Or instead of all this use a Cpp_AccFindContext* that can be used when waiting.
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
	ER_GetWindowAO,
	ER_WindowClosed,
	ER_Marshal,
	ER_Unmarshal,
	ER_ObjectInterfaceNotHooked,
};

HRESULT GetAgentWindow(HWND w, out HWND& wa)
{
	//TODO: need mutex?

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

extern "C" __declspec(dllexport)
HRESULT Cpp_AccFromWindow(bool inject, HWND w, DWORD objid, out IAccessible** aResult)
{
	*aResult = null;

	if(!inject) return AccessibleObjectFromWindow(w, objid, IID_IAccessible, (void**)aResult);

	HRESULT R; HWND wAgent = 0;
	if(R = GetAgentWindow(w, out wAgent)) return R;
	Perf.Next();

	LRESULT ro = SendMessageW(wAgent, WM_USER, 0, (LPARAM)w);
	if(ro == 0) return ER_GetWindowAO;
	Perf.Next();
	if(ObjectFromLresult(ro, IID_IAccessible, 0, (void**)aResult)) return ER_GetWindowAO;
	Perf.Next();
	return 0;
}

extern "C" __declspec(dllexport)
//HRESULT Cpp_AccFind2(bool inject, HWND w, HWND& wAgent, IAccessible*& iaccParent, STR role, STR name, int flags, AccCallbackT also, int skip, out Cpp_Acc& aResult)
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

	Perf.First();
	HRESULT R;
	if(inject) {
		IAccessiblePtr iaccParentRelease;
		if(iaccParent == null) {
			if(R = Cpp_AccFromWindow(true, w, 0, &iaccParent)) return R;
			iaccParentRelease.Attach(iaccParent);
		}

		int roleLen = 0, nameLen = 0;
		if(role) roleLen = (int)wcslen(role) + 1;
		if(name) nameLen = (int)wcslen(name) + 1;

		int memSize = AccFindParams::CalcMemSize(roleLen, nameLen);

		BSTR b = SysAllocStringByteLen(null, memSize);
		auto p = (AccFindParams*)b;
		p->size = memSize;
		p->hwnd = (int)(LPARAM)w;
		p->SetRole(role, roleLen);
		p->SetName(name, nameLen);
		p->flags = flags;
		p->findAll = also != null;
		p->skip = skip;
		Perf.Next();

		_variant_t v; v.vt = VT_BSTR; v.bstrVal = b;
		CComBSTR br; long u1 = 0;
		HRESULT hr = iaccParent->get_accHelpTopic(&br, v, &u1);
		if(hr != 1000) {
			if(hr == 1001) return ER_NotFound;
			Print(hr);
			if(hr < 1000 || hr >= 2000) return ER_ObjectInterfaceNotHooked;
			return ER_Marshal;
		}
		Perf.Next();

		DWORD streamSize = SysStringByteLen(br);
		HGLOBAL hg = GlobalAlloc(GMEM_MOVEABLE, streamSize); if(hg == 0) return ER_Unmarshal;
		LPVOID mem = GlobalLock(hg); if(mem == null) { GlobalFree(hg); return ER_Unmarshal; }
		memcpy(mem, br, streamSize);
		GlobalUnlock(mem);
		Perf.Next();

		//Print(streamSize);
		IStreamPtr stream;
		if(CreateStreamOnHGlobal(hg, true, &stream)) { GlobalFree(hg); return ER_Unmarshal; }
		if(CoUnmarshalInterface(stream, IID_IAccessible, (void**)&aResult.iacc)) return ER_Unmarshal;
		if(stream->Read(&aResult.elem, 4, null)) return ER_Unmarshal;
		//never mind: if fails, should call CoReleaseMarshalData?

		R = 0;

		Perf.Next();
		if(true) {
				////Perf.First();
				//int start = sizeof(*h);
				//HRESULT hr = ObjectFromSM(h, ref start, out aResult.iacc, out aResult.elem);
				////Perf.NW();
				//if(hr != 0) {
				//	Printf(L"ObjectFromSM: %i", hr);
				//	return ER_ObjectFromSM;
				//}
				//R = 0;

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
		} else {
			//Printf(L"n=%i", n);

			//Perf.First();
			//for(int i = 0, start = sizeof(*h); i < n; i++) {
			//	IAccessible* iacc; long elem;
			//	//Perf.First();
			//	HRESULT hr = ObjectFromSM(h, ref start, out iacc, out elem);
			//	//Perf.NW();
			//	if(hr != 0) {
			//		Printf(L"ObjectFromSM: %i", hr);
			//		return ER_ObjectFromSM;
			//	}
			//	if(iacc == null) { R = ER_NotFound; goto g2; }
			//	Cpp_Acc u(iacc, elem);
			//	also(ref u);
			//	if(u.iacc) ar[ar.Add()].Attach(iacc);
			//}
			//Perf.Next();
			//Perf.Write();
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
