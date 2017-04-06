/exe
\Dialog_Editor

ref HTMLayout "__HTMLayout_API" 1

if(!ShowDialog("" &sub.DlgProc)) ret

 BEGIN DIALOG
 1 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0 "*" "" "" ""
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"

#sub DlgProc r
function# hDlg message wParam lParam

int hlh hlr=HTMLayout.HTMLayoutProcND(hDlg message wParam lParam &hlh)
if(hlh) ret DT_Ret(hDlg hlr)

sel message
	case WM_INITDIALOG
	_s="<html><body>test <b>bold</b> <a href=''http://www.quickmacros.com''>quickmacros.com</a></body></html>"
	HTMLayoutLoadHtml(hDlg _s _s.len)
	
	 _s="Q:\Downloads\HTMLayoutSDK\html_samples\behaviors\treeview.htm"
	 HTMLayoutLoadFile(hDlg @_s)
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1

 BEGIN PROJECT
 main_function  dialog_HTMLayout
 exe_file  $my qm$\dialog_HTMLayout.qmm
 flags  6
 guid  {E9B1D898-0A9E-439F-9410-2791441BF4C3}
 END PROJECT
