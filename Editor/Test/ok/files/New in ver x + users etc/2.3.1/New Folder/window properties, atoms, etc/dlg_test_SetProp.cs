\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
opt waitmsg 1
rep 10
	hDlg=ShowDialog("dlg_test_SetProp" &dlg_test_SetProp 0 0 1)
	0.01
	clo hDlg
	0.01
	 out IsWindow(hDlg)
	 out GetProp(hDlg "__________")

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Static 0x54000000 0x0 16 14 48 12 "Text"
 END DIALOG
 DIALOG EDITOR: "" 0x2030009 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 out
	int i; str s1 s2
	for(i 0 100000)
		 _s.RandomString(100 100)
		 if(!SetProp(hDlg _s 1)) out _s.dllerror
		if(!SetProp(hDlg "__________" i)) out _s.dllerror
	
	out GetProp(hDlg "__________")
	case WM_DESTROY
	 EnumPropsEx hDlg &PropDelete_EnumProc 0
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
