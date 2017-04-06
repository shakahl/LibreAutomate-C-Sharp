 \Dialog_Editor
function! [hwndOwner] [idCode] [capture] ;;capture: 1 drag-capture immediately

 Shows 'Find HTML element' dialog.
 Returns 1 on OK.

 idCode - if used, stores all code in it (caller must call __strt.ReplaceVar), else activates QM and inserts code in code editor.

str dd=
 BEGIN DIALOG
 0 "" 0x90C80A48 0x100 0 0 361 166 "Find HTML element"
 4 Static 0x54020003 0x4 2 2 16 16 "Dra" ".1 Drag and drop to capture object.[]Right click for more options."
 18 Static 0x54020000 0x4 46 6 32 12 "Window"
 10 QM_Tools 0x54030000 0x10000 80 4 276 14 "1 64"
 19 Button 0x54013003 0x4 0 26 26 13 "Name"
 11 Edit 0x54231044 0x204 28 26 236 22 "Nam"
 29 Button 0x54012003 0x4 28 50 36 13 "Use *" "Full or with wildcard characters *?.[]If unchecked, can be partial."
 31 Button 0x54012003 0x4 66 50 42 13 "Regexp." "Regular expression"
 33 Button 0x54032000 0x4 110 50 20 12 "RX" "Regular expression menu"
 25 ComboBox 0x54230243 0x4 142 50 70 213 "Nam" "What is Name.[]Can be text or an attribute of the element."
 9 Button 0x54032000 0x4 244 50 20 12 "<.>" "View in a text box"
 22 Button 0x54013003 0x4 0 70 26 13 "Html"
 14 Edit 0x54231044 0x204 28 70 238 22 "Htm"
 26 Button 0x54012003 0x4 28 94 36 13 "Use *" "Full or with wildcard characters *?.[]If unchecked, can be partial."
 32 Button 0x54012003 0x4 66 94 42 13 "Regexp." "Regular expression"
 34 Button 0x54032000 0x4 110 94 20 13 "RX" "Regular expression menu"
 12 Button 0x54032000 0x4 246 94 20 12 "<.>" "View in a text box"
 3 Static 0x54020000 0x4 282 26 18 12 "Tag"
 5 Edit 0x54030080 0x204 302 26 54 14 "Tag"
 21 Static 0x54020000 0x4 284 44 40 13 "Index"
 20 Edit 0x54030080 0x204 326 42 30 14 "Ind" "Index of this element in collection of Tag elements.[]Does not have to be exact, but exact or near index makes faster."
 13 Button 0x54013003 0x4 298 60 26 13 "Frame" "If unchecked, the element can be anywhere, but will be slower in multi-frame page"
 15 Edit 0x54030080 0x204 326 60 30 14 "Fra" "1-based index of frame/iframe. Or path to it, like 2/3.[]If empty or 0, the element must be not in a frame/iframe."
 8 Static 0x54020000 0x4 284 78 40 24 "Index in all elements"
 7 Edit 0x54030080 0x204 326 78 30 14 "Ind" "Index of this element in collection of all elements in page or frame.[]Normally it isn't used at run time. Now you can click < > to get adjacent element."
 42 Button 0x54032000 0x4 326 94 10 13 "<" "Previous"
 43 Button 0x54032000 0x4 336 94 10 13 "!" "Update"
 44 Button 0x54032000 0x4 346 94 10 13 ">" "Next"
 40 Static 0x54020000 0x4 4 120 46 12 "Wait max, s"
 45 Edit 0x54030080 0x204 52 118 22 14 "Wai"
 27 Button 0x54012003 0x4 92 120 70 12 "Error if not found" "End macro if not found, unless error handled with err.[]Click the arrow to see example."
 24 Button 0x54030000 0x0 164 121 16 10 "6"
 37 Edit 0x48000080 0x204 236 118 30 14 "Var" "Variable for the object. Optional. Will be created if need."
 23 Static 0x54020000 0x4 284 120 40 12 "Navigate"
 16 Edit 0x54030080 0x204 326 118 30 14 "Nav" "A positive or negative number that can be used to get an adjacent element"
 1 Button 0x54030001 0x4 4 148 48 14 "OK"
 2 Button 0x54030000 0x4 54 148 48 14 "Cancel"
 41 Button 0x54032000 0x4 104 148 48 14 "Test" "Test whether code created by this dialog will work, and how fast"
 28 Button 0x54032000 0x4 154 148 16 14 "?"
 6 Static 0x54020000 0x4 180 148 128 16 ""
 17 Button 0x54032000 0x0 310 148 48 14 "Options..."
 30 Static 0x54000010 0x20004 4 112 354 1 ""
 36 Static 0x54000010 0x20004 0 138 372 1 ""
 END DIALOG
 DIALOG EDITOR: "___EH_CONTROLS" 0x2040107 "" "" "" ""

