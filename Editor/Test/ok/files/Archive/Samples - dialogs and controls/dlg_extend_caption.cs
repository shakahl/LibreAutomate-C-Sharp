\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_extend_caption" &dlg_extend_caption 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 143 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030006 "*" "" ""

ret
 messages

type MARGINS cxLeftWidth cxRightWidth cyTopHeight cyBottomHeight
dll- dwmapi #DwmExtendFrameIntoClientArea hWnd MARGINS*pMarInset
dll- dwmapi #DwmIsCompositionEnabled *pfEnabled
def WM_DWMCOMPOSITIONCHANGED 0x031E

sel message
	case WM_INITDIALOG
	 g1
	if(_winnt>=6 and !DwmIsCompositionEnabled(&_i) and _i)
		MARGINS m.cyTopHeight=50
		DwmExtendFrameIntoClientArea hDlg &m
	
	case WM_ERASEBKGND
	if(_winnt>=6 and !DwmIsCompositionEnabled(&_i) and _i)
		RECT r; GetClientRect hDlg &r
		FillRect wParam &r COLOR_BTNFACE+1
		r.bottom=50
		FillRect wParam &r GetStockObject(BLACK_BRUSH)
		ret DT_Ret(hDlg 1)
	
	case WM_DWMCOMPOSITIONCHANGED goto g1
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

