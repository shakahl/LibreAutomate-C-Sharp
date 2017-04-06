 /
function! hwnd hwndDesktopOf [flags] ;;flags: 1 activate hwnd and its desktop

 Moves the window (hwnd) to the virtual desktop of another window (hwndDesktopOf).
 Returns: 1 success, 0 failed.

 REMARKS
 hwnd must belong to the current process (QM). Else fails.
 Also moves windows owned by hwnd and the owner of hwnd.
 Fails if hwndDesktopOf is on all desktops. If hwnd is on all desktops, returns 1 but does not hide the window in other desktops.
 Virtual desktops is a Windows 10 feature. On older OS this function does nothing and returns 0.


if(_winver<0xA00) ret

#compile "IVirtualDesktopManager"

hwnd=GetAncestor(hwnd 3); if(!hwnd) ret ;;would not move an owned window, although MoveWindowToDesktop returns success
hwndDesktopOf=GetAncestor(hwndDesktopOf 3); if(!hwndDesktopOf) ret ;;would not get desktop id of an owned window

IVirtualDesktopManager_Raw-- m._create("{aa509086-5ca9-4c25-8f95-589d3c07b48a}")

GUID g
if m.GetWindowDesktopId(hwndDesktopOf g)
	ret
if(m.MoveWindowToDesktop(hwnd g)) ret
if(flags&1) act hwnd; err
ret 1

 tested: if hwnd is of another process, MWTD error "access denied".
 shoulddo: how to set a window to be on all desktops? It depends on window style used when creating it.
 more comments in IsWindowOnCurrentDesktop
