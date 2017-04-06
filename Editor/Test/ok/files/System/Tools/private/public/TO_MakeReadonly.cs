 /
function hDlg $items _readonly

 Makes multiple Edit controls readonly or editable.
 items format - see <help>__IdStringParser.Parse</help>. Operator - inverts _readonly for that control.

__IdStringParser p.Parse(hDlg items)

_readonly=_readonly!0
int i
for i 0 p.a.len
	SendMessage p.a[i].hwnd EM_SETREADONLY _readonly^(p.a[i].flags&1) 0

if(p.warnings.len) end p.warnings 8
