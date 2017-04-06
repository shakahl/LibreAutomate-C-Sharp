 /
function hDlg

 Closes dialog.

 REMARKS
 You can call this function from dialog procedure (any place).
 ShowDialog will return 0. Dialog variables will not be populated.
 It is called implicitly on Cancel, after calling dialog procedure (IDCANCEL), unless the dialog procedure returned 0.
 Read Help about dialogs created before QM 2.1.9.


DT_EndDialog hDlg
