 /
function# [hwndFrom] [flags] ;;flags: 1 retry from top, 2 skip minimized, 4 all desktops, 8 like Alt+Tab

 Gets next window in Z order, skipping invisible and other windows that would not be added to taskbar or not activated by Alt+Tab.
 Returns window handle. Returns 0 if there are no such windows.

 hwndFrom - handle of window behind which to search. If 0, starts from the top of Z order.
 flags:
   1 - if hwndFrom is not 0 and there are no matching windows behind it, retry from the top of the Z order. Like Alt+Tab does. Can return hwndFrom.
   2 - skip minimized windows.
   4 (QM 2.4.3) - on Windows 10 include windows on all virtual desktops. On Windows 8 include Windows store apps.
   8 (QM 2.4.3) - emulate Alt+Tab behavior with owned windows (message boxes, dialogs):
      If hwndFrom is such owned window, skip its owner.
      If the found window has an owned window that was active more recently, return that owned window.

 REMARKS
 RealGetNextWindow(win 1|8) ideally should get the same window as would be activated by Alt+Tab. However it is not always possible. Sometimes it will be a different window.
 Without flags 1|8 this function can be used to get main application windows like in taskbar. It is used by <help>GetMainWindows</help>.

 Added in: QM 2.2.1. In QM 2.4.3 redesigned and works slightly diferently.


int lastFound w2 w=hwndFrom
if(!w) flags~1
rep
	w=iif(w GetWindow(w GW_HWNDNEXT) GetTopWindow(0))
	if !w
		if(flags&1) flags~1; continue
		ret lastFound
	
	if(!IsWindowVisible(w)) continue
	
	int exstyle=GetWindowLong(w GWL_EXSTYLE)
	if exstyle&WS_EX_APPWINDOW=0
		if(exstyle&(WS_EX_TOOLWINDOW|WS_EX_NOACTIVATE)) continue
		w2=GetWindow(w GW_OWNER); if(w2) if(flags&8=0 or IsWindowVisible(w2)) continue
	
	#region IsVisibleReally
	if(flags&2 and IsIconic(w)) continue
	
	if _winver>=0xA00 ;;10
		if IsWindowCloaked(w)
			if(flags&4=0) continue
			if exstyle&WS_EX_NOREDIRECTIONBITMAP ;;probably a store app
				sel WinTest(w "Windows.UI.Core.CoreWindow[]ApplicationFrameWindow")
					case 1 continue ;;Windows search, experience host, etc. Also app windows that normally would sit on ApplicationFrameWindow windows.
					case 2 if(!sub_sys.GetWindowsStoreAppFrameChild(w)) continue
	else if _winver>=0x602 ;;8
		if exstyle&WS_EX_NOREDIRECTIONBITMAP and GetWindowLong(w GWL_STYLE)&WS_CAPTION=0
			if(flags&4=0 and exstyle&WS_EX_TOPMOST) continue ;;skip store apps
			int pid pidShell; if(GetWindowThreadProcessId(GetShellWindow &pidShell) and GetWindowThreadProcessId(w &pid) and pid=pidShell) continue ;;skip captionless shell windows
		 On Win8 impossible to get next window like Alt+Tab. All store apps are topmost, covering non-topmost desktop windows. DwmGetWindowAttribute has no sense here. Desktop windows are never cloaked, inactive store windows are cloaked, etc.
	#endregion
	
	if flags&8
		w2=GetLastActivePopup(GetAncestor(w 3)) ;;call with the root owner, because GLAP returns w if w has an owner (documented)
		if(w2!w) if(!IsWindowVisible(w2) or (flags&2 and IsIconic(w2))) w2=w ;;don't check cloaked etc for owned window if its owner passed
		if(w2=hwndFrom) lastFound=w2; continue
		w=w2
	
	ret w
	err+
