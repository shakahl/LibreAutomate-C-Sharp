
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 Static 0x54000003 0x0 0 0 16 16 ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3"
str si3
 si3="$system$\shell32.dll,3" ;;16x16
 si3="&$system$\shell32.dll,3" ;;32x32
 cannot set other icon size here
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	__Hicon- t_hi1=GetFileIcon("$system$\shell32.dll,3" 0 MakeInt(0 48))
	SendDlgItemMessage hDlg 3 STM_SETICON t_hi1 0
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
