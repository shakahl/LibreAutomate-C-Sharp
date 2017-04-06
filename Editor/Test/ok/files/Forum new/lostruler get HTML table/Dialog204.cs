 /exe

 WebBrowserControlDisableIE7Emulation

str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 754 404 "Dialog"
 3 ActiveX 0x54030000 0x0 0 16 754 388 "SHDocVw.WebBrowser {8856F961-340A-11D0-A96B-00C04FD705A2}"
 4 Button 0x54032000 0x0 0 0 50 14 "QM forum"
 5 Button 0x54032000 0x0 56 0 48 14 "IMEA"
 END DIALOG
 DIALOG EDITOR: "" 0x2040400 "*" "" "" ""

str controls = "3"
str ax3SHD
ax3SHD="http://www.imea.com.br/imea-site/indicador-milho"
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
SHDocVw.WebBrowser we3
we3._getcontrol(id(3 hDlg))
sel wParam
	case 4
	we3.Navigate("http://www.quickmacros.com/forum")
	case 5
	we3.Navigate("http://www.imea.com.br/imea-site/indicador-milho")
ret 1

 BEGIN PROJECT
 main_function  Dialog204
 exe_file  $my qm$\Dialog204.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 flags  6
 guid  {277BAF27-47F8-41D5-B906-EF8079929F6A}
 END PROJECT
