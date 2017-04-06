\Dialog_Editor
 /exe 1
out

str dd=
 BEGIN DIALOG
 0 "" 0x90CF0AC8 0x0 0 0 224 150 "Dialog"
 3 QM_ComboBox 0x54230243 0x0 8 8 96 13 "RO"
 4 QM_ComboBox 0x54230A42 0x0 8 52 180 13 "Ed" "tttttttttttt"
 5 ComboBox 0x54230243 0x0 120 8 96 213 "RO"
 6 ComboBox 0x54230242 0x0 120 28 96 213 "Ed" "mmmmmmmmmmm"
 8 Button 0x54032000 0x0 168 76 48 14 "Test"
 7 Edit 0x54030080 0x200 8 84 96 13 ""
 9 Button 0x54012003 0x0 168 100 48 10 "&Check"
 10 ListBox 0x54230101 0x200 8 112 42 30 ""
 11 ListBox 0x54230109 0x200 56 112 42 30 ""
 1 Button 0x54030001 0x4 116 132 48 14 "OK"
 2 Button 0x54030000 0x4 168 132 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2040300 "*" "" "" ""
 3 QM_ComboBox 0x54230243 0x0 8 8 96 13 "RO"

 __ImageList il.Load("$qm$\il_qm.bmp")
 __ImageList il
 if(1) il=__ImageListLoad("$qm$\il_qm.bmp")
 else il=ImageList_Create(1 16 0 0 0) ;;set row height. Without this on XP 14, with icons 18; on Win7+ always 19.

 str controls = "4 5 6 7"
 str qmcb4Ed cb5RO cb6Ed e7
 str qmcb3RO
str controls = "3 4 5 6 7 9 10 11"
str qmcb3RO qmcb4Ed cb5RO cb6Ed e7 c9Che lb10 lb11
qmcb3RO=
 -1 def text,$qm$\il_qm.bmp,0x201,Cue,0x8000,0xffcccc
 Sunday,11,,Tooltip,0xff,0xc0f0c0
 Monday,-1,,,,0xe0e0e0
 "Tuesday",3,1,"Multiline
 tooltip"
 Wednesday,,0xE,,,,2
 Thursday,-1,0xE,,,0xe0e0e0
 "Friday ""looooooooooong""",5,,Tooooooooo
 Saturday ,-1,0xE,,,0xe0e0e0

 rep(50) qmcb3RO.addline(_s.RandomString(5 20 "a-z") 1); qmcb3RO+",4"

 0,$my qm$\imagelists\icons32.bmp,0x00,Cue,0x8000,0xffcccc
 qmcb3RO="[]Sunday[]Monday"
 qmcb3cb=",,7[]one,,1[]two[]Three,,1[]Four[]Thr"
 qmcb3cb=",,1,Cue,0x8000,0xf0ffff[]one,,1,Tooltip,0xff,0xc0f0c0,4[]Header,,0x100[]Three,,1,''Multiline[]tooltip'',,,2[]Four[]Thr wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww,,,Tooooooooo"

qmcb4Ed=qmcb3RO

cb5RO="&one[]two[]Three[]four[]five[]Thr[]loooooooooooooooooooooooooooooooooooong"
cb6Ed="&one[]two[]Three[]four[]five[]Thr[]loooooooooooooooooooooooooooooooooooong"
 rep(5) cb6Ed.addline(cb6Ed 1)

lb10="&one[]two"; lb11=lb10

if(!ShowDialog(dd &sub.DlgProc &controls _hwndqm)) ret
out qmcb3RO
out qmcb4Ed


#sub DlgProc
function# hDlg message wParam lParam

int+ g_outwinmsg; if(g_outwinmsg) OutWinMsg message wParam lParam
int- subclassed

IQmDropdown- t_dd

