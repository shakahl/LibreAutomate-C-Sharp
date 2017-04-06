int hwnd=val(_command)
int htb=win("TOOLBAR34" "QM_toolbar")
if !htb
	htb=mac("Toolbar34" win("Program Manager" "Progman")) ;;create toolbar attached to desktop
	SetProp htb "ho" hwnd
TB_AltAttach htb hwnd
