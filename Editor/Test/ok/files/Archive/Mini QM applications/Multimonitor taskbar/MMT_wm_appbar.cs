 /MMT_Main
function wParam lParam

MMTVAR- v

sel wParam
	case ABN_POSCHANGED MMT_SetPos
	case ABN_FULLSCREENAPP
	ShowWindow v.hwnd iif(lParam SW_HIDE SW_SHOWNOACTIVATE)
	 problem: on Win7 does not receive ABN_FULLSCREENAPP when full screen mode ends. Receives only after certain events. On XP ok.
