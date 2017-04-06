 /dialog_test_QM_EditComboBox

 Registers and implements control window class "QM_EditComboBox".
 Similar to standard ComboBox controls. Supports images, check boxes (multiple selections), etc.
 To add a control to a dialog, in Dialog Editor click "Other controls", type "QM_EditComboBox", OK. ;;TODO

 Supports dialog variables to initialize controls when calling ShowDialog and to get control data on OK.
 To initialize control, use CSV string. It is not so difficult, see examples below.
   First row - control properties. Columns:
     1 - selected item index (can be -1) or checked items, depending on flags (see below). Can be empty.
     2 - imagelist. Can be:
       Imagelist file (if single line). To create imagelists you can use QM Imagelist Editor, look in floatng toolbar.
       List of icon files (if multiple lines or ends with a newline character), enclosed in double quotes.
       Imagelist handle (if a number). To load/create/delete an imagelist, you can use an __ImageList variable. The control does not delete the imagelist.
     3 - control flags:
       1 with check boxes. Use item flags to get/set check states. Set Edit text to the list of checked item labels. If the control created with ES_MULTILINE style, the list is multiline, else comma-separated.
       2 with check boxes. To get/set check states, use the first cell in the first column. It is a 32-bit number where bits of checked items are 1. Can be max 32 items. If also flag 1, sets Edit text to the list of checked item labels, else to the hexadecimal 32-bit number.
       4 sorted.
       0x100 for a dialog variable get CSV. Read more below.
     4 - cue banner (a gay text displayed when the Edit control is empty, not focused and is not multiline).
     5 - list text color in 0xBBGGRR format.
     6 - list background color in 0xBBGGRR format.
   Other rows - list items. Columns:
     1 - item text.
     2 - item image index in imagelist, or -1 for no image.
     3 - item flags:
         1 checked. If control flags has flag 1 (check boxes) and not flag 2, the control updates flag 1 of items with the new check states.
         2 no checkbox.
         4 disabled.
         8 bold font.
     4 - item tooltip text. Note that CSV supports multiple lines and commas in cell strings, they just must be enclosed in double quotes.
     5 - item text color in 0xBBGGRR format.
     6 - item background color in 0xBBGGRR format.
     7 - item indent.
   Example CSV strings:
     Simplest: "0[]Mercury[]Venus[]Earth". Adds three items and selects Mercury (index 0). To initialize a standard ComboBox like this, you would use "&Item1[]Item2[]Item3".
     With icons: "0,$my qm$\imagelist.bmp[]Mercury,0[]Venus,1[]Earth,2". Adds three items, selects Mercury, and in the drop-down list displays the first three images from the imagelist file.
     With checkboxes, using item flags to check: ",,1[]Mercury,,1[]Venus,,0[]Earth". Adds three items, checks Mercury (its item flags contain 1), and sets Edit text to "Mercury".
     With checkboxes, using bits to check items: "0x5,,3[]Mercury[]Venus[]Earth". Adds three items, checks Mercury and Earth (first and third bits in 0x5 are 1), and sets Edit text to "Mercury, Earth". On OK the variable would be "0x5" if not changed; if for example the user checked or typed just Venus, on OK the variable would be "0x2".
   Note that it must be valid CSV. In CSV, strings containing special characters (comma (,), double quote ("), new line) must be enclosed in double quotes. Double quotes in strings must be replaced with two double quotes. To create valid CSV at run time, use an <help>ICsv</help> variable.

 To get data from controls of this class, you can use dialog variables. The format depends on control flag 0x100:
 By default it is Edit text.
 If control flag 0x100 used, it is initial CSV string, modified depending on user-made selection:
   If without check boxes: CSV cell 0 0 is the selected item index.
   Else (if with checkboxes):
     If used control flag 2: CSV cell 0 0 is the 32-bit number containing bits for checked/unchecked items (read more above).
     Else: item flags of checked items have bit 1. Use an ICsv variable to get item flags.

 If you want to get control data in dialog procedure, use function DT_GetControl. Function str.getwintext gets Edit text.
 If you want to set control data in dialog procedure, use function DT_SetControl or str.setwintext. Function EditReplaceSel changes Edit text.
 When the user or EditReplaceSel changes Edit text, the control parses the new text and updates selected/checked item states in its drop-down list.

 The Edit part of the control is based on standard Edit control and supports all its features.
 Supports all Edit control messages (EM_... and other) and notifications (WM_COMMAND/EN_... and other).
 Supports all Edit control styles. For example, can be multiline (ES_MULTILINE), read-only (ES_READONLY). If you need true read-only combo box, instead use class "QM_ReadonlyComboBox".
 The drop-down list part is a virtual listview control managed by this control.

 Other features:
 If control width is less than height of two list items, shows the list like a menu: calculates its width from item texts and shows at the mouse pointer position. For example if control width is equal to the arrow button width.
 Incremental search. When the user changes Edit text while the drop-down list is shown, the control highlights the first item whose text begins with or contains Edit text. Use Enter to select it.
 Unlike standard ComboBox control, this control does not select Edit text when focused with the mouse or selected an item from the list. However selects before showing the drop-down list, need it for incremental search.
 The Edit part of this control can be multiline (style ES_MULTILINE), any height, with scrollbars (styles WS_VSCROLL, WS_HSCROLL, WS_EX_LEFTSCROLLBAR).
 Fast, supports large number of items.
 Closes the drop-down list when the user moves the mouse >100 pixels from the list+Edit or clicks the Edit control or its dialog. Then the selected item is not cahnged, unless the list is with checkboxes.
 Middle-click to clear Edit text.
 Keyboard shortcuts in Edit:
   Down/Up/PgDn/PgUp - show drop-down list. Need Alt if multiline.
 Keyboard shortcuts in drop-down list:
   Down/Up/PgDn/PgUp - highlight items.
   Enter or Tab - select the highlighted item and close the list.
   Esc or Alt - close the list without selecting.
 To show drop-down list, the control uses function .... You can use the function directly if don't need a combo box control.

 TODO: add a message to get current CSV. Eg to get checkboxes.
 TODO: Add "item selected" notification. Can be WM_COMMAND/CBN_SELENDOK. If need data, can use DT_GetControl or a message to get CSV.
 TODO: Add a "checked/unchecked" notification when checking. It allows to correct bad flag combinations.
 TODO: add message to set drop-down RECT.


 The Edit control must not know about these styles. We reserve them for the future, or they are incompatible.
 0x200 (unused), 0x400 (es_oemconvert), 0x4000 (unused), 0x8000 (unused), wm_thickframe, wm_dlgframe.
def ___QMECBS_CLEAR 0x44C600

function# message wParam lParam

#region outwinmsg
 #compile "__Indent"
 Indent ind; str si1 si2
 int+ g_indent; if(message=WM_NCCREATE) g_indent=0
 OutWinMsg message wParam lParam &si1; out si2.format("%.*m%s" g_indent 9 si1)
 ind.Add(&g_indent)
 OutWinMsg message wParam lParam
#endregion

sel message
	case WM_NCCREATE sub.OnNcCreate(+lParam)
	case WM_NCCALCSIZE ret sub.OnNcCalcSize(wParam lParam) ;;adds nonclient area for our button
	case WM_STYLECHANGING sub.OnStyleChanging(wParam +lParam)
	case WM_SETTEXT ret sub.OnSetText(wParam +lParam)
	case [WM_KEYDOWN,WM_SYSKEYDOWN] if(sub.OnKeyDown(message wParam lParam)) ret
	case WM_SETCURSOR if(wParam=_hwnd and sub.OnSetCursor) ret 1 ;;set arrow cursor if in nonclient, because we returned HTCLIENT on nchittest
	case WM_LBUTTONDOWN if(sub.IsCursorInButton(lParam)) ret sub.ShowPopup
	case WM_RBUTTONDOWN if(sub.IsCursorInButton(lParam)) ret ;;disable the context menu if on our button
	case WM_MOUSELEAVE ret sub.OnMouseMove(1) ;;manages hot state painting; don't pass to the base wndproc because its theming interferes with ours
	case WM_NCPAINT ret _Paint(1 wParam lParam) ;;don't pass to the base wndproc, because we called defwndproc if need
	case WM_NOTIFY ret sub.OnNotify(+lParam)
	case [WM_CHAR,WM_PASTE,WM_CUT,WM_UNDO,EM_REPLACESEL,EM_UNDO] ret sub.OnTextChange(message wParam lParam)
	case WM_MBUTTONDOWN EditReplaceSel _hwnd 0 "" 3; ret
	case else
	if(message=_WM_QM_GETDIALOGVARIABLEDATA) ret sub.GetDialogVariableData(wParam +lParam)

int R=_BaseProc(message wParam lParam)

 OutWinMsg message wParam lParam

sel message
	case WM_CREATE sub.OnCreate(+lParam)
	case WM_DESTROY sub.OnDestroy
	case WM_THEMECHANGED _Theme(WM_THEMECHANGED)
	case WM_SETFOCUS if(!_isFocused) _isFocused=1; _Paint ;;manages focused state painting
	case WM_KILLFOCUS if(_isFocused) _isFocused=0; _Paint ;;manages focused state painting
	case WM_MOUSEMOVE sub.OnMouseMove(0 lParam) ;;manages hot state painting, together with WM_MOUSELEAVE which is handled above
	case WM_NCHITTEST sel(R) case [HTVSCROLL,HTHSCROLL] case else ret HTCLIENT ;;HTCLIENT because all other codes cause Windows to do something; also, now we'll receive client mouse messages, including wm_mouseleave when leaving window rect. But it would disable scrollbars.

ret R


#sub OnNcCalcSize c
function wParam lParam
 Adds nonclient space at the right, to draw our button.
 wm_nccalcsize is sent after wm_nccreate (wParam=0) and later (wParam=1) on each resizing, style change etc.

  debug
 out "wParam=%i" wParam
 NCCALCSIZE_PARAMS* p=+lParam; int i
 for(i 0 iif(wParam 3 1)) outRECT p.rgrc[i]
 int R=_BaseProc(WM_NCCALCSIZE wParam lParam)
 for(i 0 iif(wParam 3 1)) outRECT p.rgrc[i]
 out "R=%i" R
  Results:
    The base proc always just sets the first rect. Probably just calls defwindowproc. Returns 0.
    If wParam is 0, coords are screen, else parent client.
    When wParam is 1, getrects/getwindowrect give old rect.

if(wParam and _winver>=0x600 and _animate) BufferedPaintStopAllAnimations _hwnd

RECT& r=+lParam
RECT k=r
_BaseProc(WM_NCCALCSIZE wParam lParam)
int bl(r.left-k.left) br(k.right-r.right); _borderWidth=iif(br<bl br bl) ;;get minimal border width, because can have a scrollbar, which can be at the right or left
if(_borderWidth>2) _borderWidth=2
r.right-_GetButtonWidth-_borderWidth ;;-_borderWidth because we draw our button on the right border too


#sub OnMouseMove c
function !onLeave [xyClient]
int prevIn=_isMouseIn
if onLeave
	if(!_isMouseIn) ret
	_isMouseIn=0
else
	_isMouseIn=iif(sub.IsCursorInButton(xyClient) 2 1)
	if(!prevIn) TRACKMOUSEEVENT tm.cbSize=sizeof(tm); tm.dwFlags=TME_LEAVE; tm.hwndTrack=_hwnd; TrackMouseEvent &tm

if(_isMouseIn!=prevIn) if(!(_isFocused and _isMouseIn^prevIn=1)) _Paint ;;paint when _isMouseIn changed, except when mouse moved into/outof the non-button area while focused
prevIn=_isMouseIn
 info: disabled windows don't receive mouse messages, it's good


#sub OnSetCursor c
function#
 If nonclient area, sets arrow cursor and returns 1.

POINT p; xm p _hwnd 1
RECT r; GetClientRect _hwnd &r
if(PtInRect(&r p.x p.y)) ret
SetCursor LoadCursor(0 +IDC_ARROW)
ret 1


#sub IsCursorInButton c
function# xyClient

POINT p.x=ConvertSignedUnsigned(xyClient&0xffff 2); p.y=ConvertSignedUnsigned(xyClient>>16 2)
RECT r; GetWindowRect _hwnd &r; POINT c=p; ClientToScreen _hwnd &c; p.x=c.x-r.left; p.y=c.y-r.top ;;client to window
RECT rb; _GetRects(rb)
ret PtInRect(&rb p.x p.y)


#sub OnNcCreate c
function# CREATESTRUCTW&c

int style=c.style
if(style&(WS_VSCROLL|WS_HSCROLL)) _styleScrollbars=1
c.style~___QMECBS_CLEAR; if(c.style!=style) _noMessages=2; SetWinStyle _hwnd c.style; _noMessages=0
_style=style

_csv._create
_csv.FromString("-1")


#sub OnStyleChanging c
function wParam STYLESTRUCT&ss
_style=ss.styleNew
ss.styleNew~___QMECBS_CLEAR


#sub OnCreate c
function# CREATESTRUCTW&c

 tested with richedit:
 _BaseProc(EM_SETEDITSTYLE SES_EMULATESYSEDIT SES_EMULATESYSEDIT) ;;don't paste rich text. But sets single-level undo, that undoes just SINGLE last typed character.
 _BaseProc(EM_SETTEXTMODE TM_PLAINTEXT|TM_MULTILEVELUNDO|TM_MULTICODEPAGE 0) ;;don't paste rich text. But sets multiline-level undo, where each Undo undoes just SINGLE typed character.

 err+ out _error.description


#sub OnDestroy c
function#
_Theme(WM_DESTROY)


#sub OnSetText c
function# wParam word*w
 Parses text as CSV to _csv. Sets Edit text to match the selected/checked items in it.

if(_isPressed) ret
_flags=0
int notCsv
if(empty(w)) notCsv=1; else _s.ansi(w)
if(!notCsv) _csv.FromString(_s)
err+ notCsv=1
if(notCsv) _csv.FromString("-1"); _BaseProc(WM_SETTEXT 0 L"")
if(_csv.ColumnCount>2) _flags=_csv.CellInt(0 2)
lpstr s
if(_csv.ColumnCount>3) s=_csv.Cell(0 3); if(!empty(s)) _BaseProc(EM_SETCUEBANNER 0 @s)

sub.SetSelectedItem(1)


#sub GetDialogVariableData c
function# wParam str&s

if _flags&0x100
	sub.EnsureCsvUpdated
	_csv.ToString(s)
else s.getwintext(_hwnd)

ret 1


#sub SetSelectedItem c
function# [action] [iSelected] ;;action: 0 select (use iSelected), 1 on settext (iSelected not used), 2 on drop-down ok (iSelected not used, CSV updated)

sel(action) case [0] sub.EnsureCsvUpdated
str s
if(!_GetSelectedItemText(s action iSelected)) ret

_noMessages=1 ;;EM_REPLACESEL would be used by OnTextChange
EditReplaceSel _hwnd 0 s 1
_noMessages=0


#sub ShowPopup c
if(_isPressed or _csv.RowCount<2) ret

 TODO: notify parent

_isPressed=1
_isFocused=1; SetFocus _hwnd ;;isFocused=1 to prevent calling sub.paint on wm_setfocus
if(_flags&3=0) _BaseProc(EM_SETSEL 0 -1) ;;for incremental search
_Paint
sub.EnsureCsvUpdated
__QM_ComboList x
int R=x.Show(_csv _hwnd 0 _hlv)
 out R
_isPressed=0
_Paint
if(R<0) ret
sub.SetSelectedItem(2)
_textChanged=0

 TODO: notify parent


#sub OnKeyDown c
function# message wParam lParam
 If processes the key (up/down arrows etc), returns 1.

sel(wParam) case [VK_DOWN,VK_UP,VK_PRIOR,VK_NEXT] if(_style&ES_MULTILINE=0 or message=WM_SYSKEYDOWN) sub.ShowPopup; ret 1
if message=WM_KEYDOWN
	sel wParam
		case VK_DELETE sub.OnTextChange(message wParam lParam); ret 1


#sub OnTextChange c
function# message wParam lParam
 We call this on various messages that can change Edit text.
 We call basewndproc and set _textChanged if text changed. Later use EnsureCsvUpdated, it updates CSV from changed Edit text, if _textChanged.

int incrementalSearch=_isPressed and _flags&3=0 and IsWindow(_hlv)
if(_textChanged and !incrementalSearch) ret _BaseProc(message wParam lParam)
 OutWinMsg message wParam lParam
str s1 s2
s1.getwintext(_hwnd)
int R=_BaseProc(message wParam lParam)
s2.getwintext(_hwnd)
if(s2=s1) ret R
 out "changed"
_textChanged=1
 incremental search
if(!incrementalSearch) ret R
int i=_csv.Find(s2 1 0 1); if(i<0) i=_csv.Find(s2 5 0 1); if(i<0) i=_csv.Find(s2 17 0 1) ;;exact, the beginning, then anywhere
if(i>0) i-1
SendMessage GetParent(_hlv) CB_SETCURSEL 0 i
ret R


#sub EnsureCsvUpdated c
function#
 If Edit text changed since start or prev call to EnsureCsvUpdated, parses Edit text and updates CSV. Returns 1 if actually updated.
 Uses and resets _textChanged.

if(!_textChanged) ret
_textChanged=0
str s.getwintext(_hwnd)
int i j n=_csv.RowCount
int oldValue=_csv.CellInt(0 0)

 TODO: skip disabled
sel _flags&3
	case 0
	i=_csv.Find(s 1 0 1); if(i<0) i=_csv.Find(s 5 0 1); if(i<0) i=_csv.Find(s 17 0 1) ;;exact, the beginning, then anywhere
	if(i>0) i-1
	if(i!=oldValue) _csv.CellInt(0 0)=i; ret 1
	
	case 2
	i=val(s)
	if(i!=oldValue) _csv.CellHex(0 0)=i; ret 1
	
	case else
	ARRAY(str) a
	tok s a -1 "," 0x2000
	lpstr k; int check itemFlags checkedState itemFlagsChanged
	if(_flags&2 and n>33) n=33
	for i 1 n
		k=_csv.Cell(i 0)
		for(j 0 a.len) if(!StrCompare(k a[j] 1)) break
		check=j<a.len
		if _flags&2
			if(check) checkedState|1<<(i-1)
		else
			itemFlags=_csv.CellInt(i 2)
			if(check!=itemFlags&1) _csv.CellInt(i 2)=itemFlags^1; itemFlagsChanged=0
	
	if(_flags&2) if(checkedState!=oldValue) _csv.CellHex(0 0)=checkedState; ret 1
	else ret itemFlagsChanged


#sub OnNotify c
function NMHDR*nh



#sub Notify c
function code
 info: Edit and ComboBox notification codes don't overlap

SendMessageW(GetParent(_hwnd) WM_COMMAND MakeInt(GetWinId(_hwnd) code) _hwnd)
