function flags ;;flags: 1 focus, 2 select, 4 extend selection, 8 add to selection, 16 unselect.

 Selects the object, or/and makes it focused.

 REMARKS
 Not all objects support it. Most objects support not all flags.
 With some objects, fails if the window is not active or the control is not focused, especially with flag 1. Use <help>act</help> to activate/focus (see example). Try flags 1, 2, 3.

 EXAMPLE
 int w=win("FileZilla" "wxWindowNR")
 Acc a.Find(w "LISTITEM" "download.html" "class=SysListView32[]id=-31781" 0x1015)
 act child(a)
 a.Select(3)


if(!a) end ERR_INIT
a.Select(flags elem); err end _error
if(_hresult) end F"{ERR_FAILED}.  Try to focus the parent control before calling Select: <code>act child(AccVariable)</code>"
