 /
function hDlg $items _enable

 Enables or disables multiple controls.
 items format - see <help>__IdStringParser.Parse</help>. Operator - inverts _enable for that control.

__IdStringParser p.Parse(hDlg items)

_enable=_enable!0
int i
for i 0 p.a.len
	EnableWindow p.a[i].hwnd _enable^(p.a[i].flags&1)

if(p.warnings.len) end p.warnings 8
