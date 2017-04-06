 /Dialog162

 Registers and implements control window class "QM_ComboBox".
 Similar to standard ComboBox controls. Supports images, check boxes (multiple selections), etc.
 To add a control to a dialog, in Dialog Editor click "Other controls", type "QM_ComboBox", OK. ;;TODO

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

 To get data from controls of this class, you can use dialog variables. The format depends on control flags and style:
 By default it is:
   If the control is editable (without ES_READONLY style): Edit text.
   Else if without checkboxes: selected item index (or -1 if not selected) followed by Edit text, like "0 First item". Use function <help>val</help> to convert to int.
   Else (if with checkboxes):
     If used control flag 2: the 32-bit value (read more above).
     Else: a string consisting of '1' characters for checked items and '0' characters for unchecked. For example, if there are 3 items and only the first item is checked, gets "100".
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
 Supports all Edit control styles. For example, can be read-only (ES_READONLY) or multiline (ES_MULTILINE).
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
 TODO: don't clear styles.
 TODO: hybrid style. Some items are with checkboxes, some not. When a non-checkbox clicked, the list func returns its index. Returns -1 on cancel. The Edit control behaves as with checkboxes.
 TODO: maybe need to make tooltip topmost.

#region init
__RegisterWindowClass+ __rwcQMCB
if(__rwcQMCB.atom) ret
__rwcQMCB.Superclass("Edit" "QM_ComboBox" &sub.WndProc_Superclass 4)
 __rwcQMCB.Superclass("RichEdit20W" "QM_ComboBox" &sub.WndProc_Superclass 4)
QmSetWindowClassFlags "QM_ComboBox" 1|8 ;;flags: 1 use dialog variables, 2 use dialog definition text, 4 disable text editing in Dialog Editor, 8 supports "WM_QM_GETDIALOGVARIABLEDATA"
#endregion


#sub WndProc_Superclass
function# hwnd message wParam lParam

 The Edit control must not know about these styles. We reserve them for the future, or they are incompatible.
 0x200 (unused), 0x400 (es_oemconvert), 0x4000 (unused), 0x8000 (unused), wm_thickframe, wm_dlgframe.
def ___QMCBS_CLEAR 0x44C600

type ___QM_ComboBox
	hwnd theme hlv
	!themeInited !isFocused !isMouseIn !isPressed !noMessages !animate !styleScrollbars !textChanged !showFocus
	style borderWidth stateF stateB animTime
	ICsv'csv flags WM_QM_GETDIALOGVARIABLEDATA

___QM_ComboBox* d
if message=WM_NCCREATE
	SetWindowLong hwnd __rwcQMCB.baseClassCbWndExtra d._new
	d.hwnd=hwnd
else
	d=+GetWindowLong(hwnd __rwcQMCB.baseClassCbWndExtra)
	if(!d) ret sub.CallBase(hwnd message wParam lParam)
	if(d.noMessages) ret iif(d.noMessages=2 DefWindowProc(hwnd message wParam lParam) sub.CallBase(hwnd message wParam lParam))

#region outwinmsg
 #compile "__Indent"
 Indent ind; str si1 si2
 int+ g_indent; if(message=WM_NCCREATE) g_indent=0
 OutWinMsg message wParam lParam &si1; out si2.format("%.*m%s" g_indent 9 si1)
 ind.Add(&g_indent)
 OutWinMsg message wParam lParam
#endregion

