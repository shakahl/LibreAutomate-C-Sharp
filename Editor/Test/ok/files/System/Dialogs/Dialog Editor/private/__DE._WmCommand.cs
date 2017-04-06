 \Dialog_Editor
function wParam [lParam]

sel wParam
	case 1001 _Save
	case 1002 PostMessage(_hwnd WM_CLOSE 0 0)
	case 1004 sub.Test
	case 1005 QmHelp "IDH_DIALOG_EDITOR"
	case 1006 _Options
	case 1007 _Tooltip
	case 1008 sub.Delete
	case 1009 _Style
	case 1011 _Page(_page-1)
	case 1012 _Page(_page+1)
	case 1013 sub.Edit
	case 1014 _Undo(1)
	case 1015 _Undo(2)
	case 1016 _Events
	case 1021 sub.OnProperties
	case 1022 sub.TextEditDialog
	case EN_CHANGE<<16|1020 sub.OnTextChange
	case EN_CHANGE<<16|1013 _Page(-2)
	case 1030 mac "TO_MenuEditor"
	case 1040 sub.Edit(lParam) ;;from _Key


#sub OnProperties c

sel _s.getwinclass(_hsel) 1
	case "QM_Grid" _Prop_Grid


#sub TextEditDialog c

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 282 96 "Text"
 3 Edit 0x54231044 0x200 4 4 274 66 "txt"
 1 Button 0x54030001 0x4 4 76 48 14 "OK"
 2 Button 0x54030000 0x4 54 76 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030509 "*" "" "" ""

str controls = "3"
str e3txt
e3txt.getwintext(_hselText)
if(!ShowDialog(dd 0 &controls _hwnd)) ret
EditReplaceSel _hselText 0 e3txt 3


#sub OnTextChange c
if(_textChanged) _save=1
else _Undo; _textChanged=1

str s ss
if(!_hsel) _hsel=_hform
ss.getwintext(_hselText)

___DE_CONTROL& c=subs.GetControl(_hsel)
if(_hsel=_hform or __CDD_CanHaveText(s.getwinclass(_hsel) c.style)) ss.setwintext(_hsel)
c.txt=ss

if(_hsel=_hform) ss="Dialog"
else _Name(c _hsel s ss)
ss.setwintext(_hselName)
 

#sub Edit c
function [action]

___DE_CLIPBOARD+ __de_clipboard

if(!_hsel) ret
if(action) goto g1
str mi=
 5Cu&t	Ctrl+X
 1&Copy	Ctrl+C
 2&Paste	Ctrl+V
 3Clo&ne	Ctrl+D
MenuPopup m.AddItems(mi)
if(_hsel=_hform) m.DisableItems("5 1 3")
if(!__de_clipboard.cls.len) m.DisableItems("2")
action=m.Show(_hwnd)
if(!action) ret
 g1
int h=_hsel; if(!h) ret
if(action=2) if(!__de_clipboard.cls.len) ret
else if(h=_hform) ret

___DE_CLIPBOARD _x
___DE_CLIPBOARD& x=__de_clipboard

if action&1 ;;1 copy, 3 clone, 5 cut
	if(action&2) &x=_x
	___DE_CONTROL& c=subs.GetControl(h); x.c=c
	GetWinXY h 0 0 x.wid x.hei
	x.cls.getwinclass(h)
	sel x.cls
		case ["QM_Grid","QM_DlgInfo"] x.txt=c.txt
		case else x.txt.getwintext(h)
	x.style=GetWinStyle(h)
	x.exstyle=GetWinStyle(h 1)

if action&2 ;;2 paste, 3 clone
	_AddControl(-1 1 x)
else if action&4 ;;5 cut
	sub.Delete


#sub Delete c
if(!_hsel or _hsel=_hform) ret

_Undo

_ac.remove(subs.FindControl(_hsel))
DestroyWindow _hsel
_Select(_hform)


#sub Test c
_FormatDD(_s)
int f=2
if(_xDlg or _yDlg) f|0x100
ShowDialog(_s &sub.DlgProcTest 0 0 f 0 0 &this)


#sub DlgProcTest
function# hDlg message wParam lParam
sel message
	case WM_INITDIALOG
	__DE* d=+DT_GetParam(hDlg)
	DT_Page hDlg d._page d._pageMap
	case WM_COMMAND ret 1
