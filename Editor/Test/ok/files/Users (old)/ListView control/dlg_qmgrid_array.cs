\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("dlg_qmgrid_array" &dlg_qmgrid_array)) ret

 BEGIN DIALOG
 0 "" 0x10C80A44 0x100 0 0 223 135 "Form"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x54000001 0x200 4 4 216 106 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2010800 "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	fill_listview_sample id(3 hDlg)
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
 messages3
 NMHDR& nh=+lParam
 sel nh.code
	 case LVN_COLUMNCLICK
