 /
function $style

sel style 1
	case "None" ;;faster
	key McRY
	ret

int w1=win("" "OWL.Dock" "" 0x0 "cClass=ComboBox[]cId=590")
if(!w1) w1=win("Adobe Dreamweaver" "_macr_dreamweaver_frame_window_")
int c=id(590 w1) ;;combo box
SendClickMessage 5 5 c
rep 10
	CB_SelectString c style
	err 0.1; continue ;;sometimes 'item not found' when running this macro too frequently
	key Y
	break
