 /exe
 \Dialog_Editor
typelib ARGradientControl {94F7E282-F78A-11D1-9587-0000B43369D3} 1.1
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str ax3SHD
if(!ShowDialog("dialog_activex_exe" &dialog_activex_exe 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2030307 "*" "" ""
 3 ActiveX 0x54030000 0x0 0 0 224 100 "SHDocVw.WebBrowser"
 4 ActiveX 0x54030000 0x0 0 104 92 30 "ARGradientControl.ARGradient {94F7E278-F78A-11D1-9587-0000B43369D3}"

#exe addactivex "SHDocVw.WebBrowser"
#exe addactivex "ARGradientControl.ARGradient"

ret
 messages
sel message
	case WM_INITDIALOG
	CreateControl(0 "ActiveX" "SHDocVw.WebBrowser" 0 0 0 300 100 hDlg 3)
	CreateControl(0 "ActiveX" "ARGradientControl.ARGradient {94F7E278-F78A-11D1-9587-0000B43369D3}" 0 0 120 20 20 hDlg 4)
	CreateControl(0 "ActiveX" "ARGradientControl.ARGradient" 0 30 120 20 20 hDlg 4)
	CreateControl(0 "ActiveX" "{94F7E278-F78A-11D1-9587-0000B43369D3}" 0 60 120 20 20 hDlg 4)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dialog_activex_exe
 exe_file  $my qm$\dialog_activex_exe.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  7
 end_hotkey  0
 guid  {0FC6A9A5-D9C1-40B2-96BB-4267687DB3E1}
 END PROJECT
