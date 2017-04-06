 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 BEGIN DIALOG
 0 "" 0x10000448 0x200 0 0 217 255 ""
 3 Button 0x54032009 0x0 2 86 72 13 "Option 1 of 3"
 4 Button 0x54002009 0x0 2 158 64 13 "Option 2 of 3"
 5 Button 0x54002009 0x0 2 240 66 13 "Option 3 of 3"
 6 Edit 0x54031044 0x200 176 0 18 254 ""
 7 Button 0x54032000 0x0 124 36 48 14 "Browse..."
 8 Static 0x54000000 0x0 126 68 24 13 "Static"
 9 Edit 0x54030080 0x200 2 36 120 14 ""
 10 Static 0x54000003 0x0 4 4 16 16 ""
 11 Static 0x54000000 0x0 40 6 132 22 "Quick Macros"
 12 Button 0x54012003 0x0 2 54 48 12 "Check"
 END DIALOG
 DIALOG EDITOR: "DCSCONTROLS842" 0x2030003 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 this is not necessary. Just sets font of control 11.
	__Font-- f
	f.Create("Georgia" 16 1)
	f.SetDialogFont(hDlg "11")
	DT_SetTextColor(hDlg 0xff0000 "11")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case 7
	if(OpenSaveDialog(0 _s)) _s.setwintext(id(9 hDlg))
ret 1
