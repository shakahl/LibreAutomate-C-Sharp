out

 optionally add icons
SetThreadMenuIcons "100=0 101=34 102=2" "$qm$\il_qm.bmp" 1

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 223 135 "Dialog"
 3 Button 0x54032000 0x0 8 8 48 14 "Popup menu"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040308 "*" "" "" ""

if(!ShowDialog(dd &sub.DlgProc 0 0 0 0 0 0 0 0 0 sub.GetMenuDef)) ret


#sub DlgProc
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	
	case WM_MEASUREITEM
	sel wParam
		case 0
		sub.MeasureMenuItem hDlg +lParam
		ret DT_Ret(hDlg 1)
	
	case WM_DRAWITEM
	sel wParam
		case 0
		sub.DrawMenuItem hDlg +lParam
		ret DT_Ret(hDlg 1)
ret
 messages2
sel wParam
	case 3 ;;Popup menu
	int i=ShowMenu(sub.GetMenuDef hDlg)
	
	case IDOK
	case IDCANCEL
ret 1


#sub GetMenuDef
function~

str md=
 BEGIN MENU
 >&File
 	&New :100 0x100 0x0 Cn
 	&Open :101 0x100 0x0 Co
 	&Save :102 0x100 0x0 Cs
 	Save &As... :103 0x100
 	-
 	>&Recent :0 0x100
 		not implemented :190 0x0 0x3
 		<
 	-
 	E&xit :2 0x100 0x0 AF4
 	<
 &Help :901
 END MENU
ret md


#sub MeasureMenuItem
function hDlg MEASUREITEMSTRUCT&m

BSTR b="Menu item text" ;;this should be retrieved from array that you create before showing menu
SIZE z
GetTextExtentPoint32W(sub.GetMenuDC b b.len &z)
m.itemWidth=z.cx
m.itemWidth+24 ;;add space for icon or checkbox
if(m.itemHeight<18) m.itemHeight=18 ;;icon height + 2 pixels between icons


#sub DrawMenuItem
function hDlg DRAWITEMSTRUCT&d

int hdc=d.hDC
RECT r=d.rcItem
int selected(d.itemState&ODS_SELECTED) disabled(d.itemState&ODS_DISABLED)

FillRect hdc &r iif(selected COLOR_HIGHLIGHT COLOR_MENU)+1

 this code gets default text color. Change it if need other color.
int col=-1
if(disabled) col=GetSysColor(COLOR_GRAYTEXT)
else if(selected) col=GetSysColor(COLOR_HIGHLIGHTTEXT)
else col=GetSysColor(COLOR_MENUTEXT)

SetTextColor(hdc col)
SetBkMode(hdc TRANSPARENT)

BSTR b="Menu item text" ;;this should be retrieved from array that you create before showing menu

r.left+24 ;;add space for icon or checkbox, which you can draw here or use SetThreadMenuIcons
r.top+1

DrawTextW(hdc b b.len &r DT_END_ELLIPSIS)


#sub GetMenuDC
function#

 Returns DC to measure menu item text width.

POINT-- m ;;x=DC, y=oldFont
if m.x=0
	NONCLIENTMETRICSW nc.cbSize=sizeof(nc)
	SystemParametersInfoW(SPI_GETNONCLIENTMETRICS, nc.cbSize, &nc, 0);
	int font=CreateFontIndirectW(&nc.lfMenuFont)
	m.x=CreateCompatibleDC(0)
	m.y=SelectObject(m.x font)
	atend sub.__DeleteMenuDC &m
ret m.x

#sub __DeleteMenuDC
function POINT&m
DeleteObject SelectObject(m.x m.y)
DeleteDC m.x
