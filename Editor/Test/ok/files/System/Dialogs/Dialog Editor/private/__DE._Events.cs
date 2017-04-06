 \Dialog_Editor

str m sc
int cid
__Subs x
if(!x.EventsInit(_ddMacro _ddSub 0)) ret

 init
if _hsel=_hform
	m="&WM_INITDIALOG (the first message)[]WM_DESTROY (the last message before destroying controls)[]WM_COMMAND (from controls)[]WM_NOTIFY (from controls)[]WM_SYSCOMMAND[]WM_MOVE[]WM_SIZE[]WM_EXITSIZEMOVE[]WM_ACTIVATE[]WM_MOUSEACTIVATE[]WM_NCACTIVATE[]WM_SHOWWINDOW[]WM_SETCURSOR[]WM_MOUSEMOVE[]WM_LBUTTONDOWN[]WM_LBUTTONUP[]WM_LBUTTONDBLCLK[]WM_RBUTTONDOWN[]WM_RBUTTONUP[]WM_TIMER[]WM_CONTEXTMENU[]WM_INITMENUPOPUP[]WM_CANCELMODE[]WM_CAPTURECHANGED[]WM_QM_DRAGDROP[]WM_USER[]WM_APP"
else
	cid=GetDlgCtrlID(_hsel); if(!cid) goto e2
	sel sc.getwinclass(_hsel) 1
		case "Button" m="&BN_CLICKED (clicked)"
		case ["ComboBox","QM_ComboBox"] m="&CBN_SELENDOK (item selected)[]CBN_DROPDOWN (before drop-down)"
		case "ListBox" m="&LBN_SELCHANGE (item selected)[]LBN_DBLCLK (double click; must have LBS_NOTIFY style)"
		case "Edit" m="&EN_CHANGE (text changed)[]EN_SETFOCUS[]EN_KILLFOCUS"
		case "QM_Edit" m="CBN_DROPDOWN (arrow button pressed)[]BN_CLICKED (user button clicked)[]EN_CHANGE (text changed)[]EN_SETFOCUS[]EN_KILLFOCUS"
		case ["RichEdit20A","RichEdit20W","RichEdit50W"] m="&EN_SETFOCUS[]EN_KILLFOCUS"
		case "Static" m="&STN_CLICKED (must have SS_NOTIFY style)"
		case "SysLink" m="Link"; goto gAfterDialog
		case "ActiveX" sub.ActiveX(x cid); ret
		case "QM_Grid"
		_i=find(_s.getmacro("sample_Grid") "[] messages3[]")+2
		out F"<>You can copy sample code from <open ''sample_Grid /{_i}''>sample_Grid</open>. QM_Grid notification code begins from the 'messages3' line. Also add the 'case WM_NOTIFY' line and 'DlgGrid g.Init...'."
		ret
		case else
		 e2
		mes "This control class is not supported." "" "!"; ret

 show dialog
str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 248 192 "Events"
 1 Button 0x54030001 0x4 8 172 48 14 "OK"
 2 Button 0x54030000 0x4 60 172 48 14 "Cancel"
 5 Button 0x54032000 0x0 112 172 18 14 "?"
 3 ComboBox 0x54230241 0x4 8 8 232 84 ""
 4 QM_DlgInfo 0x54000000 0x20000 8 96 232 68 "<>This will add or find a <help>case</help> statement in the dialog procedure for the selected event message. Also will minimize the Dialog Editor. Then you can add your code under that case statement (now or later). Your code will be executed in response to that event.[][]Here are listed some often used messages. You can find help and more messages in the MSDN Library (click the ? button). You can use <help>OutWinMsg</help> to see what messages are actually received."
 END DIALOG
 DIALOG EDITOR: "" 0x2040108 "*" "" "" ""

str controls = "3"
__strt cb3
cb3=m
if(!ShowDialog(dd &sub.DlgProcEvents &controls _hwnd)) ret
if(x.sText!=_s.getmacro(_ddMacro)) mes "Macro changed." "" "x"; ret

