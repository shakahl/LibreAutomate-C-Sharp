sel ListDialog("name[]handle" "Center Notepad by:")
	case 1
	sub.MyCenterWindow "Notepad"
	case 2
	int hwnd=win("Notepad")
	sub.MyCenterWindow hwnd


#sub MyCenterWindow
function ANY'_window

 _window - window handle (integer) or name (string).

int hwnd
sel _window.ta
	case ANY_INTEGER
	hwnd=_window.i
	
	case ANY_STRING
	hwnd=win(_window.s)
	
	case else
	end ERR_BADARG

if(!hwnd) end ERR_WINDOW
CenterWindow hwnd
act _window ;;some QM window functions support ANY; all support handle
