out
int wMain=win("QM Help" "HH Parent")
int w=id(222 wMain) ;;list 'Help Index Control'

WindowText wt.Init(w)
int pos=GetScrollPos(w SB_VERT)
SendMessage w WM_VSCROLL SB_TOP 0
Acc a.Find(w "PUSHBUTTON" "Page down" "" 0x1031) ;;the sroll bar area that scrolls page down when clicked
rep
	str s=wt.CaptureToString
	out s
	 out GetScrollPos(w SB_VERT)
	if(a.State&STATE_SYSTEM_INVISIBLE) break
	a.DoDefaultAction
	0.01 ;;may not scroll to the end if we call DoDefaultAction too frequently
 SendMessage w WM_VSCROLL SB_TOP 0
 SetScrollPos(w SB_VERT pos 1) ;;does not scroll, just sets scrollbar position
SendMessage w WM_VSCROLL MakeInt(SB_THUMBPOSITION pos) 0

 Other functions that may be useful, all documented in MSDN Library:
 GetScrollInfo, GetScrollRange, SetScrollInfo

 This code has several problems:
 1. Works only if the control has standard scroll bar. Can be modified to work with controls that use a separate ScrollBar control like Word 2003.
 2. Gets duplicate text items in last page. Maybe, using all the above functions, it is possible to avoid it. In some controls also may get one or several duplicates in other pages.
 3. And possibly more problems.
