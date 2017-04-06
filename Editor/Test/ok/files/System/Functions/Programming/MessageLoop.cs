 /
function# [flags]

 Standard message loop.
 
 flags:
   1 - should be used if the message loop is not the main message loop of the thread. If this flag is used, the function reposts WM_QUIT message to the outer loop.
   2 (QM 2.3.0) - enable standard keyboard navigation in modeless dialogs.

 REMARKS
 You can learn more about message loops in MSDN.
 A message loop is required for any thread that wants to receive Windows messages, COM events and some other events. It waits and gets messages, does some standard processing, and passes to window procedures.
 Some windows and functions have own message loop, eg modal dialogs, message boxes, menus. In other cases you can use this function. Or use <help>wait</help> with opt waitmsg 1; then it uses an internal message loop.
 This function returns when this thread receives WM_QUIT message (when destroying main window, call PostQuitMessage) or message 2000 (PostMessage 0 2000 retVal 0). Post[Quit]Message sets the return value.
 Threads with message loop should not use "wait" or "wait for" commands with long wait time, unless opt waitmsg 1 is set.

 See also: <MessageLoopOptions>, <MainWindow>.


MSG m
__MSGLOOP- ___t_ml
int ha idm(flags&2 or ___t_ml.flags&0x100)
rep
	if(GetMessage(&m 0 0 0)<1 or m.message=2000)
		if(flags&1) sel(m.message) case WM_QUIT PostQuitMessage(m.wParam); case 2000 PostMessage m.hwnd 2000 m.wParam m.lParam
		ret m.wParam
	if(___t_ml.cbFunc and call(___t_ml.cbFunc &m ___t_ml.cbParam)) continue
	if(___t_ml.accel) ha=GetAncestor(m.hwnd 2); if(!___t_ml.accelHwnd or ___t_ml.accelHwnd=ha) if(TranslateAccelerator(ha ___t_ml.accel &m)) continue
	if(idm and IsDialogMessage(GetAncestor(m.hwnd 2) &m)) continue
	TranslateMessage &m
	DispatchMessage &m
