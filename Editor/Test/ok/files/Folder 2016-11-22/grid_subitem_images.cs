
str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 281 159 "QM_Grid"
 3 QM_Grid 0x56031041 0x200 0 0 206 134 "0x16,0,0,0x0,0x0[]A,50%,,[]B,20%,,[]C,20%,,"
 1 Button 0x54030001 0x4 2 142 48 14 "OK"
 2 Button 0x54030000 0x4 52 142 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3"
str qmg3x

 Set data for the grid control.
qmg3x=
 <//2>row 0,q,z
 <//4>row 1,w,x

if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret


#sub DlgProc
function# hDlg message wParam lParam

DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	 set imagelist for the grid control
	__ImageList- il.Load("$qm$\il_dlg.bmp")
	g.SetImagelist(il il)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
ret 1

 Shows how to set images for non-first-column cells.

 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	sel nh.code
		case LVN_GETDISPINFOW
		NMLVDISPINFOW& di=+nh
		LVITEMW& u=di.item
		 out F"{u.iItem} {u.iSubItem}"
		if u.iSubItem
			u.mask|LVIF_IMAGE
			u.iImage=6
