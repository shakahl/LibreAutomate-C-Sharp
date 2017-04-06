\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str- sItems=
 January
 February
 March
 April
 May

str controls = "3 4"
str e3 lb4
if(!ShowDialog("dlg_list_filtering" &dlg_list_filtering &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 3 Edit 0x54030080 0x200 0 0 96 14 ""
 4 ListBox 0x54230101 0x200 0 16 96 102 ""
 5 Button 0x54032000 0x0 110 16 110 34 "Select an item in the list and click me; or double click an item"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030304 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetTimer hDlg 1 300 0
	int he=id(3 hDlg)
#ifdef MyEditSubclassProc
	SetProp he "wndproc" SubclassWindow(he &MyEditSubclassProc)
#endif
	
	case WM_DESTROY
	RemoveProp he "wndproc"
	
	case WM_COMMAND goto messages2
	case WM_TIMER
	sel wParam
		case 1
		KillTimer hDlg wParam
		int hlb=id(4 hDlg)
		SendMessage hlb LB_RESETCONTENT 0 0
		str s sEdit.getwintext(id(3 hDlg))
		foreach s sItems
			if(sEdit.len and find(s sEdit 0 1)<0) continue
			LB_Add hlb s
ret
 messages2
sel wParam
	case EN_CHANGE<<16|3
	SetTimer hDlg 1 300 0
	
	case LBN_DBLCLK<<16|4
	goto gShowSelected
	
	case 5
	goto gShowSelected
	
	case IDOK
	case IDCANCEL
ret 1

 gShowSelected
hlb=id(4 hDlg)
_i=LB_SelectedItem(hlb)
LB_GetItemText hlb _i s
mes s
