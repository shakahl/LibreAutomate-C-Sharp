\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog137" &Dialog137 0)) ret
out "dialog ok"

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Button 0x54032000 0x0 0 0 48 14 "Button"
 4 Button 0x54032000 0x0 0 22 48 14 "mes"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030606 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3 goto gw
	case 4 mes 1
ret 1
 gw
opt waitmsg 1

int ht=mac("Function266")
out wait(0 H ht)

 ARRAY(__Handle) a
 rep 2
	 a[]=run("notepad")
 out wait(0 HMA a)

out "ok"
