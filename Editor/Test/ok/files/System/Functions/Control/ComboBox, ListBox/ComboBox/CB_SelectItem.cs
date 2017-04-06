 /
function# hwnd itemIndex [_]

 Selects combo box (ComboBox control) item.
 QM 2.4.3: Returns new selected item index or -1.

 hwnd - control handle.
 itemIndex - 0-based item index.
 _ - obsolete, don't use.

 EXAMPLE
  Select combo box item 1 (window "Open", combo box id=1136):
 CB_SelectItem(id(1136 "Open") 1)


if(!hwnd) end ERR_HWND
ret sub_sys.CBLB_SelectItem(hwnd itemIndex 0 _)
