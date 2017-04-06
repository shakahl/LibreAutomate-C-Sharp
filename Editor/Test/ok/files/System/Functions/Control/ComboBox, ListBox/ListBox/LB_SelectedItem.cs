 /
function# hwnd [str&itemText]

 Returns 0-based index of selected item of list box (ListBox control).
 If there is no selection, returns -1.

 hwnd - control handle.
 itemText - str variable that receives item text. Can be 0 or omitted.


if(!hwnd) end ERR_HWND
ret sub_sys.CBLB_SelectedItem(hwnd 1 itemText)