sel message
	case WM_NCCREATE sub.OnNcCreate(d +lParam)
	case WM_NCCALCSIZE ret sub.OnNcCalcSize(d wParam lParam) ;;adds nonclient area for our button
	case WM_STYLECHANGING sub.OnStyleChanging d wParam +lParam
	case WM_SETTEXT ret sub.OnSetText(d wParam +lParam)
	case [WM_KEYDOWN,WM_SYSKEYDOWN] if(sub.OnKeyDown(d message wParam lParam)) ret
	case WM_SETCURSOR if(wParam=hwnd and sub.OnSetCursor(d)) ret 1 ;;set arrow cursor if in nonclient, because we returned HTCLIENT on nchittest
	case WM_LBUTTONDOWN if(d.style&ES_READONLY or sub.IsCursorInButton(d lParam)) ret sub.ShowPopup(d)
	case WM_RBUTTONDOWN if(sub.IsCursorInButton(d lParam)) ret ;;disable the context menu if on our button
	case WM_MOUSELEAVE ret sub.OnMouseMove(d 1) ;;manages hot state painting; don't pass to the base wndproc because its theming interferes with ours
	case WM_NCPAINT ret sub.Paint(d 1 wParam lParam) ;;don't pass to the base wndproc, because we called defwndproc if need
	case WM_NOTIFY ret sub.OnNotify(d +lParam)
	case [WM_CHAR,WM_PASTE,WM_CUT,WM_UNDO,EM_REPLACESEL,EM_UNDO] ret sub.OnTextChange(d message wParam lParam)
	case WM_MBUTTONDOWN EditReplaceSel d.hwnd 0 "" 3; ret
	case WM_UPDATEUISTATE if(wParam&0x10000) d.showFocus=wParam&2
	case [WM_SETFOCUS,WM_KILLFOCUS] if(d.style&ES_READONLY) goto afterBase ;;don't need caret etc; we'll draw focus rect if need
	case WM_PAINT if(sub.PaintReadonlyClient(d)) ret
	case else
	if(message=d.WM_QM_GETDIALOGVARIABLEDATA) ret sub.GetDialogVariableData(d wParam +lParam)

int R=sub.CallBase(hwnd message wParam lParam)

 OutWinMsg message wParam lParam

 afterBase
sel message
	case WM_CREATE sub.OnCreate(d +lParam)
	case WM_DESTROY sub.OnDestroy(d)
	case WM_THEMECHANGED sub.Theme d WM_THEMECHANGED
	case WM_SETFOCUS if(!d.isFocused) d.isFocused=1; sub.Paint(d) ;;manages focused state painting
	case WM_KILLFOCUS if(d.isFocused) d.isFocused=0; sub.Paint(d) ;;manages focused state painting
	case WM_MOUSEMOVE sub.OnMouseMove(d 0 lParam) ;;manages hot state painting, together with WM_MOUSELEAVE which is handled above
	case WM_NCHITTEST sel(R) case [HTVSCROLL,HTHSCROLL] case else ret HTCLIENT ;;HTCLIENT because all other code cause Windows to do something; also, now we'll receive client mouse messages, including wm_mouseleave when leaving window rect. But it would disable scrollbars.

if(message=WM_NCDESTROY) d._delete
ret R


#sub OnNcCalcSize
function ___QM_ComboBox&d wParam lParam
 Adds nonclient space at the right, to draw our button.
 wm_nccalcsize is sent after wm_nccreate (wParam=0) and later (wParam=1) on each resizing, style change etc.

  debug
 out "wParam=%i" wParam
 NCCALCSIZE_PARAMS* p=+lParam; int i
 for(i 0 iif(wParam 3 1)) outRECT p.rgrc[i]
 int R=sub.CallBase(d.hwnd WM_NCCALCSIZE wParam lParam)
 for(i 0 iif(wParam 3 1)) outRECT p.rgrc[i]
 out "R=%i" R
  Results:
    The base proc always just sets the first rect. Probably just calls defwindowproc. Returns 0.
    If wParam is 0, coords are screen, else parent client.
    When wParam is 1, getrects/getwindowrect give old rect.

if(wParam and _winver>=0x600 and d.animate) BufferedPaintStopAllAnimations d.hwnd

RECT& r=+lParam
RECT k=r
sub.CallBase(d.hwnd WM_NCCALCSIZE wParam lParam)
int bl(r.left-k.left) br(k.right-r.right); d.borderWidth=iif(br<bl br bl) ;;get minimal border width, because can have a scrollbar, which can be at the right or left
if(d.borderWidth>2) d.borderWidth=2
r.right-sub.GetButtonWidth(d)-d.borderWidth ;;-d.borderWidth because we draw our button on the right border too


