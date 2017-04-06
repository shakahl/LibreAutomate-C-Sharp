\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 Shows how to set fonts and colors.
GdiObject- CustomColor =CreateSolidBrush(ColorFromRGB(70 130 180)) 

str controls = "6 7 8 9"
str e6 e7 cb8 c9Che
e6="aaaaaaaaaaaaa bbbbbbbbbbbbbbb"
e7=e6
cb8="&aaaaaaaaaaaaaa[]bbbbbbbbbbbbbbbbb"
if(!ShowDialog("Dialog_font_sample4" &Dialog_font_sample4 &controls)) ret

 BEGIN DIALOG
 0 "" 0x10CA0A40 0x100 0 0 173 99 "Dialog Fonts"
 4 Static 0x54000000 0x4 6 6 48 14 "Text"
 3 Static 0x54000000 0x0 60 6 42 12 "Text"
 5 Static 0x54000000 0x0 112 6 48 12 "Text"
 6 Edit 0x54030080 0x200 6 22 96 14 ""
 7 Edit 0x54230844 0x20000 6 38 96 12 ""
 8 ComboBox 0x54230243 0x0 6 54 96 213 ""
 9 Button 0x54012003 0x0 112 30 48 12 "Check"
 10 Button 0x54020007 0x0 114 54 44 32 "Ssss"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "*" "" ""
 1 Button 0x54030001 0x4 4 82 48 14 "OK"
 2 Button 0x54030000 0x4 56 82 48 14 "Cancel"
 11 QM_Splitter 0x54030000 0x0 104 6 6 62 ""

ret
 messages
sel message
	case WM_INITDIALOG
	SetWindowTheme id(9 hDlg) L"" L""
	__Font-- f
	f.Create("Courier New" 14 1)
	f.SetDialogFont(hDlg "3 4 5")
	f.SetDialogFontColor(hDlg 0xff "3 4 5 6 7 8 9 10")

	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_CTLCOLORDLG ret CustomColor ;;and this 
	 case WM_CTLCOLORSTATIC
	 SetBkMode wParam TRANSPARENT
	 SetTextColor wParam 0xff ;;red text
	 ret GetStockObject(NULL_BRUSH) ;;transparent brush
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1