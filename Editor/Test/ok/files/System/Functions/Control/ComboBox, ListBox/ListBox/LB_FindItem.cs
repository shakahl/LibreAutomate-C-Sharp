 /
function# hwnd $itemText [startFrom] [exact]

 Returns list box (ListBox control) item index (0-based), when given item text.
 Returns -1 if string not found.

 hwnd - control handle.
 itemText - item text.
 startFrom - 0-based item index from which to start searching.
 exact - if 1, must match whole string, else must match beginning.


if(!hwnd) end ERR_HWND
ret sub_sys.CBLB_FindItem(hwnd itemText 1 startFrom exact)
