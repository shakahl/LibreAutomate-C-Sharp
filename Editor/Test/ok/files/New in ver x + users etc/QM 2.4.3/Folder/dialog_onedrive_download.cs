 /exe 4
 \Dialog_Editor

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 562 374 "Dialog"
 4 Button 0x54032000 0x0 0 0 48 14 "Download"
 3 ActiveX 0x54030000 0x0 0 16 562 358 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

str controls = "3"
str ax3SHD
ax3SHD="https://onedrive.live.com"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dialog_onedrive_download
 exe_file  $my qm$\dialog_onedrive_download.qmm
 flags  6
 guid  {AF2E9BAA-89D5-4506-B6D0-9F069182CC6D}
 END PROJECT
