 /
function# hwnd $rname $rkey save [flags] ;;save: 1 save, 0 restore.  flags: 1 show window

 Saves or restores window position and width/height to/from the registry.
 Returns 1 if successfully restored previously saved position. Else returns 0.

 hwnd - window handle.
 rname - registry value name.
 rkey - registry key.
   The function uses QM registry functions <help>rset</help>/rget with HKEY_CURRENT_USER.
 save - see above.
 flags (QM 2.3.4):
   1 - when restoring, show and activate the window.
     Without this flag, if the window is currently hidden (eg the function called under WM_INITDIALOG), it remains hidden. Then cannot set maximized state (Windows API limitation).

 REMARKS
 If the window was minimized or hidden when saving, restores to the most recent visible state (normal or maximized).

 EXAMPLE
  in dialog procedure
	 case WM_INITDIALOG
	 RegWinPos hDlg getopt(itemname 6) "\RegWinPos" 0
	 case WM_DESTROY
	 RegWinPos hDlg getopt(itemname 6) "\RegWinPos" 1


WINDOWPLACEMENT w.Length=sizeof(WINDOWPLACEMENT)

if save
	if GetWindowPlacement(hwnd &w)
		if(w.showCmd=SW_SHOWMINIMIZED) w.showCmd=iif(w.flags&WPF_RESTORETOMAXIMIZED SW_SHOWMAXIMIZED SW_SHOWNORMAL) ;;info: MSDN says that flags always 0, but it's not true
		rset w rname rkey
else
	if rget(w rname rkey)=sizeof(w)
		if(flags&1=0 and !IsWindowVisible(hwnd)) w.showCmd=SW_HIDE ;;info: when showing later, always will be normal; don't know how to maximize hidden, sorry, tried everything
		ret SetWindowPlacement(hwnd &w)