#sub Paint
function ___QM_ComboBox&d [!onWmNcPaint] [wParam] [lParam]

type __QMCB_PAINT !onWmNcPaint wParam lParam RECT'r RECT'rb RECT're
__QMCB_PAINT p.onWmNcPaint=onWmNcPaint; p.wParam=wParam; p.lParam=lParam

sub.GetRects(d p.rb p.r p.re)
sub.PaintScrollbars(d p)

__Hdc dc=GetWindowDC(d.hwnd)
ExcludeClipRect dc p.re.left p.re.top p.re.right p.re.bottom

if !d.theme
	if(d.borderWidth) DrawEdge dc &p.r EDGE_SUNKEN BF_RECT
	int state; if(d.style&WS_DISABLED) state=DFCS_INACTIVE; else if(d.isPressed) state=DFCS_FLAT|DFCS_HOT; else if(d.isMouseIn=2) state=DFCS_HOT
	DrawFrameControl dc &p.rb DFC_SCROLL DFCS_SCROLLDOWN|state
	 sub.PaintFocusRect(d p dc)
	ret

 frame
int stateF=CBB_NORMAL
if(d.style&WS_DISABLED) stateF=CBB_DISABLED; else if(d.isFocused) stateF=CBB_FOCUSED; else if(d.isMouseIn) stateF=CBB_HOT
if(!d.borderWidth) p.r.top-1; p.r.right+1; p.r.bottom+1 ;;don't draw border but draw background (need for transparent button)
 button
int stateB=CBXS_NORMAL
if(stateF=CBB_DISABLED) stateB=CBXS_DISABLED; else if(d.isPressed) stateB=CBXS_PRESSED; else if(d.isMouseIn=2) stateB=CBXS_HOT

if !sub.PaintAnimated(d p dc stateF stateB)
	 out "no anim, onWmNcPaint=%i" onWmNcPaint
	sub.PaintThemed(d p dc stateF stateB)

d.stateF=stateF; d.stateB=stateB


#sub PaintAnimated
function# ___QM_ComboBox&d __QMCB_PAINT&p dc stateF stateB
 Returns 1 if painted.

 use animation?
if(!d.animate and _winver>=0x600 and d.theme and !BufferedPaintInit) d.animate=1
if(!d.animate) ret

if(p.onWmNcPaint) if(BufferedPaintRenderAnimation(d.hwnd dc)) ret 1
else if(stateB=d.stateB and stateF=d.stateF) ret 1

int transDuration ;;tested: buton times are the same as frame times.
if(stateB!=d.stateB) GetThemeTransitionDuration(d.theme CP_BORDER d.stateB stateB TMT_TRANSITIONDURATIONS &transDuration)
else GetThemeTransitionDuration(d.theme CP_DROPDOWNBUTTONRIGHT d.stateF stateF TMT_TRANSITIONDURATIONS &transDuration)

 when state changes while in a middle of previous animation, get the first image from window DC instead of drawing old state bitmap. We cannot get current buff animation DC, there is no API.
if(transDuration>600) transDuration=600
int timeNow=GetTickCount
if(d.animTime>timeNow and !p.onWmNcPaint) int bitblt=1 ;;prev animation not ended
d.animTime=timeNow+transDuration
 out "%i %i %i %i    %i  bitblt=%i" d.stateF stateF d.stateB stateB transDuration bitblt

BP_PAINTPARAMS paintp.cbSize=sizeof(paintp); paintp.dwFlags=BPPF_NONCLIENT; paintp.prcExclude=&p.re
BP_ANIMATIONPARAMS animp.cbSize=sizeof(animp); animp.style=BPAS_LINEAR; animp.dwDuration=transDuration
int hdcFrom hdcTo
int hbpAnimation=BeginBufferedAnimation(d.hwnd dc &p.r BPBF_COMPATIBLEBITMAP &paintp &animp &hdcFrom &hdcTo)
if(!hbpAnimation) ret
 out "hdcFrom=%i hdcTo=%i" hdcFrom hdcTo
