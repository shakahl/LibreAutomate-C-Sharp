 /
function hDlg

 Frees allocated memory when destroying dialog.
 Called implicitly on WM_DESTROY, after calling dialog procedure.
 Read Help about dialogs created before QM 2.1.9.


__DIALOG* d=+RemoveProp(hDlg +__atom_dialogdata)
d._delete
