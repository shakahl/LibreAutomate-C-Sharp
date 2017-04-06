function hwnd [ctrlId]

 Assigns a control to this variable.

 hwnd - handle of control's parent window. If ctrlId is omitted or 0 - handle of control itself.
 ctrlId - control id.


h=iif(ctrlId GetDlgItem(hwnd ctrlId) hwnd)
