int hwnd=val(_command)
int htb=mac("Toolbar34" win("Program Manager" "Progman")) ;;create toolbar attached to desktop
SetProp htb "ho" hwnd
TB_AltAttach htb hwnd
