\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_x_parent" &dlg_x_parent 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Parent dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 4 30 214 34 ""
 4 Button 0x54032000 0x0 4 6 74 14 "Open child dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_APP+10 ;;received info from child dialog
	str& info=+lParam
	str s.format("Received info from child dialog %i:[][]%s" wParam info)
	s.setwintext(id(3 hDlg))
ret
 messages2
sel wParam
	case IDOK
	case 4 ;;Open child dialog
	if(!ShowDialog("dlg_x_child" &dlg_x_child 0 hDlg)) ret
ret 1
