//rejected:
//	Cpp_AccFromWindow - slower.
//	Cpp_AccFromPoint - unreliable and not much faster. Normal API is not too slow.
#if false

extern "C" __declspec(dllexport)
HRESULT Cpp_AccFromWindow(bool inject, HWND w, DWORD objId, out Cpp_Acc& aResult);
extern "C" __declspec(dllexport)
HRESULT Cpp_AccFromPoint(bool inject, POINT p, out Cpp_Acc& aResult);


void _AgentAction(UINT msg, WPARAM wParam, LPARAM lParam)
{
	switch(msg) {
	case WM_USER: {
		eAgentAction action = (eAgentAction)wParam;
		switch(action) {
		case eAgentAction::AccFindInWnd:
			_AccFind1(action, (DWORD)lParam);
			break;
		}
	}break;
	case WM_USER + 1: {
		Cpp_Acc a;
		HRESULT hr = Cpp_AccFromWindow(false, (HWND)wParam, (DWORD)lParam, out a);
		if(hr) a.iacc = null;
		_WriteSingleResultCallback(a.iacc, 0);
		if(a.iacc) a.iacc->Release();
	}break;
	case WM_USER + 2: {
		POINT p = { (int)wParam, (int)lParam };
		Cpp_Acc a;
		HRESULT hr = Cpp_AccFromPoint(false, p, out a);
		if(hr) a.iacc = null;
		_WriteSingleResultCallback(a.iacc, 0);
		if(a.iacc) a.iacc->Release();
	}break;
	}
}


HRESULT _AccFrom(int action, HWND w, WPARAM wParam, LPARAM lParam, out Cpp_Acc& aResult)
{
	HRESULT hr; HWND wAgent;
	if(hr = GetAgentWindow(w, out wAgent)) return hr;
	if(!InitIPC()) return ER_InitIPC;

	switch(WaitForSingleObject(s_searchMutex, INFINITE)) { //never mind: does not allow other threads to search in other windows while we search. To implement it, need mutex+event for each agent window.
	case 0: case WAIT_ABANDONED: break;
	default: Print(L"failed wait s_searchMutex"); return ER_WaitSearchMutex;
	}
	AutoReleaseMutex searchMutexAR(s_searchMutex);
	while(GetWindowLongW(wAgent, 0)) { //TODO: now this works, but makes difficult for the server thread to check whether stopped
		Sleep(50);
	}

	if(!PostMessageW(wAgent, WM_USER + action, wParam, lParam)) return ER_PostMessage;
	//if(!SendNotifyMessageW(wAgent, WM_USER + action, wParam, lParam)) return ER_PostMessage;
	//DWORD_PTR smtResut;
	//if(!SendMessageTimeoutW(wAgent, WM_USER + action, wParam, lParam, 0, 3000, &smtResut)) return ER_PostMessage;

	for(;;) {
		auto r = WaitForSingleObject(s_resultEvent, 1000);
		if(r == 0) break;
		if(r != WAIT_TIMEOUT) return E_FAIL;
		if(!IsWindow(wAgent)) return ER_ServerThreadEnded;
		if(!IsWindow(w)) return ER_WindowClosed;
	}

	SM_HEADER* h = (SM_HEADER*)s_sm.Mem();
	if(h->n == 0) return ER_NotFound;
	int start = sizeof(*h);
	hr = ObjectFromSM(h, ref start, out aResult.iacc, out aResult.elem);
	//Perf.NW();
	if(hr != 0) {
		Printf(L"ObjectFromSM: %i", hr);
		return ER_ObjectFromSM;
	}

	return 0;
}

extern "C" __declspec(dllexport)
HRESULT Cpp_AccFromWindow(bool inject, HWND w, DWORD objId, out Cpp_Acc& aResult)
{
	if(!inject) {
		aResult.elem = 0;
		auto hr = AccessibleObjectFromWindow(w, objId, IID_IAccessible, (void**)&aResult.iacc);
		if(hr) {
			aResult.iacc = null;
		}
		return hr;
	}

	return _AccFrom(1, w, (WPARAM)w, objId, out aResult);
}

extern "C" __declspec(dllexport)
HRESULT Cpp_AccFromPoint(bool inject, POINT p, out Cpp_Acc& aResult)
{
	if(!inject) {
		VARIANT v;
		auto hr = AccessibleObjectFromPoint(p, &aResult.iacc, &v);
		if(hr) {
			aResult.iacc = null;
			aResult.elem = 0;
		} else {
			aResult.elem = v.lVal;
		}
		return hr;
	}

	HWND w = WindowFromPoint(p); if(!w) return ER_NotFound; //TODO: unreliable

	return _AccFrom(2, w, p.x, p.y, out aResult);
}


[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
internal static extern int Cpp_AccFromWindow([MarshalAs(UnmanagedType.I1)] bool inject, Wnd w, int objId, out Cpp_Acc aResult);

[DllImport("AuCpp.dll", CallingConvention = CallingConvention.Cdecl)]
internal static extern int Cpp_AccFromPoint([MarshalAs(UnmanagedType.I1)] bool inject, Point p, out Cpp_Acc aResult);

#endif
