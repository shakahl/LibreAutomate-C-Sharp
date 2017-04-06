\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dlg_SysListView32)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 SysListView32 0x54030001 0x0 0 0 166 90 ""
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030203 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int h=id(3 hDlg)
	int lvexs=LVS_EX_GRIDLINES|LVS_EX_FULLROWSELECT|LVS_EX_INFOTIP
	SendMessage h LVM_SETEXTENDEDLISTVIEWSTYLE lvexs lvexs
	TO_LvAddCol h 0 "col" -50
	TO_LvAddCol h 1 "col" -50
	TO_LvAdd h 0 0 0 "one wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww" "two wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww"
	TO_LvAdd h 1 0 0 "two"
	
	 SetProp(h "sub" SubclassWindow(h &WndProc23))
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1
