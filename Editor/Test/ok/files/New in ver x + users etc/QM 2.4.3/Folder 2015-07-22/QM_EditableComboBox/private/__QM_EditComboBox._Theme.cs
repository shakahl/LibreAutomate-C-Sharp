function message ;;message: wm_create, wm_themechanged, wm_destroy

if(_winver<0x501) ret
_stateF=CBB_NORMAL; _stateB=CBXS_NORMAL
if(_winver>=0x600 and _animate) _animate=0; BufferedPaintUnInit
if(_theme) CloseThemeData _theme; _theme=0
if(message=WM_DESTROY or !IsAppThemed) ret

_theme=OpenThemeData(0 L"COMBOBOX") ;;note: don't pass _hwnd, because then we get 0 because theming is disabled for this window (see below); if not disabled, then getwindowtheme after this would return this theme, which is not good if used by the base wndproc.
 out _theme
sel message
	case WM_CREATE ;;actually called on the first wm_nccalcsize, because need to know whether the window is themed
	 Disable default theming processing by the base wndproc.
	 Because don't need it, and it interferes with our theming. Eg sends wm_paint on mouse in/out etc, sometimes even multiple (animated).
	 Tested, does not work: _BaseProc(WM_THEMECHANGED -1 0x80000001)
	 No such problems with richedit controls, but cannot trust it.
	 Never mind: don't enable/disable on wm_stylechanged. If user ever needs scrollbars, let create control with these styles, and then remove if don't need.
	if !_styleScrollbars ;;scrollbars would be ugly without theme
		_noMessages=1 ;;avoid receiving wm_themechanged, but the base wndproc must receive
		SetWindowTheme _hwnd L"" L""
		_noMessages=0
	
	case WM_THEMECHANGED
	RedrawWindow _hwnd 0 0 RDW_INVALIDATE|RDW_FRAME 
