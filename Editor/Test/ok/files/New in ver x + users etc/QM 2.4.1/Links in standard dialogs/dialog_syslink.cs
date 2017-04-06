/exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 str controls = "3"
 str sl3Tex
 sl3Tex=F"Test <a id=''200''>id 200</a>[]Test <a href=''{&syslink_click} param''>callback</a>"
 if(!ShowDialog("dialog_syslink" &dialog_syslink &controls)) ret
if(!ShowDialog("dialog_syslink" &dialog_syslink)) ret

 BEGIN DIALOG
 1 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 SysLink 0x54020000 0x0 8 6 96 48 "Test <a id=''200''>id 200</a>"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040101 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 _s="Text <a href=''www.quickmacros.com''>quickmacros</a>[]Line2 <a id=''http://www.google.com''>google</a>"; _s.setwintext(id(3 hDlg))
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dialog_syslink
 exe_file  $my qm$\dialog_syslink.exe
 icon  <default>
 manifest  
 flags  6
 guid  {FC5F743C-BEC7-42B7-BAAD-AA037AC3D606}
 END PROJECT
 manifest  $qm$\default.exe.manifest
