function cid $text

 Updates tooltip text for a control.

 cid - control id.
 text - tooltip text.

 REMARKS
 QM 2.4.3. Also updates tooltip of combo box child Edit control. Also you can use ".2 " at the beginning of text, like with <help>__Tooltip.AddControl</help>.


if(!cid or empty(text) or !htt) ret

if text[0]='.'
	text+1
	int f=val(text 0 _i)
	text+_i; if(text[0]=32) text+1

int i h=id(cid __hwnd)
TOOLINFOW tti.cbSize=44
tti.hwnd=__hwnd
tti.lpszText=@text
tti.uFlags|TTF_IDISHWND
tti.uId=h

SendMessageW htt TTM_UPDATETIPTEXTW 0 &tti

if f&2
	ARRAY(int) a; child "" "" h 0 "" a
	for(i 0 a.len) tti.uId=a[i]; tti.hwnd=GetParent(a[i]); SendMessageW htt TTM_UPDATETIPTEXTW 0 &tti
else ;;auto update ComboBox Edit tt
	int he=GetWindow(h GW_CHILD)
	if(he and WinTest(he "Edit") and WinTest(h "*Combo*")) tti.uId=he; tti.hwnd=h; SendMessageW htt TTM_UPDATETIPTEXTW 0 &tti