if hdcFrom
	if(bitblt) BitBlt hdcFrom 0 0 p.r.right p.r.bottom dc 0 0 SRCCOPY
	else sub.PaintThemed(d p hdcFrom d.stateF d.stateB 1)
if(hdcTo) sub.PaintThemed(d p hdcTo stateF stateB)
EndBufferedAnimation(hbpAnimation 1)
ret 1


#sub PaintThemed
function ___QM_ComboBox&d __QMCB_PAINT&p dc stateF stateB [!noParent]

if _winver>=0x600 and !noParent and IsThemeBackgroundPartiallyTransparent(d.theme CP_BORDER stateF) ;;draw rounded corners on Win7
	DrawThemeParentBackgroundEx(d.hwnd dc DTPB_WINDOWDC|DTPB_USECTLCOLORSTATIC &p.r) ;;without DTPB_USECTLCOLORSTATIC error "Incorrect function", even whem animation disabled (it draws to a memory DC); DrawThemeParentBackground too; but even then draws correctly
DrawThemeBackground(d.theme dc CP_BORDER stateF &p.r 0) ;;frame
DrawThemeBackground(d.theme dc iif((_winver<0x600 or d.style&(ES_READONLY|ES_MULTILINE)) CP_DROPDOWNBUTTON CP_DROPDOWNBUTTONRIGHT) stateB &p.rb 0) ;;button
 sub.PaintFocusRect(d p dc)


#sub PaintScrollbars
function ___QM_ComboBox&d __QMCB_PAINT&p
 Calls defwndproc wm_ncpaint if need, excluding our frame/button region from its update region.
 In our wndproc we never pass wm_ncpaint to the base wndproc, because it interferes with our nonclient drawing and causes flickering.
 Instead call this func before actually drawing our frame/button, to let it draw scrollbars if need.

if(!(p.onWmNcPaint and d.styleScrollbars)) ret

 out p.wParam
RECT r re=p.re
GetWindowRect d.hwnd &r; OffsetRect &re r.left r.top
__GdiHandle hrgn=CreateRectRgnIndirect(&re)
if(p.wParam!1 and CombineRgn(hrgn hrgn p.wParam RGN_AND)=1) ret ;;NULLREGION, usually when processing our animated paint

DefWindowProcW(d.hwnd WM_NCPAINT hrgn 0)
 0.1 ;;test flickering


#sub PaintReadonlyClient
function# ___QM_ComboBox&d
if(d.style&ES_READONLY=0 or !d.theme) ret
PAINTSTRUCT ps; int dc=BeginPaint(d.hwnd &ps)

 TODO: test disabled
 out "paint"
RECT r; GetClientRect d.hwnd &r; InflateRect &r d.borderWidth d.borderWidth
int state=CBRO_NORMAL; if(d.style&WS_DISABLED) state=CBRO_DISABLED; else if(d.isPressed) state=CBRO_PRESSED; else if(d.isMouseIn) state=CBRO_HOT
out state

DrawThemeBackground(d.theme dc CP_READONLY state &r 0)
 DrawThemeBackground(d.theme dc iif(_winver<0x600 CP_DROPDOWNBUTTON CP_DROPDOWNBUTTONRIGHT) stateB &p.rb 0)

EndPaint d.hwnd &ps
ret 1


 #sub PaintFocusRect
 function ___QM_ComboBox&d __QMCB_PAINT&p dc
 if d.style&ES_READONLY and d.isFocused and d.showFocus and d.borderWidth=2
	 RECT rf=p.re; InflateRect &rf 1 1
	 DrawFocusRect dc &rf


#sub OnMouseMove
function ___QM_ComboBox&d !onLeave [xyClient]
int prevIn=d.isMouseIn
if onLeave
	if(!d.isMouseIn) ret
	d.isMouseIn=0
