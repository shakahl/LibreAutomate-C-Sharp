\Dialog_Editor

 Shows differences between two macros.
 Line colors: added green, deleted red, modified yellow.
 Can be easily extended to work with any scintilla or scintilla-based control.
 QM code controls are based on scintilla control.

 Requires Xdiff: http://www.quickmacros.com/forum/viewtopic.php?f=2&t=4379


function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "4 6"
str cb4 cb6
cb4="Primary code editor[]Secondary code editor[]QM file viewer"
cb6=cb4
if(!ShowDialog("" &Compare2Macros &controls _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 130 66 "Compare 2 Macros"
 3 Static 0x54000000 0x0 4 6 22 12 "Old"
 4 ComboBox 0x54230243 0x0 30 4 96 213 ""
 5 Static 0x54000000 0x0 4 24 22 12 "New"
 6 ComboBox 0x54230243 0x0 30 22 96 213 ""
 7 Button 0x54032000 0x0 14 48 48 14 "Compare"
 8 Button 0x54032000 0x0 68 48 48 14 "Clear"
 END DIALOG
 DIALOG EDITOR: "" 0x2030205 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	CB_SelectItem id(4 hDlg) 1
	CB_SelectItem id(6 hDlg) 0
	case WM_COMMAND goto messages2
ret
 messages2
int action
sel wParam
	case 7 action=1; goto compare
	case 8 goto compare
ret 1

 compare
C2M_Compare action C2M_GetHwnd(hDlg 4) C2M_GetHwnd(hDlg 6)
