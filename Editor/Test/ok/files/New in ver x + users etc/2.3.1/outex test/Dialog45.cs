\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

act outex_create_window

 SetCurDir "$desktop$"
 act outex_create_window(1)

 int w1=outex_create_window
 _s="rrrrrro"; _s.setwintext(w1)
 act w1

if(!ShowDialog("Dialog45" &Dialog45)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x203000D "*" "" ""

ret
 messages
OutWinMsg message wParam lParam &_s
outex _s
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
