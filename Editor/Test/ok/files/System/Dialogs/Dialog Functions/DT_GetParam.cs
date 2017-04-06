 /
function# hDlg

 Call this function from dialog procedure to get param that was passed to ShowDialog.


__DIALOG* d=+GetProp(hDlg +__atom_dialogdata)
if(!d) ret
ret d.param