else
	d.isMouseIn=iif(sub.IsCursorInButton(d xyClient) 2 1)
	if(!prevIn) TRACKMOUSEEVENT tm.cbSize=sizeof(tm); tm.dwFlags=TME_LEAVE; tm.hwndTrack=d.hwnd; TrackMouseEvent &tm

if(d.isMouseIn!=prevIn) if(!(d.isFocused and d.isMouseIn^prevIn=1)) sub.Paint(d) ;;paint when d.isMouseIn changed, except when mouse moved into/outof the non-button area while focused
prevIn=d.isMouseIn
 info: disabled windows don't receive mouse messages, it's good


#sub OnSetCursor
function# ___QM_ComboBox&d
 If nonclient area or readonly, sets arrow cursor and returns 1.

if d.style&ES_READONLY=0
	POINT p; xm p d.hwnd 1
	RECT r; GetClientRect d.hwnd &r
	if(PtInRect(&r p.x p.y)) ret
SetCursor LoadCursor(0 +IDC_ARROW)
ret 1


#sub IsCursorInButton
function# ___QM_ComboBox&d xyClient

POINT p.x=ConvertSignedUnsigned(xyClient&0xffff 2); p.y=ConvertSignedUnsigned(xyClient>>16 2)
RECT r; GetWindowRect d.hwnd &r; POINT c=p; ClientToScreen d.hwnd &c; p.x=c.x-r.left; p.y=c.y-r.top ;;client to window
RECT rb; sub.GetRects(d rb)
ret PtInRect(&rb p.x p.y)


#sub GetRects
function ___QM_ComboBox&d [RECT&rButton] [RECT&rWindow] [RECT&rClient]
 rButton - button rect in window.
 rWindow - window rect in window (left and top are 0). Other rects are relative to it.
 rClient - client area in window. Includes scrollbars etc, ie the area that we don't draw. We draw only border and button.

RECT r rs rb
GetWindowRect d.hwnd &rs
r=rs; OffsetRect &r -r.left -r.top
int cxButton=sub.GetButtonWidth(d) ;;also inits theme if need
int inflate; if(!d.theme) inflate=-d.borderWidth; else if(!d.borderWidth) inflate=1; else if(_winver<0x600) inflate=-1
rb=r; InflateRect &rb inflate inflate; rb.left=r.right-cxButton
if(&rClient) rClient=r; InflateRect &rClient -d.borderWidth -d.borderWidth; rClient.right=rb.left
if(&rButton) rButton=rb
if(&rWindow) rWindow=r


#sub GetButtonWidth
function# ___QM_ComboBox&d
 Returns button width from the right edge of the window.

if(!d.themeInited) d.themeInited=1; sub.Theme d WM_CREATE
int R=GetSystemMetrics(SM_CXVSCROLL)
if(!d.theme) R+d.borderWidth; else if(!d.borderWidth) R-1; else if(_winver<0x600) R+1
ret R

 tested: it seems that combo button width is always calculated from SM_CXVSCROLL. GetThemePartSize gets inorrect results. All other theme metrics API fail or get 0.
 RECT k kk; SIZE z
 if(!GetThemePartSize(d.theme dc CP_DROPDOWNBUTTON CBXS_NORMAL &r TS_TRUE &z)) out "%i %i" z.cx z.cy ;;7 21
 if(!GetThemePartSize(d.theme dc CP_DROPDOWNBUTTONLEFT CBXS_NORMAL &r TS_TRUE &z)) out "%i %i" z.cx z.cy ;;6 23
 if(!GetThemePartSize(d.theme dc CP_DROPDOWNBUTTONRIGHT CBXS_NORMAL &r TS_TRUE &z)) out "%i %i" z.cx z.cy ;;5 23
 if(!GetThemeBackgroundContentRect(d.theme dc CP_DROPDOWNBUTTON CBXS_NORMAL &r &k)) outRECT k ;;144 20
 if(!GetThemeTransitionDuration(d.theme CP_DROPDOWNBUTTON CBXS_NORMAL CBB_HOT TMT_TRANSITIONDURATIONS &_i)) out _i ;;225
  these fail
 if(!GetThemeRect(d.theme CP_DROPDOWNBUTTON CBXS_NORMAL TMT_RECT &k)) outRECT k
 if(!GetThemeMetric(d.theme dc CP_DROPDOWNBUTTON CBXS_NORMAL TMT_WIDTH &_i)) out _i


