 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x10C80A48 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Edit 0x54231044 0x200 0 0 96 44 ""
 5 Static 0x54000000 0x0 2 48 218 30 "Scroll using the scrollbar. Will show WM_VSCROLL messages."
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	
	 Subclass Edit control
	int- t_oldwndproc
	t_oldwndproc=SubclassWindow(id(3 hDlg) &WndProcSubclassedEdit)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
