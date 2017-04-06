\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str e3
if(!ShowDialog("dlg_extend_caption3" &dlg_extend_caption3 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 143 "Dialog"
 3 Edit 0x54030080 0x200 94 4 96 14 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030006 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 int bcol=ColorFromRGB(200 201 202)
	int bcol=0
	 SetWinStyle hDlg WS_EX_LAYERED 5
	 SetLayeredWindowAttributes hDlg bcol 0 LWA_COLORKEY
	__GdiHandle-- hb=CreateSolidBrush(bcol)
	
	WINAPI2.MARGINS m.cyTopHeight=50
	WINAPI2.DwmExtendFrameIntoClientArea hDlg &m
	
	 __GdiHandle hr=CreateRectRgn(30 30 100 100)
	 WINAPI2.DWM_BLURBEHIND bb
	 bb.dwFlags=WINAPI2.DWM_BB_ENABLE|WINAPI2.DWM_BB_BLURREGION
	 bb.fEnable=1
	 bb.hRgnBlur=hr
	 WINAPI2.DwmEnableBlurBehindWindow hDlg &bb
	
	case WM_ERASEBKGND
	RECT r; GetClientRect hDlg &r
	FillRect wParam &r COLOR_BTNFACE+1
	r.bottom=50
	FillRect wParam &r GetStockObject(BLACK_BRUSH)
	ret DT_Ret(hDlg 1)
	
	 enable drag
	 case WM_NCHITTEST
	 if(GetWinId(child(mouse))=999) ret DT_Ret(hDlg HTCAPTION)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

