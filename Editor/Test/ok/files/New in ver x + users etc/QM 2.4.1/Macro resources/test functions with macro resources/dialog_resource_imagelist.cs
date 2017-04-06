 /exe
\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

__ImageList- t_a=__ImageListLoad(":5 $qm$\il_qm.bmp")
__ImageList- t_b=__ImageListLoad("resource:il_de.bmp")
__ImageList- t_c=__ImageListLoad("image:il_dlg")

if(!ShowDialog("" &dialog_resource_imagelist)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 ToolbarWindow32 0x54010808 0x0 0 0 222 20 ""
 4 ToolbarWindow32 0x54010808 0x0 0 30 222 20 ""
 5 ToolbarWindow32 0x54010808 0x0 0 60 222 20 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040100 "" "" "" ""

 messages
sel message
	case WM_INITDIALOG
	DSI_InitToolbar hDlg 3 t_a
	DSI_InitToolbar hDlg 4 t_b
	DSI_InitToolbar hDlg 5 t_c
	
	case WM_COMMAND goto messages2
ret
 messages2
ret 1

 BEGIN PROJECT
 main_function  dialog_resource_imagelist
 exe_file  $my qm$\dialog_resource_imagelist.qmm
 flags  23
 guid  {336FA072-70A1-422F-8137-3D8E762A6D56}
 END PROJECT
