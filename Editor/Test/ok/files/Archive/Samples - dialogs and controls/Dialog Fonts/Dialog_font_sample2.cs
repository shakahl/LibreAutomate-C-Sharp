\Dialog_Editor

 Shows how to use FontDialog and __Font.CreateFromString.

function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4"
str e4
e4="AaBbYyZz"
if(!ShowDialog("Dialog_font_sample2" &Dialog_font_sample2 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 135 "Dialog Fonts2"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 Button 0x54032000 0x0 6 116 56 14 "Change Font"
 4 Edit 0x54030080 0x200 6 58 212 46 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020104 "" "" ""

ret
 messages
str s
__Font-- f

sel message
	case WM_INITDIALOG
	if(rget(s "font" "\Test")) goto g1
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 3
	rget s "font" "\Test"
	if(!FontDialog(s)) ret
	rset s "font" "\Test"
	 g1
	f.CreateFromString(s)
	f.SetDialogFont(hDlg "4")
	
	case IDOK
	case IDCANCEL
ret 1
