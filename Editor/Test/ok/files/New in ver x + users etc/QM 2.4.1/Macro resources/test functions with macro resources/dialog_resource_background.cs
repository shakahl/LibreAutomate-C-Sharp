 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dialog_resource_background 0 0 0 0 0 0 0 0 "resource:output.ico")) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "" "" "" ""

 messages
sel message
	case WM_INITDIALOG
	DT_SetBackgroundImage hDlg "resource:<>image:ąč ﯔﮥ k"
	
	case WM_COMMAND goto messages2
ret
 messages2
ret 1

 BEGIN PROJECT
 main_function  dialog_resource_background
 exe_file  $my qm$\dialog_resource_background.qmm
 flags  23
 guid  {336FA072-70A1-422F-8137-3D8E762A6D56}
 END PROJECT
