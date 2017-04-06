 /
function# VARIANT'window_

 Converts any window expression (handle, name, etc) to window handle.
 Returns handle.
 Error if failed.

 REMARKS
 If window_ contains integer (VT_I4), it is interpreted as window handle.
 If window_ contains string (VT_BSTR), it is interpreted as window name or +class. If empty string - as currently active window.
 Error if window_ contains some other type.


int hwnd

sel window_.vt
	case VT_I4 ;;hwnd
	hwnd=window_.lVal
	if(!IsWindow(hwnd)) end "invalid window handle"
	
	case VT_BSTR ;;window name etc
	str s=window_.bstrVal
	if(!s.len) hwnd=win
	else if(s[0]='+') hwnd=win("" s+1)
	else hwnd=win(s)
	if(!hwnd) end "window not found"
	
	case else end "incorrect argument"

ret hwnd

err+ end _error
