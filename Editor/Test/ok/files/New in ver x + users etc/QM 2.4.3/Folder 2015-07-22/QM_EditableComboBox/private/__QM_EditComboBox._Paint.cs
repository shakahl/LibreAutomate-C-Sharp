 /dialog_test_QM_EditComboBox
function [!onWmNcPaint] [wParam] [lParam]

type __QMCB_PAINT !onWmNcPaint wParam lParam RECT'r RECT'rb RECT're
__QMCB_PAINT p.onWmNcPaint=onWmNcPaint; p.wParam=wParam; p.lParam=lParam

_GetRects(p.rb p.r p.re)
sub.PaintScrollbars(p)

__Hdc dc=GetWindowDC(_hwnd)
ExcludeClipRect dc p.re.left p.re.top p.re.right p.re.bottom

if !_theme
	if(_borderWidth) DrawEdge dc &p.r EDGE_SUNKEN BF_RECT
	int state; if(!IsWindowEnabled(_hwnd)) state=DFCS_INACTIVE; else if(_isPressed) state=DFCS_FLAT|DFCS_HOT; else if(_isMouseIn=2) state=DFCS_HOT
	DrawFrameControl dc &p.rb DFC_SCROLL DFCS_SCROLLDOWN|state
	ret

 frame
int stateF=CBB_NORMAL
if(!IsWindowEnabled(_hwnd)) stateF=CBB_DISABLED; else if(_isFocused) stateF=CBB_FOCUSED; else if(_isMouseIn) stateF=CBB_HOT
if(!_borderWidth) p.r.top-1; p.r.right+1; p.r.bottom+1 ;;don't draw border but draw background (need for transparent button)
 button
int stateB=CBXS_NORMAL
if(stateF=CBB_DISABLED) stateB=CBXS_DISABLED; else if(_isPressed) stateB=CBXS_PRESSED; else if(_isMouseIn=2) stateB=CBXS_HOT

if !sub.PaintAnimated(p dc stateF stateB)
	 out "no anim, onWmNcPaint=%i" onWmNcPaint
	sub.PaintThemed(p dc stateF stateB)

_stateF=stateF; _stateB=stateB


#sub PaintAnimated c
function# __QMCB_PAINT&p dc stateF stateB
 Returns 1 if painted.

 use animation?
if(!_animate and _winver>=0x600 and _theme and !BufferedPaintInit) _animate=1
if(!_animate) ret

if(p.onWmNcPaint) if(BufferedPaintRenderAnimation(_hwnd dc)) ret 1
else if(stateB=_stateB and stateF=_stateF) ret 1

int transDuration ;;tested: buton times are the same as frame times.
if(stateB!=_stateB) GetThemeTransitionDuration(_theme CP_BORDER _stateB stateB TMT_TRANSITIONDURATIONS &transDuration)
else GetThemeTransitionDuration(_theme CP_DROPDOWNBUTTONRIGHT _stateF stateF TMT_TRANSITIONDURATIONS &transDuration)

 when state changes while in a middle of previous animation, get the first image from window DC instead of drawing old state bitmap. We cannot get current buff animation DC, there is no API.
if(transDuration>600) transDuration=600
int timeNow=GetTickCount
if(_animTime>timeNow and !p.onWmNcPaint) int bitblt=1 ;;prev animation not ended
_animTime=timeNow+transDuration
 out "%i %i %i %i    %i  bitblt=%i" _stateF stateF _stateB stateB transDuration bitblt

BP_PAINTPARAMS paintp.cbSize=sizeof(paintp); paintp.dwFlags=BPPF_NONCLIENT; paintp.prcExclude=&p.re
BP_ANIMATIONPARAMS animp.cbSize=sizeof(animp); animp.style=BPAS_LINEAR; animp.dwDuration=transDuration
int hdcFrom hdcTo
int hbpAnimation=BeginBufferedAnimation(_hwnd dc &p.r BPBF_COMPATIBLEBITMAP &paintp &animp &hdcFrom &hdcTo)
if(!hbpAnimation) ret
 out "hdcFrom=%i hdcTo=%i" hdcFrom hdcTo
if hdcFrom
	if(bitblt) BitBlt hdcFrom 0 0 p.r.right p.r.bottom dc 0 0 SRCCOPY
	else sub.PaintThemed(p hdcFrom _stateF _stateB 1)
if(hdcTo) sub.PaintThemed(p hdcTo stateF stateB)
EndBufferedAnimation(hbpAnimation 1)
ret 1


#sub PaintThemed c
function __QMCB_PAINT&p dc stateF stateB [!noParent]

if _winver>=0x600 and !noParent and IsThemeBackgroundPartiallyTransparent(_theme CP_BORDER stateF) ;;draw rounded corners on Win7
	DrawThemeParentBackgroundEx(_hwnd dc DTPB_WINDOWDC|DTPB_USECTLCOLORSTATIC &p.r) ;;without DTPB_USECTLCOLORSTATIC error "Incorrect function", even whem animation disabled (it draws to a memory DC); DrawThemeParentBackground too; but even then draws correctly
DrawThemeBackground(_theme dc CP_BORDER stateF &p.r 0) ;;frame
DrawThemeBackground(_theme dc iif((_winver<0x600 or _style&(ES_READONLY|ES_MULTILINE)) CP_DROPDOWNBUTTON CP_DROPDOWNBUTTONRIGHT) stateB &p.rb 0) ;;button
if(_borderWidth=1) DrawEdge dc &p.r BDR_SUNKENOUTER BF_RECT ;;ws_ex_staticedge, standard Edit controls are not themed
 TODO: if small height, eg


#sub PaintScrollbars c
function __QMCB_PAINT&p
 Calls defwndproc wm_ncpaint if need, excluding our frame/button region from its update region.
 In our wndproc we never pass wm_ncpaint to the base wndproc, because it interferes with our nonclient drawing and causes flickering.
 Instead call this func before actually drawing our frame/button, to let it draw scrollbars if need.

if(!(p.onWmNcPaint and _styleScrollbars)) ret

 out p.wParam
RECT r re=p.re
GetWindowRect _hwnd &r; OffsetRect &re r.left r.top
__GdiHandle hrgn=CreateRectRgnIndirect(&re)
if(p.wParam!1 and CombineRgn(hrgn hrgn p.wParam RGN_AND)=1) ret ;;NULLREGION, usually when processing our animated paint

DefWindowProcW(_hwnd WM_NCPAINT hrgn 0)
 0.1 ;;test flickering
