\Dialog_Editor
 /exe
function# hDlg message wParam lParam
if(hDlg) goto messages

ref GRID

if(!ShowDialog("" &dlg_QM_Grid7 0)) ret

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
	 add columns
	LvAddCol g 0 "col" 80
	
	TO_LvAdd g 0 0 0 "s1"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	 get and show all cells
	ICsv c=CreateCsv(1)
	c.FromQmGrid(id(3 hDlg))
	c.ToString(_s); ShowText "" _s
	
	case 4
	out SendMessage(id(3 hDlg), LVM_GETITEMCOUNT, 0, 0)
ret 1
