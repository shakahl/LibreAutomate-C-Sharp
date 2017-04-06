 /
function hDlg [retVal]

 Gets data from controls and closes dialog.

 REMARKS
 You can call this function from dialog procedure (any place).
 If retVal is omitted or 0, ShowDialog returns 1. If it is some other value, ShowDialog returns that value.
 It is called implicitly on OK, after calling dialog procedure (IDOK), unless the dialog procedure returned 0.
 Read Help about dialogs created before QM 2.1.9.


DT_EndDialog hDlg iif(retVal retVal 1)
