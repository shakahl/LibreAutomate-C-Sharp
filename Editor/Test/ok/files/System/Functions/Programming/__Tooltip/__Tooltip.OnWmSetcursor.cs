function wParam lParam

 Relays mouse event to the tooltip control.
 If flag 4 used with <help>__Tooltip.Create</help>, parent window/dialog procedure should always call this function on WM_SETCURSOR.
 Else can optionally call just to fix Windows XP bug: no tooltip after a click.


if m_flags&4=0 ;;called just to fix XP bug. To fix, need to relay event when mouse leaves subclassed control.
	if(_winnt>5 or wParam=m_hwndPrev) ret
	m_hwndPrev=wParam

MSG m.hwnd=wParam; m.message=lParam>>16
SendMessageW htt TTM_RELAYEVENT 0 &m
