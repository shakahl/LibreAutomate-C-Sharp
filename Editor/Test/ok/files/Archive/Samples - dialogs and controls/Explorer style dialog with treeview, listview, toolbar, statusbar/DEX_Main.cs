\Dialog_Editor

 Sample dialog with toolbar, tree view, list view and status bar controls, with images.
 Also shows how to use custom draw to change tree view and list view item text and background colors. In you don't need it, remove case NM_CUSTOMDRAW lines in DEX_Dialog.
 Run this function.


type DEX_DATA __ImageList'il ;;you can add more members to store various data
DEX_DATA d ;;to access this variable from dialog procedure, use DEX_DATA& d; &d=+DT_GetParam(hDlg)

 add bitmap containing all icons
d.il.Load("$qm$\il_qm.bmp") ;;loads an imagelist created with the imagelist editor
 or load multiple files (slower):
 str iconlist=
  $qm$\close.ico
  $qm$\controls.ico
  $qm$\copy.ico
  $qm$\cut.ico
  $qm$\del.ico
  $qm$\dialog.ico
  $qm$\email.ico
  $qm$\favorites.ico
  $qm$\files.ico
  $qm$\find.ico
 d.il.Create(iconlist)

str dd=
 BEGIN DIALOG
 1 "" 0x90CF0A48 0x100 0 0 222 142 "Dialog"
 5 SysListView32 0x54010000 0x200 84 18 68 64 ""
 4 SysTreeView32 0x54010000 0x200 4 18 68 64 ""
 3 ToolbarWindow32 0x54010000 0x0 0 0 222 17 ""
 6 msctls_statusbar32 0x54000100 0x0 0 128 222 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020002 "*" ""

if(!ShowDialog(dd &sub.WndProc 0 0 0 0 0 &d)) ret


#sub WndProc
function# hDlg message wParam lParam

 messages
sel message
	case WM_INITDIALOG
	sub.InitToolbar hDlg id(3 hDlg)
	sub.InitStatusbar hDlg id(6 hDlg)
	sub.InitTreeview hDlg id(4 hDlg)
	sub.InitListview hDlg id(5 hDlg) 0
	sub.Autosize hDlg
	
	case WM_DESTROY

	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	case WM_SIZE sub.Autosize hDlg
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case [1001,1002,1003] sub.SetStatusbarText hDlg 0 wParam ;;toolbar button clicked
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3 ;;toolbar
	sel nh.code
		case NM_RCLICK
		NMMOUSE* mo=+nh
		if(mo.dwItemSpec=-1) ret ;;not on button
		out "right click on button %i" mo.dwItemSpec
		
		case TBN_GETINFOTIPW
		NMTBGETINFOTIP* tt=+lParam
		str-- stt
		stt.format("tooltip text of button %i" tt.iItem)
		tt.pszText=stt.unicode
		
	case 4 ;;tree
	sel nh.code
		case TVN_SELCHANGEDW ;;treeview item selected
		NMTREEVIEWW* ntv=+nh
		int i=ntv.itemNew.lParam
		if(i) sub.SetStatusbarText hDlg 1 i ;;else folder
		
		case NM_CUSTOMDRAW ret DT_Ret(hDlg sub.TvCustomdraw(+nh))
	
	case 5 ;;list
	sel nh.code
		case LVN_ITEMCHANGED
		NMITEMACTIVATE* na=+nh
		if(na.uNewState&LVIS_SELECTED and na.uOldState&LVIS_SELECTED=0) ;;listview item selected
			SetFocus nh.hwndFrom
			sub.SetStatusbarText hDlg 2 na.lParam ;;was set by DEX_LvAdd
		
		case NM_CUSTOMDRAW ret DT_Ret(hDlg sub.LvCustomdraw(+nh))
		


#sub InitToolbar
function hDlg htb

DEX_DATA& d=+DT_GetParam(hDlg)

SetWinStyle htb TBSTYLE_FLAT|TBSTYLE_TOOLTIPS 1
SendMessage htb TB_SETIMAGELIST 0 d.il

