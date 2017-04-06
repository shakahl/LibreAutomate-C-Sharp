function cid $text [flags] [hwndParent] ;;flags: 1 cid is hwnd

 Adds tooltip for a control.

 cid - control id.
 text - tooltip text.
   Can begin with .flags, eg ".1 text". Flags:
      1 - use initial control rectangle. Use for controls that are transparent to mouse messages, eg a Static control without SS_NOTIFY style. Shows tooltip even if the control is hidden.
      2 (QM 2.3.5) - also add the tooltip to child controls of the control.
 hwndParent (QM 2.4.3) - handle of control parent window to use instead of hwnd passed to Create.

 REMARKS
 Call this before or after Create(). If before, the control and its parent window may still not exist; flags and hwndParent are not used.
 This function does not replace tooltips. Don't call it twice for a control.
 QM 2.4.3. Also adds combo box control tooltip for its child Edit control, don't need ".2 " in text.


if(!cid or empty(text)) ret

if !htt
	STRINT& r=__a[]
	r.i=cid
	r.s=text
	ret

if text[0]='.'
	text+1
	int f=val(text 0 _i)
	text+_i; if(text[0]=32) text+1

if(!hwndParent) hwndParent=__hwnd
int i h=iif(flags&1 cid id(cid hwndParent)); err ret
TOOLINFOW tti.cbSize=44
tti.hwnd=hwndParent
tti.lpszText=@text
if(m_flags&4=0) tti.uFlags=TTF_SUBCLASS
if f&1
	GetWindowRect h &tti.rect
	MapWindowPoints 0 hwndParent +&tti.rect 2
else
	tti.uFlags|TTF_IDISHWND
	tti.uId=h

SendMessageW htt TTM_ADDTOOLW 0 &tti

if f&2
	ARRAY(int) a; child "" "" h 0 "" a
	for(i 0 a.len) AddControl(a[i] text 1 GetParent(a[i]))
else ;;auto add for ComboBox Edit
	int he=GetWindow(h GW_CHILD)
	if(he and WinTest(he "Edit") and WinTest(h "*Combo*")) AddControl(he text 1 h)
