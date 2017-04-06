
str dd=
 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 224 136 "Dialog"
 3 QM_Grid 0x56031041 0x200 0 0 224 110 "0[]A[]B"
 1 Button 0x54030001 0x4 116 116 48 14 "OK"
 2 Button 0x54030000 0x4 168 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

str controls = "3"
str qmg3
if(!ShowDialog(dd &sub.DlgProc &controls)) ret


#sub DlgProc
function# hDlg message wParam lParam

sel message
	case WM_INITDIALOG
	QmRegisterDropTarget(id(3 hDlg) hDlg 16)
	
	case WM_QM_DRAGDROP
	QMDRAGDROPINFO& di=+lParam
	DlgGrid g.Init(di.hwndTarget)
	int iSub i=sub.LvItemFromPoint(g iSub)
	 out F"{i} {iSub}"
	foreach _s di.files
		 out _s
		g.CellSet(i iSub _s)
		if(i>=0) i+1
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
ret 1


#sub LvItemFromPoint
function# hlv int&subItem

 Sends LVM_HITTEST and returns item index of listview item from mouse.

 subItem - receives subitem index.

LVHITTESTINFO ht
xm &ht.pt hlv 1
SendMessage(hlv LVM_SUBITEMHITTEST 0 &ht)
subItem=ht.iSubItem
ret ht.iItem
