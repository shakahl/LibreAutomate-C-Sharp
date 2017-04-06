\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "3"
str qmg3x
qmg3x=
 one,"two
 oooooooooooooooooooooooooooooooooooooooooo"
 threeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee,"two wwwwwwwwwwwwwwwwwwwwwwwwwwwww
 mmmmmmmmmmmmm"

if(!ShowDialog("" &dlg_Grid_custombutton &controls _hwndqm)) ret
out qmg3x

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 527 135 "Dialog"
 3 QM_Grid 0x56031041 0x0 0 0 528 110 "[]edit,,16,[]edit ml,,8,[]combo,,17,[]check,,2,[]date,,3,[]time,,11,[]none,,7,"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 4 Button 0x54032000 0x0 282 116 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030203 "*" "" ""
 3 QM_Grid 0x56031041 0x0 0 0 528 110 "0x20,0,0,2,0x8000[]edit,,16,[]edit ml,,8,[]combo,,17,[]check,,2,[]date,,3,[]time,,11,[]none,,7,"

ret
 messages
DlgGrid g.Init(hDlg 3)
 __Font-- f.Create("Arial" 12)
sel message
	case WM_INITDIALOG
	 SetProp(g "sub" SubclassWindow(g &WndProc23))
	 SetProp(hDlg "sub" SubclassWindow(hDlg &WndProc23))
	 g.GridStyleSet(LVS_EX_SUBITEMIMAGES 5)
	 outx g.Send(LVM_GETEXTENDEDLISTVIEWSTYLE)
	 f.SetDialogFont(hDlg)
	 SetProp(g "sub" SubclassWindow(g &WndProc23))
	__ImageList- t_imagelist
	 t_imagelist.Load(":3383 $qm$\il_test32.bmp")
	t_imagelist.Load(":3383 $qm$\il_qm.bmp")
	 t_imagelist=ImageList_Create(1 16 0 0 0)
	 out t_imagelist
	 g.SetImagelist(t_imagelist)
	 g.SetImagelist(0 t_imagelist)
	
	g.RowPropSet(1 16 0 0 0 0 4)
	
	g.GridStyleSet(LVS_EX_CHECKBOXES 5)
	ret
	
	 int il=g.Send(LVM_GETIMAGELIST LVSIL_SMALL)
	g.Send(LVM_SETIMAGELIST LVSIL_SMALL 0)
	g.GridStyleSet(LVS_EX_CHECKBOXES 5)
	 g.Send(LVM_SETIMAGELIST LVSIL_SMALL il)
	
	g.RowsDeleteAll
	g.FromCsv("a,b[]c,d" ",")
	g.RowPropSet(1 16 0 0 0 0 20)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	case 4
	 SetProp(hDlg "sub" SubclassWindow(hDlg &WndProc23))
	 int gnew=CreateControl(0 "QM_Grid" "0[]aaaa" 0 0 0 100 100 hDlg 8)
	 outw gnew
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	GRID.QM_NMLVDATA* cd=+nh
	NMLVDISPINFO* di=+nh
	NMLISTVIEW* nlv=+nh
	NMITEMACTIVATE* na=+nh
	sel nh.code
		case LVN_BEGINLABELEDIT
		if(di.item.iSubItem) ret
		 int hb=child("" "Button" nh.hwndFrom); if(!hb) ret
		  siz 80 0 hb 2
		  SetWindowText hb " Browse..."
		 
		 SetWinStyle hb BS_ICON 1 ;;on XP does not work if without BS_ICON style. But if with this style, does not display text.
		 __Hicon-- hi=GetFileIcon("browse.ico")
		 SendMessage hb BM_SETIMAGE IMAGE_ICON hi
		
		 g.SetButtonProp(hi "text ąčę[]line2")
		g.SetButtonProp(1 "Browse...")
		
		case GRID.LVN_QG_BUTTONCLICK ;;when user clicks button
		out "button: item=%i subitem=%i text=%s" cd.item cd.subitem cd.txt
		if(OpenSaveDialog(0 _s))
			_s.setwintext(cd.hctrl)
