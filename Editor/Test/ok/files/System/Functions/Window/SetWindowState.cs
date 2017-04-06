 /
function hwnd state [flags] ;;state: 0 hide, 1 normal, 3 max, 4 restore no act, 6 min, 7 min no deact, 8 show, 9 restore.  flags: 1 fast

 Makes window maximized, minimized, normal, hidden or visible.

 hwnd - window handle.
 state - one of:
   0 (SW_HIDE) - hide. See also <help>hid</help>.
   1 (SW_SHOWNORMAL) - make normal (not min/max). If minimized, activate.
   3 (SW_SHOWMAXIMIZED) - maximize. If minimized, activate.
   4 (SW_SHOWNOACTIVATE) - restore without activating. If flag 1 used, makes normal (like 1), else restores (like 9).
   6 (SW_MINIMIZE) - minimize. If the window was active, activate next window.
   7 (SW_SHOWMINNOACTIVE) - minimize. Don't activate next window.
   8 (SW_SHOWNA) - show. Don't activate.
   9 (SW_RESTORE) - restore. If minimized, activate. If minimized, make normal or maximized, like before minimizing.
   11 (SW_FORCEMINIMIZE) - minimize even if hung.
   101 (with flag 1) - minimize so that the window will be normal when restored.
   102 (with flag 1) - minimize so that the window will be maximized when restored.
 flags:
   1 - use different method. Window animation is disabled.

 REMARKS
 This function works slightly differently than <help #IDP_MIN>max/min/res</help>.
 Not all windows properly respond to this function.


int cor
sel(state) case [1,3,9] cor=min(hwnd)+1 ;;*

WINDOWPLACEMENT p.Length=sizeof(p)
if(flags&1)
	GetWindowPlacement hwnd &p
	sel(state)
		case SW_RESTORE if(p.showCmd=SW_SHOWMINIMIZED and p.flags&WPF_RESTORETOMAXIMIZED) state=SW_SHOWMAXIMIZED ;;without this would make normal, whereas ShowWindow restores
		case [SW_MINIMIZE,SW_SHOWMINNOACTIVE,SW_FORCEMINIMIZE] if(IsZoomed(hwnd)) p.flags|WPF_RESTORETOMAXIMIZED; else p.flags~WPF_RESTORETOMAXIMIZED ;;Windows forgets to remove the flag
		case 101 state=6; p.flags~WPF_RESTORETOMAXIMIZED
		case 102 state=6; p.flags|WPF_RESTORETOMAXIMIZED
	p.showCmd=state
	SetWindowPlacement hwnd &p
else
	ShowWindow hwnd state

if(cor) if(cor=2 or !win) act hwnd ;;*
err+

 * fix inactive window bug. ShowWindow, when restoring minimized window of other process, would leave no active window.
 ShowWindowAsync activates, but it's also buggy. When maximizing inactive window, sets its caption and caret like of active window, but the window is not active.
 note: tried to make state 4 behavior same in both modes but it is difficult and would be dirty.
 ShowWindow states with my comments are in 'ShowWindow notes' macro.

 These are unclear or useless or incosistent:
   2 (SW_SHOWMINIMIZED) - minimize. API doc says that also activates, but it was not true in my tests. Was same as 7 (SW_SHOWMINNOACTIVE).
   10 (SW_SHOWDEFAULT) - show as specified by the program that started the process. Not sure whether it always works.
