\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dlg_tab_owner_draw)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 SysTabControl32 0x54030040 0x0 0 0 224 110 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	str-- t_tabLabels="A[]Bbbbbbbbbbbbbb[]C"
	__GdiHandle-- t_tabBrush=CreateSolidBrush(0x80f0c0) ;;color
	 __GdiHandle bm=LoadPictureFile("$qm$\bitmap1.bmp"); __GdiHandle-- t_tabBrush=CreatePatternBrush(bm) ;;image
	
	int htb=id(3 hDlg)
	SetWinStyle htb TCS_OWNERDRAWFIXED 1
	TCITEMW ti.mask=TCIF_TEXT
	foreach _s t_tabLabels
		ti.pszText=@_s; SendMessage htb TCM_INSERTITEMW _i &ti
		_i+1
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_DRAWITEM
	DRAWITEMSTRUCT& d=+lParam
	 out "%i %i %i" d.itemID d.itemAction d.itemState
	
	if(d.itemState&ODS_SELECTED) FillRect d.hDC &d.rcItem t_tabBrush
	
	_s.getl(t_tabLabels d.itemID)
	RECT r=d.rcItem; InflateRect &r -2 -2
	SetBkMode d.hDC TRANSPARENT
	DrawTextW d.hDC @_s -1 &r DT_NOPREFIX
	
	ret DT_Ret(hDlg 1)
	
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
