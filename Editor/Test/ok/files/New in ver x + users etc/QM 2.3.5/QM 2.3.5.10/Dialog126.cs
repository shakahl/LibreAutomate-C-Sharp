\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3 4"
str e3 cb4
if(!ShowDialog("Dialog126" &Dialog126 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 135 "Dialog"
 5 Edit 0x54030080 0x200 7 1 96 14 ""
 3 Edit 0x54030080 0x200 7 25 96 13 ""
 4 ComboBox 0x54230242 0x0 7 25 96 213 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x203050A "" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int he=id(3 hDlg)
	int hcb=id(4 hDlg)
	RECT r
	GetWindowRect hcb &r; MapWindowPoints 0 hDlg +&r 2; MoveWindow he r.left r.top r.right-r.left r.bottom-r.top 0
	OffsetRect &r -r.left -r.top
	int hr=CreateRectRgnIndirect(&r)
	r.left=r.right-20; InflateRect &r -2 -2
	zRECT r
	int hr2=CreateRectRgnIndirect(&r)
	CombineRgn hr hr hr2 RGN_DIFF
	DeleteObject hr2
	SetWindowRgn he hr 0
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
int h=id(1001 id(4 hDlg))
sel wParam
	 case EN_SETFOCUS<<16|3
	 outw h
	 SendMessage h WM_SETFOCUS 0 0
	 case EN_KILLFOCUS<<16|3
	 SendMessage h WM_KILLFOCUS 0 0
	case IDOK
	case IDCANCEL
ret 1
