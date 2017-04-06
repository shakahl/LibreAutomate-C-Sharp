 /dialog_QM_Tools
 \Dialog_Editor

str sc
str- ___t_s_win
_WinGetText(sc ___t_s_win 3)
if(!sub.Dlg(0 0 &sc &this)) ret
EditReplaceSel mw_heC 0 sc 1


#sub Dlg
function# hDlg message wParam lParam
if(hDlg) goto messages

 If not a sub, this func could be used everywhere.
 To show this dialog, call this func with hDlg=0, wParam=str* (sets and receives "child(...)" or "id(...)"), lParam=__ToolsControl* or 0.
 Returns 1 on OK.

str& s=+wParam; str sTest; str- ___t_s_win
int h
if lParam
	__ToolsControl& tc=+lParam
	h=tc.mw_captC; if(!IsWindow(h)) h=0
	hDlg=tc.m_hwnd
else hDlg=_hwndqm

str controls = "3 6 8 9 7 10 15"
__strt c3Tex e6Tex c8Cla e9Cla cb7Tex qmg10x e15Mat

int-- t_wd_flags; t_wd_flags=0
ARRAY(str) a
int i test
__strt prop
ICsv c

sel s 2
	case "child(?*)"
	s.get(s 6 s.len-7)
	__Tok(s &a "ss nsn")
	e6Tex=a[0]; e9Cla=a[1]; t_wd_flags=__Eval(a[3]); prop=a[4]; e15Mat=a[5]
	if(e6Tex.len) c3Tex=1
	if(e9Cla.len) c8Cla=1
	
	case "id(?*)"
	s.get(s 3 s.len-4)
	__Tok(s &a "n n")
	prop=F"id={a[0]}"
	i=__Eval(a[2]); if(i&1) t_wd_flags|16

if h
	if(!e6Tex.len) e6Tex.getwintext(h)
	if(!e9Cla.len) e9Cla.getwinclass(h)

sub_to.CB_InitVar cb7Tex t_wd_flags&1|(t_wd_flags&0x200>>8) "Partial[]Full or with *?[]Regular expr."

lpstr rows=
 id,
 <8>accName
 wfName
 xy
 style
 exStyle
 cClass
 <8>cText
 cId
 cFlags
 GetProp
 callback,{&MyFunc} {myParam}
sub_to.GridCsvSetVar c rows prop i
if h
	if(i&1=0) c.CellInt(0 1)=GetDlgCtrlID(h)
	if(i&2=0) Acc ac=acc(h); if(ac.a) c.Cell(1 1)=ac.Name; err
	if(i&16=0) c.CellHex(4 1)=GetWinStyle(h)
	if(i&32=0) c.CellHex(5 1)=GetWinStyle(h 1)
c.ToString(qmg10x)


if(!ShowDialog("" &sub.Dlg &controls hDlg)) ret

 gTest

if(c3Tex!1) e6Tex=""
if(c8Cla!1) e9Cla=""

int f=t_wd_flags~0x201; sel(val(cb7Tex)) case 1 f|1; case 2 f|0x200
if(!e6Tex.len) f~0x207

if(!test) _s="{window}"; else if(___t_s_win.len) _s="_i"; else _s="''''"

qmg10x.rtrim("[]")

if !e6Tex.len and !e9Cla.len and !(f&~16) and !findrx(qmg10x "^id,\d+$") and !e15Mat.len
	s=F"id({qmg10x+3} {_s}"
	if(f&16) s+" 1"
	s+")"
else
	s=F"child({e6Tex.S} {e9Cla.S} {_s} 0x{f} {qmg10x.CSV} {e15Mat.N})"
	sub_to.Trim s " 0x0 '''' 0"

if(test) sub_to.Test hDlg F"{___t_s_win}[]_i={s}" 0 1 16 "__ToolsControl._WinTest"

ret 1

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 297 180 "Control properties"
 3 Button 0x54012003 0x0 4 4 34 13 "Text"
 6 Edit 0x50231044 0x200 38 4 256 30 "Tex" "Must match case"
 8 Button 0x54012003 0x0 4 36 34 13 "Class"
 9 Edit 0x50030080 0x200 38 36 120 14 "Cla" "Can be with wildcard characters *?"
 11 Static 0x54000000 0x0 182 38 28 13 "Text is"
 7 ComboBox 0x54230243 0x0 212 36 62 213 "Tex"
 13 Button 0x54032000 0x0 276 36 18 14 "RX" "Regular expression menu"
 10 QM_Grid 0x56035041 0x200 4 54 290 76 "0x27,0,0,14,0x10008000[],20%,,[],75%,," "accName - accessible object name.[]wfName - Windows Forms (.NET) control name.[]xy - x and y of a point in client area of parent window that must be in this control. Can be part (eg 0.5 0.5).[]style, exStyle - style and extended style. Can be followed by mask.[]cClass, cText, cId, cFlags - a child control of this control. Like with function child().[][]Variables must be enclosed in { }."
 14 Static 0x54000000 0x0 4 138 34 13 "Match #"
 15 Edit 0x54030080 0x200 40 136 44 14 "Mat" "1-based match index.[]For example, use 2 for the second matching control."
 12 Button 0x54032000 0x0 256 136 38 14 "Flags..."
 1 Button 0x54030001 0x4 4 162 48 14 "OK"
 2 Button 0x54030000 0x4 54 162 48 14 "Cancel"
 4 Button 0x54032000 0x4 104 162 18 14 "?"
 17 Button 0x54032000 0x0 124 162 48 14 "Test"
 16 Static 0x54000000 0x0 180 164 78 13 ""
 5 Static 0x54000010 0x20004 0 156 312 2 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2040202 "*" "" "" ""

 messages
sel message
	case WM_INITDIALOG
	case WM_COMMAND goto messages2
	case WM_NOTIFY TO_GridCheckOnChange lParam 10
ret
 messages2
TO_EditCheckOnChange hDlg 9 8  6 3
sel wParam
	case 4 QmHelp "child"
	case 12 sub_to.FlagsDialog(t_wd_flags "2,Text is case insensitive[]4,Get text using an alternative method[]16,Must be direct child[]0x400,Must be visible" hDlg)
	case 13 if(RegExpMenu(id(6 hDlg))) CB_SelectItem id(7 hDlg) 2
	case 17 DT_GetControls(hDlg &controls); test=1; &s=sTest; goto gTest
ret 1
