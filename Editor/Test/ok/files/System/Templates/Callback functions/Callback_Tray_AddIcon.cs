 /
function Tray&x message

 Callback function for Tray.AddIcon.
 Called for each received message - when tray icon clicked, or mouse moved.

 x - reference to this object.
 message - mouse message (WM_MOUSEMOVE, WM_LBUTTONDOWN, etc).


 OutWinMsg message 0 0 ;;uncomment to see received messages

sel message
	case WM_LBUTTONUP
	out "left click"
	 out x.param
	
	case WM_RBUTTONUP
	out "right click"
	
	case WM_MBUTTONUP
	out "middle click"
	
