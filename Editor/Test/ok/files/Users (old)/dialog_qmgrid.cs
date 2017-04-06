\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dialog_qmgrid)) ret

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
	str s=
 one, two
 three, four

	ICsv c=CreateCsv(1)
	c.FromString(s)
	
	int hlv=id(3 hDlg)
	TO_LvAddCol hlv 0 "col1" 80
	TO_LvAddCol hlv 1 "col2" 80
	
	c.ToQmGrid(hlv 0)
	
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	c=CreateCsv(1) ;;create again because we use local variable, just to simplify grid populating and retrieving data
	c.FromQmGrid(id(3 hDlg) 0)
	int i
	for i 0 c.RowCount
		out c.Cell(i 0)
		out c.Cell(i 1)
	
	case IDCANCEL
ret 1
 messages3
 NMHDR& nh=+lParam
 sel nh.code
	 case LVN_...
	 NMLISTVIEW& nl=+lParam
	 ...
	 
	 case LVN_QG_...
	 QM_NMLVDATA& nq=+lParam
	 ...
