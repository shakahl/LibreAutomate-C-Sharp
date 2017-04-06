 /
function hDlg $regKey $regValue $controls [$password]

 Saves dialog control values in registry.
 Call this function in dialog procedure, under case IDOK or case WM_DESTROY.

 hDlg - hDlg.
 regKey, regValue - registry key and value. The function uses HKEY_CURRENT_USER hive. regKey must not begin with HKEY_CURRENT_USER or similar.
 controls - space-delimited list of control ids. Example: "3 4 15".
 password - password control text encryption key. If not used, password control text will not be encryted.


str s.GetDialogXml(hDlg controls password)
rset s regValue regKey
err+ end _error
