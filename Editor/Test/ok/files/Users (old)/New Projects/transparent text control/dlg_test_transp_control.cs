\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 3 6 9"
str c4Che qmtc3 e6 cb9
qmtc3="Test ąčę Test ąčę Test ąčę Test ąčę Test ąčę Test ąčę"
if(!ShowDialog("dlg_test_transp_control" &dlg_test_transp_control &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A40 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 4 Button 0x50012003 0x0 14 10 34 12 "Check"
 3 QM_TranspCtrl 0x50000000 0x0 6 6 216 10 ""
 5 Button 0x54032000 0x0 50 8 30 14 "Button"
 6 Edit 0x54030080 0x200 82 10 22 12 ""
 7 Static 0x54000000 0x0 106 10 18 12 "Text"
 9 ComboBox 0x54230243 0x0 152 12 22 213 ""
 8 Button 0x54020007 0x0 126 8 24 16 "aa"
 END DIALOG
 DIALOG EDITOR: "" 0x2030006 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	__Font-- f.Create("Courier New" 14)
	f.SetDialogFont(hDlg "3")
	
	SetTimer hDlg 1 2000 0
	
	case WM_TIMER
	sel wParam
		case 1
		str s.getmacro("dlg_test_transp_control")
		s.getl(s RandomInt(0 numlines(s)-1))
		s.setwintext(id(3 hDlg))
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
