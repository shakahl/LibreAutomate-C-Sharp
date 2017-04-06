\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3x
qmg3x=
 one, 5,    2/21/2012, 12:30
 two, 2,    5/10/2000, 2:10
 three, 31, 12/10/2000
 four, 1000

if(!ShowDialog("dialog_grid_sort_date" &dialog_grid_sort_date &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x0 0 0 224 114 "0x0,0,0,0,0x0[]Text,,,[]Number,,,[]Date,,3,[]Time,,11,"
 END DIALOG
 DIALOG EDITOR: "" 0x2030306 "*" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
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
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	NMLISTVIEW* nlv=+nh
	sel nh.code
		case LVN_COLUMNCLICK ;;click header
		DlgGrid g.Init(nh.hwndFrom)
		int sortFlags=4|0x10000; if(nlv.iSubItem=2) sortFlags|128 ;;sort third column as date
		g.Sort(sortFlags nlv.iSubItem)

		 case LVN_COLUMNCLICK ;;click header
		 DlgGrid g.Init(nh.hwndFrom)
		  int sortFlags=4|0x10000; if(nlv.iSubItem=2) sortFlags|128
		  int sortFlags=0x10000; if(nlv.iSubItem=2) sortFlags|128
		 int sortFlags=0x10000; if(nlv.iSubItem>=2) sortFlags|128
		 g.Sort(sortFlags nlv.iSubItem)
		
		 ICsv c=CreateCsv(1)
		 c.FromQmGrid(nh.hwndFrom)
		 c.Sort(4 nlv.iSubItem)
		 c.ToQmGrid(nh.hwndFrom)
		
		 sel nlv.iSubItem
			 case 2 ;;date
			 ICsv c=CreateCsv(1)
			 c.FromQmGrid(nh.hwndFrom)
			 c.ToQmGrid(nh.hwndFrom)
			 
			 case else
			 DlgGrid g.Init(nh.hwndFrom)
			 g.Sort(4|0x10000 nlv.iSubItem)
