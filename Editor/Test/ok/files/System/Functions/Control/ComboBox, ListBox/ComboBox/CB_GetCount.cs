 /
function# hwnd

 Returns number of items in the combo box (ComboBox control).


if(!hwnd) end ERR_HWND
ret SendMessage(hwnd CB_GETCOUNT 0 0)
