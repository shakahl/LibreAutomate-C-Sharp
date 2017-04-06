 /
function# hwnd $itemText [itemData]

 Adds new item to combo box control.
 Returns new item index. On error, returns a negative value.

 hwnd - control handle.
 itemText - item text.
 itemData - a value that later can be accessed with SendMessage(hwnd CB_GETITEMDATA item 0).

 REMARKS
 To clear old items: SendMessage(hwnd CB_RESETCONTENT 0 0)

 Added in: QM 2.3.0.


int ni=SendMessageW(hwnd CB_ADDSTRING 0 @itemText)
if(itemData) SendMessage(hwnd CB_SETITEMDATA ni itemData)
ret ni
