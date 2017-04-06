function!

 The main function. Sets mouse capture, cursor, gets messages, stops when need.
 Call this function in a rep loop until it returns 0.
 Returns 0 when dropped (when released mouse button specified in mouseButton that was passed to Init()) or cancelled (eg user pressed Esc).
 Returns 1 on each other received mouse message and key down/up message. Also on mouse button up message if the button was not specified when calling Init().

 REMARKS
 If the caller breaks loop before Next() returns 0, it should call ReleaseCapture().
 For more info and example, look in class help.


if(!m_hwnd || !m_mouseButton) end ERR_INIT

if cursor
	if(cursor>=1 && cursor<=6) cursor=sub_to.LoadCursor(cursor)
	if(cursor) SetCursor(cursor); cursor=0

dropped=0

if !m_started
	m_started=1
	SetCapture(m_hwnd)

rep
	if(GetCapture!=m_hwnd) break
	if GetMessage(&m 0 0 0)<=0
		if(m.message=WM_QUIT) PostQuitMessage(m.wParam)
		break
	
	if m.message>=WM_MOUSEFIRST && m.message<=WM_MOUSELAST
		sel m.message
			case WM_LBUTTONUP dropped=m_mouseButton&1
			case WM_RBUTTONUP dropped=m_mouseButton&2
		if(dropped) break

		mk=m.wParam&(MK_SHIFT|MK_CONTROL)
		p.x=ConvertSignedUnsigned(m.lParam&0xFFFF 2); p.y=ConvertSignedUnsigned(m.lParam>>16 2)
		if(m.message=WM_MOUSEWHEEL) ScreenToClient(m_hwnd &p)
		ret 1
	else
		sel m.message
			case [WM_KEYDOWN,WM_KEYUP,WM_SYSKEYDOWN,WM_SYSKEYUP] ;;caller may want to update cursor when Ctrl pressed/released
			if(m.wParam=VK_ESCAPE) break
			mk=0
			ifk(C) mk|=MK_CONTROL
			ifk(S) mk|=MK_SHIFT
			GetCursorPos(&p); ScreenToClient(m_hwnd &p)
			ret 1
	
	DispatchMessage(&m)

if(GetCapture=m_hwnd) ReleaseCapture
m_started=0
