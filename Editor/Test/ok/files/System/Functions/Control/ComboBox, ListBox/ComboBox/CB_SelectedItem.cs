 /
function# hwnd [str&itemText]

 Returns 0-based index of selected item of combo box (ComboBox control).
 If there is no selection, returns -1.

 hwnd - control handle.
 itemText - str variable that receives item text. Can be 0 or omitted.

 EXAMPLES
  Get item index:
 out CB_SelectedItem(id(1136 "Open"))

  Get item text:
 str s
 if(CB_SelectedItem(id(1136 "Open") s) >= 0) out s


if(!hwnd) end ERR_HWND
ret sub_sys.CBLB_SelectedItem(hwnd 0 itemText)
