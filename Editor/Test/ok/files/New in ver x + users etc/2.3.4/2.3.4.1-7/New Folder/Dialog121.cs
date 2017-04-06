\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
out
if(!ShowDialog("" &Dialog121 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog" "0"
 3 Button 0x54032000 0x0 12 20 48 14 "Button 1"
 4 Button 0x54032000 0x0 12 40 48 14 "Button 2" "test tip 2"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030406 "*" "" "" ""

ret
 messages
__Tooltip- tt
sel message
	case WM_INITDIALOG
	
	tt.Create(hDlg 0)
	tt.AddControl(3 "test tip 1")
	
	case WM_SETCURSOR
	tt.OnWmSetcursor(wParam lParam)
	
	case WM_DESTROY tt.Destroy
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
