function# [hwndOwner] [POINT&where] [flags] [tpmFlags] ;;flags: 1 enable keyboard

 Shows popup menu. Returns selected item id or 0.

 All parameters are as with <help>ShowMenu</help>.


if(!m_h or !GetMenuItemCount(m_h)) ret

int ht hf
if flags&1
	hf=win
	if GetWindowThreadProcessId(hf 0)!=GetCurrentThreadId
		ht=CreateWindowEx(WS_EX_TOOLWINDOW +32770 0 WS_POPUP 0 0 0 0 0 0 _hinst 0)
		act ht; err

int R=__TrackPopupMenu(m_h hwndOwner tpmFlags &where)

if ht
	DestroyWindow(ht)
	if(hf and hf!=win) act hf; err

ret R
