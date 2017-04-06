 /
function'str* hDlg

 Call this function from dialog procedure to get the value that was passed to ShowDialog as third parameter (controls).
 That is, address of the variable that is used to get/set controls data.
 Unlike DT_GetControls, this function does not populate control variables.


__DIALOG* d=+GetProp(hDlg +__atom_dialogdata)
if(!d) ret
ret d.controls
