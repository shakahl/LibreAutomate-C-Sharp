\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 if(!ShowDialog("dlg_menu_draw" &dlg_menu_draw 0 0 0 0 0 0 0 0 0 "dlg_menu_draw")) ret
if(!ShowDialog("dlg_menu_draw" &dlg_menu_draw)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x203000C "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	out
	DT_SetMenu hDlg DT_CreateMenu("dlg_menu_draw")
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_MEASUREITEM
	MEASUREITEMSTRUCT& mi=+lParam
	 out 1
	out "%i %i 0x%X %i" mi.CtlID mi.CtlType mi.itemData mi.itemID
	mi.itemWidth=50
	case WM_DRAWITEM
	DRAWITEMSTRUCT& di=+lParam
	 out 2
	 zRECT di.rcItem
	 out "%i %i %i %i" di.itemAction di.itemData di.itemID di.itemState
	 out "%i %i %i %i" di.CtlID di.CtlType di.hDC di.hWndItem
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN MENU
 >&File : -1 0x100
	 &New : 100 0x100 0 Cn
	 &Open : 101 0x100 0 Co
	 &Save : 102 0x100 0 Cs
	 Save &As... : 103 0x100
	 -
	 >&Recent : -101 0x100
		 not implemented : 190 0 1
		 <
	 -
	 E&xit : 2 0x100 0 AF4
	 <
 >&Edit : -2 0x100
	 x : 200 0x100
	 <
 >&Help : -4 0x100
	 x : 200 0x100
	 <
 END MENU
