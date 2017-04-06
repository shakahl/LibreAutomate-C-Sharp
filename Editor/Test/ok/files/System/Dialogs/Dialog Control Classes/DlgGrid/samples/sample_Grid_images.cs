\Dialog_Editor

 Shows how to add icons, overlay, state icons, indent, and set control type of a row or cell.
 Also shows how to work with check boxes.
 Columns and styles are defined in dialog editor.

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 281 159 "QM_Grid"
 3 QM_Grid 0x56031041 0x200 0 0 206 134 "0x7,0,0,0[]A,50%,,[]B,20%,,[]C,20%,1,[]"
 4 QM_Grid 0x56031041 0x200 216 28 64 78 "0x37,0,0,4,0x10008000[]A,,,"
 1 Button 0x54030001 0x4 2 142 48 14 "OK"
 2 Button 0x54030000 0x4 52 142 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040301 "*" "" "" ""

str controls = "3 4"
str qmg3x qmg4x

 Set data for the first grid control.
 The <...> set row properties. More info in QM Help.
qmg3x=
 <//2>simple,q,z
 <7//4>read-only,w,x
 <//3/2>overlay,e,c
 <//5//1>state,r,v
 <//6///1>indent,t,b
 <//7////..8>cell types,y,n
 <//-1>no icon,u,m

 Set data for the second grid control.
qmg4x=
 one
 two
 <////2>checked

if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret

out "--- grid 1 ---"
out qmg3x
out "--- checked items in grid 2 ---"
out qmg4x


#sub DlgProc
function# hDlg message wParam lParam

DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	 set imagelist for the first grid control
	__ImageList- il.Load("$qm$\il_dlg.bmp")
	il.SetOverlayImages("0 1")
	g.SetImagelist(il il)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
ret 1

 Shows how to receive check box notifications from the second grid control.
 It is not necessary, but if you need it.
 Don't forget to add the 'case WM_NOTIFY goto messages3' line.

 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 4
	sel nh.code
		case LVN_ITEMCHANGED
		int row isChecked
		if(g.RowIsCheckNotification(lParam row isChecked)) out "%schecked %i" iif(isChecked "" "un") row
