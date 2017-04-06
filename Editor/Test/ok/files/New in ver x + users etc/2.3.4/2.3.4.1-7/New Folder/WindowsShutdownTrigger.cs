\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &WindowsShutdownTrigger 0 0 128)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_COMMAND ret 1
	case WM_QUERYENDSESSION
	
	mes "WM_QUERYENDSESSION" "WindowsShutdownTrigger" ;;for testing; delete then
	
	opt waitmsg 1
	wait 60 H mac("Macro1"); err
	wait 60 H mac("Macro2"); err
	
