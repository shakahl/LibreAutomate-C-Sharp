function `x `y `hwnd [flags] ;;flags: 0 window, 1 client, 2 screen, 3 work area

 Gets object from point in window, and initializes this variable.

 x, y - <help #IDP_PIXELS>coordinates</help>. Can be integer (pixels) or double (fraction, eg 0.5 is center).
 hwnd - window handle, name or +class. If "" - active window. Error if 0.
 flags:
   0,1,2,3 - coordinates are relative to: 0 - window, 1 - window client area, 2 screen, 3 work area.

 Added in: QM 2.3.3.

 Errors: ERR_OBJECTGET, ERR_HWND, ERR_WINDOW.


this=acc(x y hwnd flags)

err+
	if(_error.code=ERRC_OBJECT) end ERR_OBJECTGET
	end _error
