function hDlg color [$controls]

 Sets font color for all or some controls in a dialog.
 Obsolete. Use <help>DT_SetTextColor</help>.

 hDlg, controls - see __Font.SetDialogFont.
 color - text color in format 0xBBGGRR.

 REMARKS
 Can set color only of controls of these classes: Static, Edit, ListBox.
 Although color is managed by this class, it is not related to font. The variable may be empty, ie you don't have to call Create to set colors.


type ___FONTCOLORS wp ARRAY(POINT)'a

ARRAY(int) a; int i j
sub_sys.Font_ParseControls(hDlg controls a)

___FONTCOLORS* p=+GetProp(hDlg "__Font.color")
if(!p)
	p._new
	p.wp=SubclassWindow(hDlg &sub.Subclass)
	SetProp hDlg "__Font.color" p

for i 0 a.len
	for(j 0 p.a.len) if(p.a[j].x=a[i]) p.a[j].y=color; break
	if(j<p.a.len) continue
	POINT x; x.x=a[i]; x.y=color
	p.a[]=x

if(IsWindowVisible(hDlg)) InvalidateRect hDlg 0 1


#sub Subclass
function# hDlg message wParam lParam

___FONTCOLORS* p=+GetProp(hDlg "__Font.color"); if(!p) ret
int wp=p.wp

sel(message)
	case [WM_CTLCOLORSTATIC,WM_CTLCOLOREDIT,WM_CTLCOLORLISTBOX]
	int c i
	for(i 0 p.a.len) if(p.a[i].x=lParam) c=p.a[i].y; break
	if(i<p.a.len)
		SetTextColor wParam c
		SetBkMode wParam 1
		sel(message)
			case WM_CTLCOLORSTATIC
				if(WinTest(lParam "Edit")) GetSysColorBrush(COLOR_BTNFACE) ;;readonly edit
				ret GetStockObject(NULL_BRUSH) ;;static, checkbox, group
			case else ret GetSysColorBrush(COLOR_WINDOW)
	case WM_NCDESTROY
	RemoveProp hDlg "__Font.color"
	p._delete

ret CallWindowProc(wp hDlg message wParam lParam)
