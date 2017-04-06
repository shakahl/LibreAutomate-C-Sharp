\Dialog_Editor
function# hDlg message wParam lParam

 BEGIN DIALOG
 0 "" 0x90000048 0x80000A8 0 0 227 151 "OSD_Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2020100 "" ""

OSDA* d
sel message
	case WM_INITDIALOG
	d=+DT_GetParam(hDlg)
	if(d.cx and d.cy) siz d.cx d.cy hDlg
	Transparent hDlg iif(d.transp d.transp 255) GetSysColor(COLOR_BTNFACE)
	
	case WM_DESTROY
	d=+DT_GetParam(hDlg); d._delete
	
	case WM_PAINT
	 begin paint
	PAINTSTRUCT p
	int hdc=BeginPaint(hDlg &p)
	
	 set transparent brush
	int oldbrush=SelectObject(hdc GetStockObject(NULL_BRUSH))
	
	 call user-defined draw function
	d=+DT_GetParam(hDlg)
	RECT rc; GetClientRect hDlg &rc
	call d.fa hDlg hdc rc.right rc.bottom d.param
	
	SelectObject(hdc oldbrush)
	
	 end paint
	EndPaint hDlg &p
	ret 1
	
	 case WM_COMMAND ret 1
	