 /

 Changes the target control of WM_MOUSEWHEEL messages received by the current thread.
 Windows OS sends these messages to the focused control. This function sets a hook that redirects them to the control from mouse, if it belongs to the current thread.
 Call this function when you have a dialog with scrollable controls and want the wheel to scroll the control from mouse, not the focused control.
 Call it for example before ShowDialog or under case WM_INITDIALOG. It affects all dialogs and other windows of that thread.

 Added in: QM 2.4.2.


__WindowsHook-- hh=SetWindowsHookEx(WH_GETMESSAGE &sub.HookProc 0 GetCurrentThreadId)


#sub HookProc
function# nCode remove MSG&m
if(nCode<0) goto gNext

if m.message=WM_MOUSEWHEEL
	_i=child(mouse 1)
	if(GetWindowThreadProcessId(_i 0)=GetCurrentThreadId) m.hwnd=_i

 gNext
ret CallNextHookEx(0 nCode remove &m)
