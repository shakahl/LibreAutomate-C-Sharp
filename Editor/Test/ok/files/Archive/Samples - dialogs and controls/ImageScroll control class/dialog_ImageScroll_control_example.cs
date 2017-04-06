\Dialog_Editor

InitWinClass_ImageScroll

str dd=
 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 224 136 "Dialog"
 3 QM_ImageScroll 0x54000000 0x20000 72 8 144 120 ""
 4 Button 0x54032000 0x0 8 8 48 14 "Browse..."
 5 Button 0x54032000 0x0 8 28 48 14 "Capture"
 1 Button 0x54030001 0x4 8 92 48 14 "OK"
 2 Button 0x54030000 0x4 8 112 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" "3"

str controls = "3"
str qmis3
qmis3="C:\Program Files (x86)\Audacity\help\manual\m\images\1\19\mixer_board.png"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	DT_SetAutoSizeControls hDlg "3s"
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 4 ;;Browse
	if(!OpenSaveDialog(0 _s "bmp, png, jpg, gif[]*.bmp;*.png;*.jpg;*.gif[]")) ret
	_s.setwintext(id(3 hDlg))
	
	case 5 ;;Capture
	if(!CaptureImageOrColor(_i 0 hDlg)) ret
	_s=_i
	_s.setwintext(id(3 hDlg))
	
	case IDOK
	case IDCANCEL
ret 1
