\Dialog_Editor

 Shows files in FTP server "ftp.arm.linux.org.uk".

function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("FtpDirDlg" &FtpDirDlg)) ret

 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 223 202 "Dialog"
 1 Button 0x54030001 0x4 120 186 48 14 "OK"
 2 Button 0x54030000 0x4 172 186 48 14 "Cancel"
 3 SysTreeView32 0x54000000 0x200 0 0 224 182 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020006 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	mac "FD_Thread" "" id(3 hDlg)
	case WM_DESTROY
	shutdown -6 0 "FD_Thread"
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
