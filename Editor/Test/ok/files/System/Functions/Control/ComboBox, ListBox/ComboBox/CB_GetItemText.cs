 /
function! hwnd item str&itemText

 Gets combo box (ComboBox control) item text.
 Returns: 1 success, 0 failed.

 hwnd - control handle.
 item - 0-based item index.
 itemText - str variable that receives item text.

 REMARKS
 Does not work with controls that have "owner-draw" style.

 EXAMPLE
 str s sall; int i hwnd=id(1003 "Options")
 for i 0 CB_GetCount(hwnd)
	 CB_GetItemText hwnd i s
	 sall.formata("%i. %s[]" i s)
 out sall


if(!hwnd) end ERR_HWND
if(!&itemText) end ERR_BADARG
ret sub_sys.CBLB_GetItemText(hwnd item itemText)
