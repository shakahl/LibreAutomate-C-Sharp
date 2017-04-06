 /
Should not be used. Better wait for overloaded operator=.

function VARIANT&Window [flags];; flags: 1 no error, 2 can be empty

 Finds window specified in VARIANT variable. Returns window handle or throws error.
 For use in functions that have window argument that can be specified by name, class
 or handle. The argument must be VARIANT'.
 Window can contain:
  integer - window handle;
  string - window name or +class;
  empty or empty string - active window.

int h
sel Window.vt
	case VT_EMPTY
	if(flags&2) ret
	h=win
	case VT_I4 h=Window.lVal
	case VT_BSTR
	str s=Window.bstrVal
	if(s.len) if(s[0]='+') h=win("" s+1); else h=win(s)
	else if(flags&2) ret
	else h=win
if(!h and flags&1=0) end "510Window not found"
ret h
