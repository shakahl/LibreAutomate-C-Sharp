\Dialog_Editor

 Shows how to draw text directly in the dialog.

function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("Dialog_font_sample3" &Dialog_font_sample3)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog Fonts3"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 6 116 56 14 "Change Font"
 END DIALOG
 DIALOG EDITOR: "" 0x2020104 "" "" ""

ret
 messages
str s
__Font-- f
__Font-- fVert

sel message
	case WM_INITDIALOG
	if(rget(s "font" "\Test")) goto g1
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_PAINT
	PAINTSTRUCT p
	int dc=BeginPaint(hDlg &p)
	int oldfont=SelectObject(dc f)
	SetBkMode dc TRANSPARENT
	SetTextColor dc 0x008000
	
	str text=iif(_unicode "AaBb ąč 古印" "AaBb [193]")
	
	RECT r; r.left=30; r.top=10; r.right=300; r.bottom=90
	DrawTextW dc @text -1 &r DT_NOPREFIX
	
	SelectObject(dc fVert)
	r.left=0; r.bottom=0; r.right=20; r.top=150
	DrawTextW dc @text -1 &r DT_NOPREFIX
	
	SelectObject dc oldfont
	EndPaint hDlg &p
ret
 messages2
sel wParam
	case 3
	rget s "font" "\Test"
	if(!FontDialog(s)) ret
	rset s "font" "\Test"
	RedrawWindow hDlg 0 0 RDW_INVALIDATE|RDW_ERASE
	 g1
	f.CreateFromString(s)
	fVert.Create("" 10 2 90 f 2|4|8) ;;create smaller italic vertical font from f
	
	case IDOK
	case IDCANCEL
ret 1