#sub Theme
function ___QM_ComboBox&d message ;;message: wm_create, wm_themechanged, wm_destroy

if(_winver<0x501) ret
d.stateF=CBB_NORMAL; d.stateB=CBXS_NORMAL
if(_winver>=0x600 and d.animate) d.animate=0; BufferedPaintUnInit
if(d.theme) CloseThemeData d.theme; d.theme=0
if(message=WM_DESTROY or !IsAppThemed) ret

d.theme=OpenThemeData(0 L"COMBOBOX") ;;note: don't pass d.hwnd, because then we get 0 because theming is disabled for this window (see below); if not disabled, then getwindowtheme after this would return this theme, which is not good if used by the base wndproc.
 out d.theme
sel message
	case WM_CREATE ;;actually called on the first wm_nccalcsize, because need to know whether the window is themed
	 Disable default theming processing by the base wndproc.
	 Because don't need it, and it interferes with our theming. Eg sends wm_paint on mouse in/out etc, sometimes even multiple (animated).
	 Tested, does not work: sub.CallBase(d.hwnd WM_THEMECHANGED -1 0x80000001)
	 No such problems with richedit controls, but cannot trust it.
	 Never mind: don't enable/disable on wm_stylechanged. If user ever needs scrollbars, let create control with these styles, and then remove if don't need.
	if !d.styleScrollbars ;;scrollbars would be ugly without theme
		d.noMessages=1 ;;avoid receiving wm_themechanged, but the base wndproc must receive
		SetWindowTheme d.hwnd L"" L""
		d.noMessages=0
	
	case WM_THEMECHANGED
	RedrawWindow d.hwnd 0 0 RDW_INVALIDATE|RDW_FRAME 


#sub OnNcCreate
function# ___QM_ComboBox&d CREATESTRUCTW&c

int style=c.style
if(style&(WS_VSCROLL|WS_HSCROLL)) d.styleScrollbars=1
c.style~___QMCBS_CLEAR; if(c.style!=style) d.noMessages=2; SetWinStyle d.hwnd c.style; d.noMessages=0
d.style=style

d.csv._create
d.csv.FromString("-1")


#sub OnStyleChanging
function ___QM_ComboBox&d wParam STYLESTRUCT&ss
d.style=ss.styleNew
ss.styleNew~___QMCBS_CLEAR


#sub OnCreate
function# ___QM_ComboBox&d CREATESTRUCTW&c

d.WM_QM_GETDIALOGVARIABLEDATA=RegisterWindowMessage("WM_QM_GETDIALOGVARIABLEDATA")

 tested with richedit:
 sub.CallBase(d.hwnd EM_SETEDITSTYLE SES_EMULATESYSEDIT SES_EMULATESYSEDIT) ;;don't paste rich text. But sets single-level undo, that undoes just SINGLE last typed character.
 sub.CallBase(d.hwnd EM_SETTEXTMODE TM_PLAINTEXT|TM_MULTILEVELUNDO|TM_MULTICODEPAGE 0) ;;don't paste rich text. But sets multiline-level undo, where each Undo undoes just SINGLE typed character.

 err+ out _error.description


#sub OnDestroy
function# ___QM_ComboBox&d
sub.Theme d WM_DESTROY


#sub CallBase
function# hwnd message [wParam] [lParam]
ret CallWindowProcW(__rwcQMCB.baseClassWndProc hwnd message wParam lParam)
 ret DefWindowProcW(hwnd message wParam lParam)


#sub OnSetText
function# ___QM_ComboBox&d wParam word*w
 Parses text as CSV to d.csv. Sets Edit text to match the selected/checked items in it.

