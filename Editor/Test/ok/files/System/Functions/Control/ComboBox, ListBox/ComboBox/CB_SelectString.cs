 /
function# hwnd $itemText [_]

 Selects combo box (ComboBox control) item.
 Error if not found.
 QM 2.4.3: Returns new selected item index.

 hwnd - control handle.
 itemText - whole or beginning of item text.
 _ - obsolete, don't use.


if(!hwnd) end ERR_HWND
int i=sub_sys.CBLB_FindItem(hwnd itemText); if(i<0) end ERR_ITEM
ret sub_sys.CBLB_SelectItem(hwnd i 0 _)