sel message
	case WM_INITDIALOG
	 SetWindowTheme id(3 hDlg) L"" L""
	__Font- f.Create("Comic Sans MS" 16)
	 f.SetDialogFont(hDlg "3 5 6")
	
	 PF
	 int h=CreateControl(0 "QM_ComboBox" 0 0x54230243 0 70 100 30 hDlg 77)
	 PN;PO
	 outw h
	
	 int c=id(4 hDlg); RECT r; GetWindowRect c &r; OffsetRect &r -r.left -r.top; InflateRect &r -2 -2
	 int hr=CreateRectRgnIndirect(&r)
	 SetWindowRgn c hr 0
	
	 mov 0.4 0.92 hDlg
	SetTimer hDlg 1 4000 0
	
	case WM_TIMER
	KillTimer hDlg wParam
	sel wParam
		case 1
		 SendMessage(id(4 hDlg) CB_SETCURSEL 2 0)
		 SendMessage(id(4 hDlg) CB_RESETCONTENT 0 0)
		 SendMessage(id(4 hDlg) CB_SELECTSTRING -1 "Four")
		 int w
		  w=FindWindowExW(0 0 L"ComboLBox" 0)
		  outw w
		  out IsWindowVisible(w)
		   Zorder hDlg
		 SetWindowPos hDlg 0 0 0 0 0 SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE|SWP_NOOWNERZORDER
		   SetForegroundWindow win("Notepad")
		  w=FindWindowExW(0 0 L"ComboLBox" 0)
		  outw w
		  out IsWindowVisible(w)
		   outw GetWindow(w GW_OWNER)
		  RemoveWindowSubclass(subclassed &sub.WndProc_Subclass 1)
		
		if(!t_dd) ret
		 t_dd.Check(0 2)
		 t_dd.Close
		 
		case 2
		int i
		 for(i 0 SendMessage(id(4 hDlg) CB_GETCOUNT 0 0)) out SendMessage(id(4 hDlg) LB_GETSEL i 0)
		out SendMessage(id(4 hDlg) LB_SETSEL but(9 hDlg) GetDlgItemInt(hDlg 7 0 1))
		
		 SendMessage(id(4 hDlg) CB_SHOWDROPDOWN 0 0)
		   subclassed=FindWindowExW(0 0 L"ComboLBox" 0)
		  subclassed=id(5 hDlg)
		  out SetWindowSubclass(subclassed &sub.WndProc_Subclass 1 0)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	
	 case WM_SETCURSOR
	 if(subclassed or lParam>>16!WM_LBUTTONDOWN) ret
	 SetTimer hDlg 2 300 0
ret
 messages2
 OutWinMsg message wParam lParam ;;uncomment to see received messages
