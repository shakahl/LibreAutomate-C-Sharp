\Dialog_Editor
function# hDlg message wParam lParam
DlgGrid g.Init(hDlg 5)
if(hDlg) goto messages

str dd =
 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 420 284 "test3"
 1001 Button 0x54032000 0x0 152 0 48 14 "Button"
 5 QM_Grid 0x56031041 0x0 4 16 404 256 "0[]A[]B[]C[]D[]E[]F[]G"
 3 SysTabControl32 0x54030040 0x0 0 0 98 12 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "0" "" ""

int x(851) y(199)
str controls = "5"
str qmg5
if(!ShowDialog("" &Function294 &controls 0 0 0 0 x y)) ret
ret
 messages
__MemBmp-- t_mb
__GdiHandle-- t_hb
RECT r
 dialog size option
 DT_AutoSizeControls hDlg message "5s 15m 16m 17m 18ms 25m 26mv"

sel message
	case WM_INITDIALOG
	DT_SetAutoSizeControls hDlg "5s"

	  SetBackground
		 __GdiHandle- t_brush=CreateSolidBrush(0xF6F6F6)
		 DT_SetBackgroundColor hDlg 0 0xF6F6F6
		 DT_SetBackgroundImage
		t_hb=LoadPictureFile("$my qm$\Copy.bmp")
		SendMessage hDlg WM_SIZE 0 0


	 SetDialogFont
		__Font-- BTN_f TAB_f
		BTN_f.Create("굴림" 8 0 0 0)
		BTN_f.SetDialogFont(hDlg "3")

		TAB_f.Create("굴림" 9 0 0 0)
		TAB_f.SetDialogFont(hDlg "5")

	 SetTAB
		int htb=id(3 hDlg)
		TabControlAddTabs htb "test1[]test2[]test3"
		goto g11

	case WM_SIZE
	GetClientRect hDlg &r
	t_mb.Attach(CopyImage(t_hb 0 r.right r.bottom 0)) ;;copy/stretch t_hb and attach to t_mb
	outw id(5 hDlg)
	outw child("" "SysHeader32" id(5 hDlg))
	InvalidateRect child("" "SysHeader32" id(5 hDlg)) 0 1
	InvalidateRect hDlg 0 1
	
	case WM_ERASEBKGND
	GetClientRect hDlg &r
	BitBlt wParam 0 0 r.right r.bottom t_mb.dc 0 0 SRCCOPY
	ret DT_Ret(hDlg 1)
	
	 case WM_PAINT
	 PAINTSTRUCT ps; int dc=BeginPaint(hDlg &ps)
	 GetClientRect hDlg &r
	 BitBlt dc 0 0 r.right r.bottom t_mb.dc 0 0 SRCCOPY
	 EndPaint hDlg &ps

	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_CTLCOLORSTATIC
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
	case 5
	GRID.QM_NMLVDATA* cd=+nh
		NMLVDISPINFO* di=+nh
		NMLISTVIEW* nlv=+nh
sel nh.code
	case TCN_SELCHANGE
	_i=SendMessage(nh.hwndFrom TCM_GETCURSEL 0 0)
	 out "Tab selected: %i" _i

	 g11
	out "Tab selected: %i" _i
	DT_Page hDlg _i



	case NM_CLICK ;;when user clicks a row and it does not begin cell edit mode
		NMITEMACTIVATE* na=+nh
		 out "row click: %i %i" na.iItem na.iSubItem
	case LVN_BEGINLABELEDIT
		 out "begin edit: item=%i subitem=%i text=%s" di.item.iItem di.item.iSubItem di.item.pszText
	case GRID.LVN_QG_BUTTONCLICK:
		 out "button2: item=%i subitem=%i text=%s" cd.item cd.subitem cd.txt
	case LVN_COLUMNCLICK ;;click header
		g.Sort(4|0x10000 nlv.iSubItem)