if(d.isPressed) ret
ICsv& x=d.csv
d.flags=0
int notCsv
if(empty(w)) notCsv=1; else _s.ansi(w)
if(!notCsv) x.FromString(_s)
err+ notCsv=1
if(notCsv) x.FromString("-1"); sub.CallBase(d.hwnd WM_SETTEXT 0 L"")
if(x.ColumnCount>2) d.flags=val(x.Cell(0 2))
lpstr s
if(x.ColumnCount>3) s=x.Cell(0 3); if(!empty(s)) sub.CallBase(d.hwnd EM_SETCUEBANNER 0 @s)

sub.SetSelectedItem(d 1)


#sub GetDialogVariableData
function# ___QM_ComboBox&d wParam str&s

int i n checkboxes=d.flags&3
if(d.style&ES_READONLY=0 and d.flags&0x100=0) s.getwintext(d.hwnd); ret 1
sub.EnsureCsvUpdated(d)

if(d.flags&0x100) d.csv.ToString(s)
else if !checkboxes
	i=val(d.csv.Cell(0 0) 0 n); if(!n or i<0) i=-1
	s.format("%i %s" i _s.getwintext(d.hwnd))
else if(checkboxes&2) s=d.csv.Cell(0 0) ;;need "0xCHECKED"
else
	n=d.csv.RowCount-1
	s.all(n 2)
	for(i 0 n) s[i]=d.csv.Cell(i+1 2)&1+'0'

ret 1


#sub SetSelectedItem
function# ___QM_ComboBox&d [action] [iSelected] ;;action: 0 select (use iSelected), 1 on settext (iSelected not used), 2 on drop-down ok (iSelected not used, CSV updated)
 If on settext: updates Edit from CSV Cell(0 0). If it is not a number, sets -1 and sets Edit text = Cell(0 0).
 Else if on drop-down ok: updates Edit from CSV Cell(0 0) that is set by the drop-down control.
 Else: sets selected item to iSelected, and updates Edit text. If checkboxes, iSelected must be check states or 0.

int i n isNumber useiSelected checkState checked

sel(action) case [0] useiSelected=1; sub.EnsureCsvUpdated(d)

ICsv x=d.csv
n=x.RowCount-1
lpstr s=x.Cell(0 0)
i=iif(useiSelected iSelected val(s 0 isNumber))

if d.flags&3 ;;checkboxes
	if(d.flags&2) checkState=i
	
	if d.flags&1 ;;let Edit text be "CheckedItem1, CheckedItem2,..."
		str ss
		for i 0 n
			if(d.flags&2) checked=checkState>>i&1; if(i>31) break
			else checked=val(x.Cell(i+1 2))&1
			if(checked) ss.addline(x.Cell(i+1 0) 1)
		if(d.style&ES_MULTILINE=0) ss.findreplace("[]" ", ")
		s=ss
	else s=F"0x{checkState}" ;;let Edit text be "0xCHECKED"
	
	if(d.flags&2) x.Cell(0 0)=F"0x{checkState}"
else
	if(!useiSelected and !isNumber) i=-1
	if(i<0 or i>=n) i=-1
	x.Cell(0 0)=F"{i}"
	if(i<0) ret
	s=x.Cell(i+1 0)

d.noMessages=1 ;;EM_REPLACESEL would be used by OnTextChange
EditReplaceSel d.hwnd 0 s 1
d.noMessages=0


#sub ShowPopup
function ___QM_ComboBox&d
if(d.isPressed or d.csv.RowCount<2) ret

 TODO: notify parent

d.isPressed=1
d.isFocused=1; SetFocus d.hwnd ;;isFocused=1 to prevent calling sub.paint on wm_setfocus
if(d.flags&3=0 and d.style&ES_READONLY=0) sub.CallBase(d.hwnd EM_SETSEL 0 -1) ;;for incremental search
sub.Paint(d)
sub.EnsureCsvUpdated(d)
#compile "____QM_ComboList"
__QM_ComboList x
int R=x.Show(d.csv d.hwnd 0 d.hlv)
 out R
