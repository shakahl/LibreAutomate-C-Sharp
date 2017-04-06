Tray t.AddIcon("$qm$\copy.ico" "tt" 0 &sub.TrayCallback)
mes "test tray icon menu"


#sub TrayCallback
function Tray&x message

 Callback function for Tray.AddIcon.
 Called for each received message - when tray icon clicked, or mouse moved.
 
 x - reference to this object.
 message - mouse message (WM_MOUSEMOVE, WM_LBUTTONDOWN, etc).


 OutWinMsg message 0 0 ;;uncomment to see received messages

sel message
	case WM_LBUTTONUP
	 out "left click"
	
	case WM_RBUTTONUP
	 out "right click"
	sub.OnTrayIconRightClick x.param
	
	case WM_MBUTTONUP
	 out "middle click"
	
out 4


#sub OnTrayIconRightClick
function param

 created and can be edited with the Menu Editor

str md=
 BEGIN MENU
 >&File
 	&Open :501 0x0 0x0 Co
 	&Save :502 0x0 0x0 Cs
 	>Submenu
 		Item1 :551
 		Item2 :552
 		<
 	-
 	E&xit :2
 	<
 >&Edit
 	Cu&t :601
 	&Copy :602
 	&Paste :603
 	<
 >&Help
 	&About :901
 	<
 END MENU

int i=ShowMenu(md); out i