sel wParam
	case 8 ;;Test
	int h
	h=id(4 hDlg)
	 h=id(6 hDlg)
	
	 DT_SetControl hDlg 4 "0[]new[]bbb"
	 ret
	
	 _s="one,four"
	  _s.setwintext(h)
	 _s.setwintext(id(1001 h))
	
	 out SendMessage(h CB_GETITEMHEIGHT -1 0) ;;15
	 SendMessage(h CB_SETITEMHEIGHT -1 14)
	 out SendMessage(h CB_GETCURSEL 0 0)
	 out SendMessage(h CB_SETCURSEL 2 0)
	 out SendMessage(h CB_SELECTSTRING -1 "Thre")
	 out SendMessage(h CB_FINDSTRING -1 "Thre")
	 out SendMessage(h CB_FINDSTRINGEXACT -1 "Three")
	 out SendMessage(h CB_RESETCONTENT 0 0)
	 out SendMessage(h CB_ADDSTRING 0 "added")
	 rep(1) out SendMessage(h CB_INSERTSTRING 0 "added,11,1,new tooltip,0xffff88")
	 out SendMessage(h CB_INSERTSTRING 7 "added")
	 rep(3) out SendMessage(h CB_DELETESTRING 0 0)
	
	 out SendMessage(h CB_GETDROPPEDWIDTH 0 0)
	 out SendMessage(h CB_SETDROPPEDWIDTH 400 0)
	 out SendMessage(h CB_GETDROPPEDWIDTH 0 0)
	
	 out SendMessage(h CB_GETLBTEXTLEN 1 0)
	 out CB_GetItemText(h 2 _s); out _s
	
	 out SendMessage(h CB_SETITEMDATA 0 33)
	 out SendMessage(h CB_GETITEMDATA 0 0)
	
	 out CB_Add(h "added" 77)
	 out CB_FindItem(h "fo")
	 out CB_GetCount(h)
	 out CB_SelectedItem(h); out CB_SelectedItem(h _s); out _s
	 out CB_SelectItem(h 3)
	 out CB_SelectString(h "four")
	
	 out SendMessage(id(10 hDlg) LB_GETSEL 0 0)
	 out SendMessage(id(11 hDlg) LB_GETSEL 0 0)
	 out SendMessage(id(10 hDlg) LB_SETSEL 1 0)
	 out SendMessage(id(11 hDlg) LB_SETSEL 1 0)
	 out SendMessage(id(11 hDlg) LB_SETSEL 0 -1)
	
	 int i
	  for(i 0 SendMessage(h CB_GETCOUNT 0 0)) out SendMessage(h LB_GETSEL i 0)
	 out SendMessage(h LB_SETSEL but(9 hDlg) GetDlgItemInt(hDlg 7 0 1))
	
	 SetTimer hDlg 2 3000 0
	 SendMessage(h CB_SHOWDROPDOWN 1 0)
	 DT_GetControl hDlg 4 _s; out _s
	
	 __Tooltip& t=+DT_GetTooltip(hDlg)
	 t.Update(4 "Updated")
	
	ICsv x._create
	str s="0,,1[]zero[]one[]two,,,tooltip"
	rep(7) s.addline("addline" 1)
	x.FromString(s)
	h=id(7 hDlg)
	SetFocus h
	 outx ShowDropdownList(x 0 0 0 h 0 &sub.Callback 111 t_dd)
	outx ShowDropdownList(x 0 0 0 h 0 &sub.Callback 111)
	x.ToString(_s); out _s
	
	 __QM_Dropdown dd
	
	 SendMessage(id(3 hDlg) CB_SETCURSEL 2 0)
	 out SendMessage(id(4 hDlg) CB_GETCURSEL 0 0)
	
	case IDOK
	case IDCANCEL
	
	 case CBN_KILLFOCUS<<16|4
	 int c
	 c=id(4 hDlg); RECT r; GetWindowRect c &r; OffsetRect &r -r.left -r.top; InflateRect &r -1 -1
	 int hr=CreateRectRgnIndirect(&r)
	 SetWindowRgn c hr 1
	 
	 case CBN_SETFOCUS<<16|4
	 SetWindowRgn id(4 hDlg) 0 1
ret 1

 messages3
NMQMCB* n=+lParam
sel(n.hdr.idFrom) case [3,4] case else ret
if(n.hdr.code!1) ret
out "%i %i" n.itemIndex n.itemState

n.dd.Check(0 n.itemState)

 ICsv x
 d.GetInfo(0 0 0 0 x)
 int i=n.itemIndex
 x.CellInt(i 5)=0xff8899
 x.CellInt(i 2)=4
 x.Cell(i 3)="new tooltip"
 d.Update(i 0 1)


#sub WndProc_Subclass
function# hwnd message wParam lParam uIdSubclass dwRefData

 This function can be used with SetWindowSubclass as window procedure.
 <help>SetWindowSubclass</help> is the recommended way to subclass windows. Easier and safer than SetWindowLong. Example: SetWindowSubclass(hwnd &sub.WndProc_Subclass 1 0)


OutWinMsg message wParam lParam ;;uncomment to see received messages

 sel message
	 case ...

int R=DefSubclassProc(hwnd message wParam lParam)

sel message
	case WM_NCDESTROY
	RemoveWindowSubclass(hwnd &sub.WndProc_Subclass 1) ;;replace ThisFunction with the name of this function or subfunction (eg sub.WndProc_Subclass)
	
	 case ...

ret R


#sub Callback
function# cbParam QMDROPDOWNCALLBACKDATA&x

out "%i %i %i %i %i" cbParam x.code x.itemIndex x.itemState
if(x.code!1) ret

ICsv csv=x.dd.Csv
int i=x.itemIndex+1
csv.CellInt(i 5)=0xff8899
csv.CellInt(i 2)=x.dd.IsChecked(x.itemIndex)|4
csv.Cell(i 3)="new tooltip"
x.dd.Update(x.itemIndex 0 1)

x.dd.Check(0 2)
