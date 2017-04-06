\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages
int+ deb_counter=0; out
deb
 deb-
 deb 100
 deb 0
if(!ShowDialog("dlg_deb" &dlg_deb)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030100 "" "" ""

ret
 messages
 deb
 deb_counter+1; if(deb_counter>10) end
 OutWinMsg message wParam lParam &_s
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2

sel wParam
	case IDOK spe
	case IDCANCEL
ret 1

 29  1405  36  594499121  -1  -1  -1  
 83  126  44  594489372  -1  -1  -1  
 119  146  44  594457612  -1  -1  -1  
 119  132  51  594446249  -1  -1  -1  
 119  125  44  594437380  -1  -1  -1  
 107  132  43  594426738  -1  -1  -1  
 111  130  44  594415952  -1  -1  -1  
 112  136  48  594403750  -1  -1  -1  
 111  132  45  594391272  -1  -1  -1  
 285  118  41  594380562  -1  -1  -1  
 727  393  91  594363366  -1  -1  -1  
 113  125  67  594340684  -1  -1  -1  
