\Dialog_Editor
 /exe
function# hDlg message wParam lParam
if(hDlg) goto messages

ref GRID

if(!ShowDialog("" &dlg_QM_Grid6 0)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 291 175 "QM_Grid"
 3 QM_Grid 0x54210001 0x200 0 0 294 154 ""
 1 Button 0x54030001 0x4 98 158 48 14 "OK"
 2 Button 0x54030000 0x4 148 158 48 14 "Cancel"
 4 Button 0x54032000 0x0 236 158 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""
 3 SysListView32 0x54200001 0x200 0 0 294 154 ""

ret
 messages
sel message
	case WM_INITDIALOG
	int g
	g=id(3 hDlg)
	 use first column as noneditable
	SendMessage g LVM_QG_SETSTYLE QG_NOEDITFIRSTCOLUMN -1
	 add columns
	LvAddCol g 0 "read-only" 80
	LvAddCol g 1 "edit" 50
	LvAddCol g 2 "edit+button" 80
	 set column cell control default types
	SendMessage g LVM_QG_SETCOLUMNTYPE 2 QG_EDIT|QG_BUTTONATRIGHT
	
	TO_LvAdd g 0 0 0 "s1" "" "1"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case 4
	 g=id(3 hDlg)
ret 1
 messages3
 NMHDR* nh=+lParam
 if(nh.idFrom=3)
	 NMLVDISPINFO* di=+nh
	  OutWinMsg(message, wParam, lParam);
	 sel nh.code
		 case LVN_GETDISPINFOW
		 if(di.item.mask&LVIF_IMAGE) di.item.iImage=di.item.iItem
		  outx di.item.mask
	 
	  ret DT_Ret(hDlg gridNotify(nh))
