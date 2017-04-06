A window is on all monitors in these cases:

1. Created as WS_EX_TOPMOST.
If need that the window would be topmost but not on all monitors:
	Create as non-topmost.
	On wm_initdialog/wm_create PostMessage WM_APP.
	On WM_APP set it topmost.

2. If has WS_EX_TOOLWINDOW or WS_EX_NOACTIVATE, unless is owned.
To make not on all monitors without removing these styles:
	Create as owned, eg use QM as owner.
	On wm_initdialog/wm_create PostMessage WM_APP.
	On WM_APP: SetWindowLong(hWnd GWL_HWNDPARENT 0).
