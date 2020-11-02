#include "stdafx.h"
#include "cpp.h"


LRESULT CALLBACK ClipboardHook(int code, WPARAM wParam, LPARAM lParam) {
	auto m = (MSG*)lParam;
	if(code < 0) goto g1;
	if(m->message == WM_CLIPBOARDUPDATE) {
		//Print("WM_CLIPBOARDUPDATE");
		char cn[256];
		if(0 == GetClassNameA(m->hwnd, cn, sizeof(cn)) || 0 != strcmp(cn, "Au.DWP")) {
			m->message = 0;
			//Print(cn);
		}
		return 0;
	}
g1:
	return CallNextHookEx(0, code, wParam, lParam);

	//After unhooking, this dll remains loaded until hooked threads receive messages.
	//	To unload when [un]installing, installer calls Cpp_Unload which broadcasts messages to all top-level and message-only windows.
	//	To unload when building, Cpp project's pre-link event runs "unload AuCpp dll.exe" (created from QM2 macro "unload AuCpp dll") which calls Cpp_Unload.
}

EXPORT HHOOK Cpp_Clipboard(HHOOK hh)
{
	if(hh == NULL) {
		auto hh = SetWindowsHookExW(WH_GETMESSAGE, ClipboardHook, GetCurrentModuleHandle(), 0);
		return hh;
	} else {
		UnhookWindowsHookEx(hh);
	}
	return NULL;
}

namespace outproc
{

//EXPORT HRESULT Cpp_StartProcess(STR exeFile, STR args, STR workingDir, STR environment, out BSTR& sResult)
//{
//	HRESULT R;
//	Cpp_Acc aAgent;
//	if(R = InjectDllAndGetAgent(GetShellWindow(), out aAgent.acc)) {
//		return R;
//	}
//
//	InProcCall c;
//	auto p = (MarshalParams_StartProcess*)c.AllocParams(&aAgent, InProcAction::IPA_StartProcess, sizeof(MarshalParams_StartProcess));
//	p->...;
//	if(R = c.Call()) return R;
//	sResult = c.DetachResultBSTR();
//	return 0;
//}

} //namespace outproc
