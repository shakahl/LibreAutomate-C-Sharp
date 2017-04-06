 /
function! hwnd itemIndex str&itemText

 Gets list box (ListBox control) item text.
 Returns: 1 success, 0 failed.

 hwnd - control handle.
 itemIndex - 0-based item index.
 itemText - str variable that receives item text.

 REMARKS
 Does not work with controls that have "owner-draw" style.


if(!hwnd) end ERR_HWND
if(!&itemText) end ERR_BADARG
ret sub_sys.CBLB_GetItemText(hwnd itemIndex itemText 1)
