out
 int w=id(2202 _hwndqm) ;;QM list of items (tree view)
int w=id(1023 win("QM - My Macros" "#32770")) ;;list
 int w=id(222 win("QM Help" "HH Parent")) ;;list 'Help Index Control'

WindowText wt.Init(w 0 WT_NOCHILDREN)

SCROLLINFO z.cbSize=sizeof(z); z.fMask=SIF_ALL
if(!GetScrollInfo(w SB_VERT &z) or z.nMax<=z.nMin) end "cannot scroll this control"
out "%i %i %i %i" z.nMin z.nMax z.nPos z.nPage
int pos0=z.nPos
SendMessage w WM_VSCROLL SB_TOP 0

rep
	str s=wt.CaptureToString
	out s
	
	z.fMask=SIF_POS; GetScrollInfo(w SB_VERT &z); out "%i %i" z.nPos z.nPos+z.nPage
	if(z.nPos+z.nPage>=z.nMax) break
	SendMessage w WM_VSCROLL SB_PAGEDOWN 0
	 rep(z.nPage) SendMessage w WM_VSCROLL SB_LINEDOWN 0

 SetScrollPos(w SB_VERT pos0 1) ;;does not scroll, just sets scrollbar position. Did not test SetScrollInfo, probably the same.
 SendMessage w WM_VSCROLL MakeInt(SB_THUMBPOSITION pos0) 0 ;;works only with some controls
SendMessage w WM_VSCROLL SB_TOP 0

 All the API functions and WM_VSCROLL message are documented in MSDN Library.

 This code has several problems:
 1. Works only if the control has standard scroll bar. Can be modified to work with controls that use a separate ScrollBar control like Word 2003; it's rarely used.
 2. Gets duplicate text items in last page. Maybe, it is possible to avoid it, eg calculate RECT, or try to change scroll range, it's difficult. In some controls also gets one or several duplicates in other pages; it also depends on window size, style, horz scrollbar...
 3. And possibly more problems.

 Or can use accessible object of scroll bar. Maybe would work with some nonstandard controls, but it is too limited.
 Acc a.Find(w "PUSHBUTTON" "Page down" "" 0x1031) ;;the sroll bar area that scrolls page down when clicked
 ...
 ,if(a.State&STATE_SYSTEM_INVISIBLE) break
 ,a.DoDefaultAction