d.isPressed=0
sub.Paint(d)
if(R<0) ret
sub.SetSelectedItem(d 2)
d.textChanged=0

 TODO: notify parent


#sub OnKeyDown
function# ___QM_ComboBox&d message wParam lParam
 If processes the key (up/down arrows etc), returns 1.

sel(wParam) case [VK_DOWN,VK_UP,VK_PRIOR,VK_NEXT] if(d.style&ES_MULTILINE=0 or message=WM_SYSKEYDOWN) sub.ShowPopup(d); ret 1
if message=WM_KEYDOWN
	sel wParam
		case VK_DELETE sub.OnTextChange(d message wParam lParam); ret 1


#sub OnTextChange
function# ___QM_ComboBox&d message wParam lParam
 We call this on various messages that can change Edit text.
 We call basewndproc and set d.textChanged if text changed. Later use EnsureCsvUpdated, it updates CSV from changed Edit text, if d.textChanged.

int incrementalSearch=d.isPressed and d.flags&3=0 and IsWindow(d.hlv)
if(d.textChanged and !incrementalSearch) ret sub.CallBase(d.hwnd message wParam lParam)
 OutWinMsg message wParam lParam
str s1 s2
s1.getwintext(d.hwnd)
int R=sub.CallBase(d.hwnd message wParam lParam)
s2.getwintext(d.hwnd)
if(s2=s1) ret R
 out "changed"
d.textChanged=1
 incremental search
if(!incrementalSearch) ret R
ICsv x=d.csv
int i=x.Find(s2 1 0 1); if(i<0) i=x.Find(s2 5 0 1); if(i<0) i=x.Find(s2 17 0 1) ;;exact, the beginning, then anywhere
if(i>0) i-1
SendMessage GetParent(d.hlv) QMCBLM_SETCURSEL i 0
ret R


#sub EnsureCsvUpdated
function# ___QM_ComboBox&d
 If Edit text changed since start or prev call to EnsureCsvUpdated, parses Edit text and updates CSV. Returns 1 if actually updated.
 Uses and resets d.textChanged.

if(!d.textChanged) ret
d.textChanged=0
str s.getwintext(d.hwnd)
ICsv x=d.csv
int i j n=x.RowCount
int oldValue=val(x.Cell(0 0))

sel d.flags&3
	case 0
	i=x.Find(s 1 0 1); if(i<0) i=x.Find(s 5 0 1); if(i<0) i=x.Find(s 17 0 1) ;;exact, the beginning, then anywhere
	if(i>0) i-1
	if(i!=oldValue) x.Cell(0 0)=F"{i}"; ret 1 ;;TODO: add CellInt
	
	case 2
	i=val(s)
	if(i!=oldValue) x.Cell(0 0)=F"0x{i}"; ret 1
	
	case else
	ARRAY(str) a
	tok s a -1 "," 0x2000
	lpstr k; int check itemFlags checkedState itemFlagsChanged
	if(d.flags&2 and n>33) n=33
	for i 1 n
		k=x.Cell(i 0)
		for(j 0 a.len) if(!StrCompare(k a[j] 1)) break
		check=j<a.len
		if d.flags&2
			if(check) checkedState|1<<(i-1)
		else
			itemFlags=val(x.Cell(i 2))
			if(check!=itemFlags&1) x.Cell(i 2)=F"{itemFlags^1}"; itemFlagsChanged=0
	
	if(d.flags&2) if(checkedState!=oldValue) x.Cell(0 0)=F"0x{checkedState}"; ret 1
	else ret itemFlagsChanged


#sub OnNotify
function ___QM_ComboBox&d NMHDR*nh



#sub Notify
function ___QM_ComboBox&d code
 info: Edit and ComboBox notification codes don't overlap

SendMessageW(GetParent(d.hwnd) WM_COMMAND MakeInt(GetWinId(d.hwnd) code) d.hwnd)
