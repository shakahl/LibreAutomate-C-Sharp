 /
function# hwnd

 Returns number of items in the list box (ListBox control).


if(!hwnd) end ERR_HWND
ret SendMessage(hwnd LB_GETCOUNT 0 0)
