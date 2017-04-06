 /MMT_Main

MMTVAR- v

 unregister appbar
APPBARDATA ab.cbSize=sizeof(ab); ab.hWnd=v.hwnd
SHAppBarMessage(ABM_REMOVE &ab)

PostQuitMessage 0

 _________________________________

 Difficult to unregister the appbar. It is necessary on OS < Win7.
 If we unregister in this thread, QM may crash when QM tries to end this thread (user ends it from Running Items, or when QM exits).
 Because it may resize QM window (if it is maximized). Then QM main thread and this thread locks each other.
 Also cannot start other thread that would unregister. It will not start on QM exit.
 Tried to unregister in other process, but not always unregisters successfully, don't know why, maybe this thread must be still running when doing it.
 Better if whole this app runs as exe.
