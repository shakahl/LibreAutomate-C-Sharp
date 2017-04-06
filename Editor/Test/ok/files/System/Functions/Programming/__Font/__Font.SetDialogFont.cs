function hDlg [$controls]

 Sets font for all or some controls in a dialog.

 hDlg - dialog handle. Can be any window, not necessary a dialog.
 controls - string consisting of one or more control ids separated by space. To specify range of ids, use hyphen. Use 0 id for hDlg itself. If omitted or "", sets font for all controls.

 See also: <DT_SetTextColor>


ARRAY(int) a
sub_sys.Font_ParseControls(hDlg controls a)
for(_i 0 a.len) SendMessage a[_i] WM_SETFONT handle 1
