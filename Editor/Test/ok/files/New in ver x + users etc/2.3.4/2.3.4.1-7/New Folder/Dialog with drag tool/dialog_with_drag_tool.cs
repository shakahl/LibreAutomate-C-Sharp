\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str si3
si3="&$qm$\target.ico"
if(!ShowDialog("dialog_with_drag_tool" &dialog_with_drag_tool &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 Static 0x54000003 0x0 2 2 16 16 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	
	case WM_LBUTTONDOWN
	if GetWinId(child(mouse))=3 ;;if on Drag icon
		if(!Drag(hDlg &Callback_Drag22 0)) ret
		Acc a.FromMouse; err ret
		out a.Name; err
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