cb3.gett(cb3 1 " "); if(!cb3.len) ret
m=cb3.VN
int wm=!cid or m.beg("WM_")

 gAfterDialog
str sAppend
if !wm
	int clicked=SelStr(0 m "BN_CLICKED" "Link")
	if clicked
		m=cid
		if(clicked=1) sAppend.getwintext(_hsel); sAppend.getl(sAppend 0)
		else sAppend="link clicked. lParam is 0 for first link, 1 for second, and so on."
	else m+"<<16|"; m+cid
if(sAppend.len) sAppend-" ;;"

if wm
	sel m
		case "WM_TIMER"
		sAppend+"[][9]sel wParam[][9][9]case 1"
else
	sel sc 1
		case ["ComboBox","QM_ComboBox"] sAppend+"[][9]_i=CB_SelectedItem(lParam)"
		case "ListBox" sAppend+"[][9]_i=LB_SelectedItem(lParam)"

x.EventsInsert(_hwnd _ddMacro m sAppend wm)


#sub ActiveX c
function __Subs&x cid

int hCE=GetQmCodeEditor

RECT r1 r2; GetWindowRect hCE &r1; GetWindowRect _hwnd &r2; if(IntersectRect(&r1 &r1 &r2)) min _hwnd
mac+ _ddMacro; act hCE

 create string
str st.getwintext(_hselText) sv
if(st.replacerx("^(\w+\.\w+).*" "$1" 4)) st="UnknownTypelib.UnknownClass"
findrx(st "^\w+\.(\w{1,2})" 0 0 sv 1)
sv.lcase; sv+cid
str sNewCode=F"[9]{st} {sv}[][9]{sv}._getcontrol(id({cid} hDlg))[]"

 find where to insert
int i j
FINDRX f.ifrom=x.e.wmBegin; f.ito=x.e.wmEnd
i=findrx(x.sText "^[9]case WM_INITDIALOG\b.*?[]" f 8 j)
if(i<0) i=x.e.wmBegin; sNewCode-"[9]case WM_INITDIALOG[]"; j=-1
else i+j; f.ifrom=i; j=findrx(x.sText F"^\Q{sNewCode}\E" f 8)

if j<0 ;;insert
	j=sNewCode.len+1+sv.len
	sNewCode+F"[9]{sv}._setevents(''sub.{sv}'')[][9][]"
	SendMessage hCE SCI.SCI_GOTOPOS i 0
	InsertStatement sNewCode 0 0 1
	i+j
else ;;already exists
	i=j+sNewCode.len
	_s=F"[9]{sv}._setevents(''sub.{sv}'')"
	if !x.sText.mid(_s i)
		SendMessage hCE SCI.SCI_GOTOPOS i 0
		_s+"[]"
		InsertStatement _s 0 0 1
	i+1+sv.len

SendMessage hCE SCI.SCI_GOTOPOS i 0
mac "sub.ActiveXPopupAndTooltip"


#sub ActiveXPopupAndTooltip
key C.E ;;show popup list of members
_s="[]Double click an event (lightning icon) in the popup list.[]"
int h=wait(2 WV id(2205 _hwndqm)); err _s+"[]If there is no popup list, make sure that the type library declaration[]is inserted and the macro compiled (Ctrl+Shift+R).[]"
sub_sys.TooltipOsd _s 0 "DE Events" 0 0 0 h
if(!h) ret
wait 0 -WV h; err ret
OsdHide "DE Events"


#sub DlgProcEvents
function# hDlg message wParam lParam

int i; str s

sel message
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case 5
	i=ShowMenu("Selected notification message[]Control notifications messages" hDlg 0 2)
	sel i
		case 1 s.getwintext(id(3 hDlg)); s.gett(s 0)
		case 2
		s="control library list view tree"
		if(mes("This will search for controls reference.[]In the list of results:[]click 'Control Library (Windows)',[]click a control link,[]scroll to Notifications." "" "OCi")!='O') ret
	if(i) run F"http://www.google.com/search?q=site:microsoft.com {s.escape(9)}"; err
	
	case CBN_DBLCLK<<16|3 DT_Ok hDlg
ret 1
