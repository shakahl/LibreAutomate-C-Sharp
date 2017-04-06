\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3
if(!ShowDialog("" &dlg_Grid_readonly_button &controls)) ret
out qmg3

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x0 0 0 224 108 "0,0,0,2[]A,,16[]B,,23[]"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 __Font-- f.Create("Courier New" 20)
	 f.SetDialogFont(hDlg "3")
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 messages3
NMHDR* nh=+lParam
 if(nh.code!=NM_CUSTOMDRAW) OutWinMsg message wParam lParam
if(nh.idFrom=3)
	 ret DT_Ret(hDlg gridNotify(nh))

	GRID.QM_NMLVDATA* cd=+nh
	sel nh.code
		case GRID.LVN_QG_BUTTONCLICK:
			out "button: item=%i subitem=%i text=%s" cd.item cd.subitem cd.txt
			MenuPopup m.AddItems("one[]two[]three" 1)
			int i=m.Show(nh.hwndFrom 0 2)
			if(i)
				str s; m.GetItemText(i s)
				s.setwintext(cd.hctrl)
