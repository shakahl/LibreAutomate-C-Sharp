 /dialog_QM_Tools
 \Dialog_Editor

str s
_WinGetText(_s s 3)
if(s.beg("_i=")) s.get(s 3)
if(!sub.Dlg(0 0 &s &this)) ret
EditReplaceSel mw_heW 0 s 1


#sub Dlg
function# hDlg message wParam lParam
if(hDlg) goto messages

 If not a sub, this func could be used everywhere.
 To show this dialog, call this func with hDlg=0, wParam=str* (sets and receives "win(...)"), lParam=__ToolsControl* or 0.
 Returns 1 on OK.

str& s=+wParam; str sTest
int hw hc
if lParam
	__ToolsControl& tc=+lParam
	hw=tc.mw_captW; if(!IsWindow(hw)) hw=0
	hc=tc.mw_captC; if(!IsWindow(hc)) hc=0
	hDlg=tc.m_hwnd
else hDlg=_hwndqm

str controls = "6 9 7 18 16 10 15"
__strt e6Nam e9Cla cb7Nam c18Pro e16Pro qmg10x e15Mat

int-- t_wd_flags; t_wd_flags=0
ARRAY(str) a
int i test
__strt prop
ICsv c

if matchw(s "win(?*)")
	s.get(s 4 s.len-5)
	__Tok(s &a "sssnsn")
	e6Nam=a[0]; e9Cla=a[1]; e16Pro=a[2]; t_wd_flags=__Eval(a[3]); prop=a[4]; e15Mat=a[5]
	if(e16Pro.len) c18Pro=1; else if(hw) e16Pro.getwinexe(hw)
else if hw
	e6Nam.getwintext(hw)
	e9Cla.getwinclass(hw)
	e16Pro.getwinexe(hw)

sub_to.CB_InitVar cb7Nam t_wd_flags&1|(t_wd_flags&0x200>>8) "Partial[]Full or with *?[]Regular expr."

lpstr rows=
 owner,{hwndOwner}
 xy
 style
 exStyle
 cClass
 <8>cText
 cId
 cFlags
 GetProp
 callback,{&MyFunc} {myParam}
 threadId,{tid}
 processHandle,{hp}
sub_to.GridCsvSetVar c rows prop i
if hw
	if(i&4=0) c.CellHex(2 1)=GetWinStyle(hw)
	if(i&8=0) c.CellHex(3 1)=GetWinStyle(hw 1)
	if(i&16=0 and hc) c.Cell(4 1)=s.getwinclass(hc)
	if(i&32=0 and hc) c.Cell(5 1)=s.getwintext(hc)
	if(i&64=0 and hc) c.CellInt(6 1)=GetWinId(hc)
c.ToString(qmg10x)


if(!ShowDialog("" &sub.Dlg &controls hDlg)) ret

 gTest

if(c18Pro!1) e16Pro=""

int f=t_wd_flags~0x201; sel(val(cb7Nam)) case 1 f|1; case 2 f|0x200
if(!e6Nam.len) f~0x213

s=F"win({e6Nam.S} {e9Cla.S} {e16Pro.S} 0x{f} {qmg10x.CSV} {e15Mat.N})"
sub_to.Trim s " '''' '''' 0x0 '''' 0"

if(test) sub_to.Test hDlg F"_i={s}" 0 1 19 "__ToolsControl._WinTest"

ret 1

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 298 180 "Window properties"
 3 Static 0x54000000 0x0 4 6 24 12 "Name"
 6 Edit 0x54030080 0x200 30 4 264 14 "Nam" "Must match case"
 8 Static 0x54000000 0x0 4 22 24 12 "Class"
 9 Edit 0x54030080 0x200 30 20 122 14 "Cla" "Can be with wildcard characters *?"
 11 Static 0x54000000 0x0 180 22 30 12 "Name is"
 7 ComboBox 0x54230243 0x0 212 20 62 213 "Nam"
 13 Button 0x54032000 0x0 276 20 18 14 "RX" "Regular expression menu"
 18 Button 0x54012003 0x0 4 38 44 12 "Program"
 16 Edit 0x54030080 0x200 50 36 244 14 "Pro" "Filename (eg notepad) or full path (eg $system$\notepad.exe), or process id variable in ( ) (eg (pid))."
 10 QM_Grid 0x56035041 0x200 4 54 290 76 "0x27,0,0,14,0x10008000[],20%,,[],75%,," "owner - owner window handle in { }.[]xy - x and y of a point in screen that the window must contain. Can be part of screen (eg 0.5 0.5).[]style, exStyle - window style and extended style. Can be followed by mask.[]cClass, cText, cId, cFlags - a control in the window. Like with function child().[][]Variables must be enclosed in { }."
 14 Static 0x54000000 0x0 4 138 34 13 "Match #"
 15 Edit 0x54030080 0x200 40 136 44 14 "Mat" "1-based match index.[]For example, use 2 for the second matching window."
 12 Button 0x54032000 0x0 256 136 38 14 "Flags..."
 1 Button 0x54030001 0x4 4 162 48 14 "OK"
 2 Button 0x54030000 0x4 54 162 48 14 "Cancel"
 4 Button 0x54032000 0x4 104 162 18 14 "?"
 17 Button 0x54032000 0x0 124 162 48 14 "Test"
 19 Static 0x54000000 0x0 176 164 80 13 ""
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
TO_EditCheckOnChange hDlg 16 18
sel wParam
	case 4 QmHelp "win"
	case 12 sub_to.FlagsDialog(t_wd_flags "2,Name is case insensitive[]8,Must be popup window (style 0x80000000)[]4,Must be not popup window[]0x400,Must be visible" hDlg)
	case 13 if(RegExpMenu(id(6 hDlg))) CB_SelectItem id(7 hDlg) 2
	case 17 DT_GetControls(hDlg &controls); test=1; &s=sTest; goto gTest
ret 1
