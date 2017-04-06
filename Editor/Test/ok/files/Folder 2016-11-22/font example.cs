
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Static 0x54000000 0x0 8 8 48 12 "Normal"
 4 Static 0x54000000 0x0 8 24 48 12 "Bold"
 5 Button 0x54032000 0x0 64 24 48 14 "Change"
 6 Button 0x54032000 0x0 116 24 48 14 "Restore"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	 create bold font, using dialog font as template
	__Font- t_f.Create("" 0 1 0 1 4)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 5 ;;Change
	int- t_oldFont
	if(t_oldFont=0) t_oldFont=SendDlgItemMessage(hDlg 4 WM_GETFONT 0 0)
	t_f.SetDialogFont(hDlg "4")
	
	case 6 ;;Restore
	if(t_oldFont) SendDlgItemMessage(hDlg 4 WM_SETFONT t_oldFont 1); t_oldFont=0
ret 1
