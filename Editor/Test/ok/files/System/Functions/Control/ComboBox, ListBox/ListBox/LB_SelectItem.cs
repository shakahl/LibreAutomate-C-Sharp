 /
function hwnd itemIndex [selType] ;;selType: 0 setcursel, 1 select in multisel, 2 deselect in multisel, 3 focus.

 Selects list box (ListBox control) item.

 hwnd - control handle.
 itemIndex - 0-based item index. Use -1 to select or deselect all in multisel.


if(!hwnd) end ERR_HWND
sel selType
	case 0
	if(GetWinStyle(hwnd)&LBS_MULTIPLESEL) ret SendMessage(hwnd LB_SETSEL 1 itemIndex)
	ret SendMessage(hwnd LB_SETCURSEL itemIndex 0)
	
	case [1,2] ret SendMessage(hwnd LB_SETSEL selType!=2 itemIndex)
	case 3 ret SendMessage(hwnd LB_SETCARETINDEX itemIndex 0)
	case else end ERR_BADARG
