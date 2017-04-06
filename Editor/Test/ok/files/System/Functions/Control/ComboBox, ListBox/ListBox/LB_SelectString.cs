 /
function hwnd $itemText [selType] ;;selType: 0 setcursel, 1 select in multisel, 2 deselect in multisel, 3 focus.

 Finds and selects list box (ListBox control) item.
 Error if not found.

 hwnd - control handle.
 itemText - whole or beginning of item text.


opt noerrorshere
if(!hwnd) end ERR_HWND
int i=sub_sys.CBLB_FindItem(hwnd itemText 1); if(i<0) end ERR_ITEM
ret LB_SelectItem(hwnd i selType)
