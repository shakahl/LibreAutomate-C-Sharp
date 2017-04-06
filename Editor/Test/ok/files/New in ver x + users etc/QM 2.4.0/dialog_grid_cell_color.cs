\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

str controls = "3"
str qmg3x
if(!ShowDialog("" &dialog_grid_cell_color &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 3 QM_Grid 0x56031041 0x0 0 0 224 114 "0x2,0,0,0,0x0[]Computer name,,,[]IP,,,"
 4 Button 0x54032000 0x0 68 116 48 14 "SAVE"
 END DIALOG
 DIALOG EDITOR: "" 0x203050C "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG goto gInit
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto gNotify
ret
 messages2
ARRAY(POINT)- t_ed ;;edited cells
sel wParam
	case 4 ;;Save
	t_ed=0
	InvalidateRect id(3 hDlg) 0 1
	case IDOK
	case IDCANCEL
ret 1
 _______________________

 gInit

   ======= BEGIN: INITIAL FONT SETTINGS
__Font-- f fBold
f.Create("Courier New" 10 0)
fBold.Create("" 0 1 0 f 4)
f.SetDialogFont(hDlg "3")

int hlv=id(3 hDlg)
SendMessage hlv LVM_SETTEXTCOLOR 0 0xff0000
SendMessage hlv CCM_SETVERSION 5 0
   ======= END: INITIAL FONT SETTINGS

str csv=
 test1,192.168.0.1
 tes21,192.168.0.2
 test3,192.168.0.3

ICsv- t_csv=CreateCsv(1)
t_csv.FromString(csv)
t_csv.ToQmGrid(id(3 hDlg))

ret
 _______________________

 gNotify
int i
POINT p

NMHDR* nh=+lParam
sel nh.idFrom
	case 3
	GRID.QM_NMLVDATA* d=+nh
	NMLVDISPINFO* di=+nh
	NMLISTVIEW* nlv=+nh
	NMITEMACTIVATE* na=+nh
	sel nh.code
		case GRID.LVN_QG_CHANGE ;;when user changes grid content
		if(d.hctrl)
			out "text changed: item=%i, subitem=%i, text=%s, newtext=%s" d.item d.subitem d.txt _s.getwintext(d.hctrl)
			 add this cell to t_ed if does not exist
			p.x=d.subitem; p.y=d.item
			for(i 0 t_ed.len) if(!memcmp(&t_ed[i] &p 8)) break
			if(i=t_ed.len) t_ed[]=p
		else out "grid changed" ;;eg row deleted
		
		case NM_CUSTOMDRAW goto gCustomDraw
ret
 _______________________

 gCustomDraw
NMCUSTOMDRAW* cd=+lParam
NMLVCUSTOMDRAW* cd2=+lParam
sel cd.dwDrawStage
	case CDDS_PREPAINT _i=CDRF_NOTIFYITEMDRAW ;;yes, notify me to draw items
	case CDDS_ITEMPREPAINT _i=CDRF_NOTIFYSUBITEMDRAW ;;draw subitems
	case CDDS_ITEMPREPAINT|CDDS_SUBITEM
	p.x=cd2.iSubItem; p.y=cd.dwItemSpec
	for(i 0 t_ed.len) if(!memcmp(&t_ed[i] &p 8)) break
	if(i<t_ed.len)
		out "%i %i" p.x p.y
		cd2.clrText=0xff ;;red
		SelectObject cd.hdc fBold
	else
		cd2.clrText=0xff0000
		SelectObject cd.hdc f
	_i=CDRF_NEWFONT
ret DT_Ret(hDlg _i)
