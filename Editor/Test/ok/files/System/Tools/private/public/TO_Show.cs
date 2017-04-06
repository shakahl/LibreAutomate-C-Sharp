 /
function hDlg $items _show

 Shows or hides multiple controls.
 items format - see <help>__IdStringParser.Parse</help>. Operator - inverts _show for that control.

__IdStringParser p.Parse(hDlg items)

_show=_show!0
int i
for i 0 p.a.len
	ShowWindow p.a[i].hwnd _show^(p.a[i].flags&1)

if(p.warnings.len) end p.warnings 8
