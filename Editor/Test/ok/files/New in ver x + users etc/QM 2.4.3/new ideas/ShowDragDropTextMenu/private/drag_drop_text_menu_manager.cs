 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(getopt(nthreads)>1) ret

if(!ShowDialog("" &drag_drop_text_menu_manager 0 0 128)) ret

 BEGIN DIALOG
 0 "" 0x80C800C8 0x0 0 0 223 135 "Dialog"
 END DIALOG
 DIALOG EDITOR: "" 0x2040101 "*" "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	int+ g_hwndDragDropTextMenuManager=hDlg
	
	case WM_DESTROY
	g_hwndDragDropTextMenuManager=0
	
	case WM_APP goto gMenu
	
	case WM_MENUDRAG goto gDrag
	
	case WM_MENURBUTTONUP goto gRight
	
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDCANCEL
ret 1
 ________________

 gMenu
ICsv-- t_csv._create

if t_csv.RowCount ;;if already in menu, end it and show new
	t_csv.Clear
	EndMenu
	PostMessage hDlg WM_APP wParam lParam
	ret

int i flags(wParam) onSelect
lpstr menuCSV(+lParam) csvErr
str sm sf sFree.lpstr=+lParam

sel flags&3
	case 1 menuCSV=sf.getfile(menuCSV); err csvErr=_error.description
	case 2 menuCSV=sf.getmacro(menuCSV); err csvErr=_error.description

if(!csvErr) t_csv.FromString(menuCSV); err csvErr="invalid CSV"

if(csvErr) out "ShowDragDropTextMenu error: %s" csvErr; ret

for(i 0 t_csv.RowCount) sm.addline(t_csv.Cell(i 0))
MenuPopup m.AddItems(sm 1)
if flags&7>4
	m.AddItems("-[]30000 Edit menu")

MENUINFO mi.cbSize=sizeof(mi)
mi.fMask=MIM_STYLE|MIM_APPLYTOSUBMENUS
mi.dwStyle=MNS_DRAGDROP
SetMenuInfo(m &mi)

i=m.Show(hDlg)
if i>0
	sel i
		case 30000
		sel flags&3
			case 1 run "qmcl.exe" F"''{sFree.lpstr}''"
			case 2 mac+ sFree.lpstr
		
		case else i-1; onSelect=1; goto gPaste
 gBack
t_csv.Clear
ret
 ________________

 gDrag
i=DDTM_GetItem(lParam wParam t_csv); if(i<0) ret

__Drag x.Init(hDlg 1)
rep
	if(!x.Next) break
	x.cursor=2
if(!x.dropped) ret

 lef ;;closes menu
int w=child(mouse 1)
act w; err
POINT p; xm p w 1
int xy=p.y<<16|p.x
SendMessage w WM_LBUTTONDOWN MK_LBUTTON xy
SendMessage w WM_LBUTTONUP 0 xy

 gPaste
ifk(C) key- C; key HSE ;;select line. Note: sometimes this may trigger QTranslate because creates double-Ctrl.
lpstr s=DDTM_GetItemText(t_csv i)

lpstr macro
if(t_csv.ColumnCount>2) macro=t_csv.Cell(i 2)
if(!empty(macro)) mac macro "" s
else paste s; err

if(onSelect) goto gBack

SetCursor LoadCursor(0 +IDC_ARROW)
 ret DT_Ret(hDlg MND_ENDMENU)
ret
 ________________

 gRight
i=DDTM_GetItem(lParam wParam t_csv); if(i<0) ret
sel ShowMenu("1 Copy text" hDlg 0 0 TPM_RECURSE)
	case 1 _s=DDTM_GetItemText(t_csv i); _s.setclip
ret
 ________________

err+ end _error 4
