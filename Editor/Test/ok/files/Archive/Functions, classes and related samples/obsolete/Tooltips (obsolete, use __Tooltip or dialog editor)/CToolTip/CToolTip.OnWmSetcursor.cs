function wParam lParam

 Call this on WM_SETCURSOR.


MSG ms.hwnd=wParam; ms.message=lParam>>16
SendMessage(m_htt, TTM_RELAYEVENT, 0, &ms);
