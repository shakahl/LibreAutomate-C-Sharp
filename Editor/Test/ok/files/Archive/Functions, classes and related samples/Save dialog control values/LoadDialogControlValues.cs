 /
function hDlg $regKey $regValue [$password]

 Sets dialog control values saved by SaveDialogControlValues.
 Call this function in dialog procedure, under WM_INITDIALOG.

 Note: Controls are identified by id. If you adit dialog and add/remove controls, previously saved values may become invalid. Solution: track dialog version. When version changed, delete the registry value.

 hDlg - hDlg.
 regKey, regValue - registry key and value.
 password - password control text encryption key.


str s
rget s regValue regKey
s.SetDialogXml(hDlg password)
err+ end _error
