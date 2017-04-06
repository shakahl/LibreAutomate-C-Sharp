 /exe
\Dialog_Editor

str controls = "3"
str ax3SHD
ax3SHD="http://www.quickmacros.com"
if(!ShowDialog("" &sub.DialogProcedure &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 ActiveX 0x54030000 0x0 0 0 224 114 "SHDocVw.WebBrowser"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040104 "*" "" "" ""

#sub DialogProcedure
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	SHDocVw.WebBrowser we3
	we3._getcontrol(id(3 hDlg))
	we3._setevents("sub.we3")
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub we3_DocumentComplete v
function IDispatch'pDisp `&URL ;;SHDocVw.IWebBrowser2'we3
out URL
out ax3SHD

 BEGIN PROJECT
 main_function  Dialog147
 exe_file  $my qm$\Dialog147.qmm
 flags  6
 guid  {45B853F9-AB6C-4D09-AB22-9A8CC24005CF}
 END PROJECT