ARRAY(TBBUTTON) a.create(3)
str strings="One[]Two[]Three"
strings+"[]"; strings.findreplace("[]" "" 16); BSTR bstrings=strings ;;must be UTF-16 multi-string terminated with two 0
SendMessage(htb TB_ADDSTRINGW 0 bstrings.pstr)
int i
for i 0 a.len
	TBBUTTON& t=a[i]
	t.idCommand=1001+i
	t.iBitmap=i+1
	t.iString=i
	t.fsState=TBSTATE_ENABLED

SendMessage(htb TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0)
SendMessage(htb TB_ADDBUTTONSW a.len &a[0])


#sub InitListview
function hDlg hlv reinit

DEX_DATA& d=+DT_GetParam(hDlg)

if(!reinit)
	SetWinStyle hlv LVS_REPORT|LVS_SHAREIMAGELISTS|LVS_SINGLESEL 1
	SendMessage hlv LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_INFOTIP|LVS_EX_FULLROWSELECT|LVS_EX_ONECLICKACTIVATE|LVS_EX_DOUBLEBUFFER -1
	SendMessage hlv LVM_SETIMAGELIST LVSIL_SMALL d.il
	TO_LvAddCols hlv "Item" 70 "Subitem" 70 "Subitem" 70
else
	SendMessage hlv LVM_DELETEALLITEMS 0 0

TO_LvAdd hlv 0 3001 7 "One" 1 2
TO_LvAdd hlv 1 3002 8 "Two" 3 4
TO_LvAdd hlv 2 3003 9 "Three" 5 6


#sub InitTreeview
function hDlg htv

DEX_DATA& d=+DT_GetParam(hDlg)

SetWinStyle htv TVS_HASBUTTONS|TVS_HASLINES|TVS_LINESATROOT|TVS_INFOTIP 1
 SetWinStyle htv TVS_SINGLEEXPAND|TVS_FULLROWSELECT|TVS_INFOTIP|TVS_TRACKSELECT 1

SendMessage htv TVM_SETIMAGELIST 0 d.il

int hif
int first=TvAdd(htv 0 "One" 2001 4)
hif=TvAdd(htv 0 "Folder" 0 9)
TvAdd htv hif "Two" 2002 5
TvAdd htv 0 "Three" 2003 6

SendMessage htv TVM_SELECTITEM TVGN_CARET first


#sub InitStatusbar
function hDlg hsb

int w1(70) w2(140) w3(-1)
SendMessage hsb SB_SETPARTS 3 &w1


#sub SetStatusbarText
function hDlg part str's

SendMessage id(6 hDlg) SB_SETTEXTW part @s


#sub Autosize
function hDlg

int htb(id(3 hDlg)) htv(id(4 hDlg)) hlv(id(5 hDlg)) hsb(id(6 hDlg))
SendMessage(htb TB_AUTOSIZE 0 0)
SendMessage(hsb WM_SIZE 0 0)

int y1 h1 w1 w2
RECT rc rtb rsb
GetClientRect(hDlg &rc)
GetWindowRect(htb &rtb)
GetWindowRect(hsb &rsb)
y1=rtb.bottom-rtb.top
h1=rc.bottom-y1-(rsb.bottom-rsb.top); if(h1<0) h1=0
w1=120
w2=rc.right-w1-4; if(w2<0) w2=0
MoveWindow htv 0 y1 w1 h1 1
MoveWindow hlv w1+4 y1 w2 h1 1


#sub TvCustomdraw
function# NMTVCUSTOMDRAW*cd

 out cd.nmcd.dwDrawStage
sel cd.nmcd.dwDrawStage
	case CDDS_PREPAINT ret CDRF_NOTIFYITEMDRAW ;;yes, notify me to draw items
	case CDDS_ITEMPREPAINT
	sel cd.nmcd.lItemlParam
		case 2001 cd.clrText=0xFF
		case 2002 cd.clrText=0xFF00
		case 2003 cd.clrText=0xFF0000


#sub LvCustomdraw
function# NMLVCUSTOMDRAW*cd

 out cd.nmcd.dwDrawStage
sel cd.nmcd.dwDrawStage
	case CDDS_PREPAINT ret CDRF_NOTIFYITEMDRAW ;;yes, notify me to draw items
	case CDDS_ITEMPREPAINT
	cd.clrTextBk=iif(cd.nmcd.dwItemSpec&1 0xC0FFC0 0xC0FFFF) ;;makes alternating row colors