type ___EH_CONTROLS __strt'controls __strt'si4Dra __strt'qmt10 __strt'c19Nam __strt'e11Nam __strt'c29Use __strt'c31Reg __strt'cb25Nam __strt'c22Htm __strt'e14Htm __strt'c26Use __strt'c32Reg __strt'e5Tag __strt'e20Ind __strt'c13Fra __strt'e15Fra __strt'e7Ind __strt'e45Wai __strt'c27Err __strt'e37Var __strt'e16Nav
___EH_CONTROLS d.controls="4 10 19 11 29 31 25 22 14 26 32 5 20 13 15 7 45 27 37 16"

d.si4Dra="&$qm$\target.ico"
d.cb25Nam="&text[]id[]name[]alt[]value[]type[]title[]href[]onclick[]src[]classid"
d.c13Fra=1
d.c27Err=1

int style; if(!hwndOwner and GetWinStyle(win 1)&WS_EX_TOPMOST) style|DS_SYSMODAL
if(!ShowDialog(dd &sub.DlgProc &d hwndOwner 0 style 0 capture)) ret

str s
if(!EH_Format(d s)) ret

if idCode
	act hwndOwner; err
	s.setwintext(id(idCode hwndOwner))
else
	__strt v.ReplaceVar(s)
	act _hwndqm; err
	InsertStatement s

ret 1


#sub DlgProc
function# hDlg message wParam lParam

type ___EH
	hwnd flags
	MSHTML.IHTMLDocument2'doc MSHTML.IHTMLElement'el
	__Settings'o

___EH- dH

if(sub_to.ToolDlgCommon(&hDlg "[]$qm$\web.ico[]EH_Main2" "" 1)) ret wParam
sel message
	case WM_INITDIALOG
	___EH e; dH=e
	dH.o.Init("defWait,Default wait,3" "Options" "\EH" 1)
	_s=dH.o.GetStr("defWait"); _s.setwintext(id(45 hDlg))
	sub_to.CheckboxPushlikeNoTheme hDlg
	TO_ButtonsAddArrowSimple hDlg "24"
	if(DT_GetParam(hDlg)&1) EH_Capture hDlg WM_LBUTTONDOWN
	
	case [WM_LBUTTONDOWN,WM_RBUTTONDOWN]
	if(GetDlgCtrlID(child(mouse))=4) EH_Capture hDlg message
	
	case WM_COMMAND goto messages2
ret
 messages2
TO_EditCheckOnChange hDlg 11 19  14 22
sel wParam
	case 41 EH_Test hDlg
	case 33 TO_Check(hDlg "31" 1); TO_Check(hDlg "29" 0); RegExpMenu id(11 hDlg)
	case 34 TO_Check(hDlg "32" 1); TO_Check(hDlg "26" 0); RegExpMenu id(14 hDlg)
	case 31 TO_Check(hDlg "29" 0)
	case 29 TO_Check(hDlg "31" 0)
	case 32 TO_Check(hDlg "26" 0)
	case 26 TO_Check(hDlg "32" 0)
	
	case 9 sub.Note hDlg 0
	case 12 sub.Note hDlg 1
	case [42,43,44] sub.Next hDlg wParam-43
	case 28 QmHelp "IDP_HTM"
	case 17 dH.o.OptionsDialog(hDlg)
	case 24 sub.ExampleNotFound hDlg
	
	case CBN_SELENDOK<<16|25
	_i=CB_SelectedItem(lParam)
	if(_i>0) sub.Attribute(dH.el _i _s); else _s=dH.el.outerText; err
	_s.setwintext(id(11 hDlg))
ret 1


#sub Note
function hDlg html

___EH- dH
if(!dH.el) ret
str s
if(html) s=dH.el.outerHTML
else
	int i=TO_Selected(hDlg 25)
	if(i<1) s=dH.el.outerText
	else sub.Attribute(dH.el i s)
ShowText "Html element" s hDlg 1
err+


#sub Attribute
function# MSHTML.IHTMLElement&el attr str&s

s.fix(0)
if(attr<1 or attr>10) ret 1
_s.gett("id name alt value type title href onclick src classid" attr-1)
s=el.getAttribute(_s 2)
err+


#sub Next
function hDlg plus

___EH- dH

int i=val(_s.getwintext(id(7 hDlg)))
i+plus
MSHTML.IHTMLElement el=dH.doc.all.item(i); err ret

EH_Fill hDlg dH.doc el 0 0 1


#sub ExampleNotFound
function hDlg
if(!ShowMenu("1 Show 'if not found' example" hDlg)) ret
_s=
 <><code> if 'Error if not found' is checked, you can optionally handle the error with err
 Htm e=htm(...)
 err ;;not found, or other error
 	out "not found"
 	ret
;
  if 'Error if not found' is unchecked, you can use if/else
 Htm e=htm(...)
 if e.NotFound
 	out "not found"
 	ret
 </code>
out _s
