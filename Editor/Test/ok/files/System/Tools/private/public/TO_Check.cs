 /
function hDlg $items _check [notify]

 Checks or unchecks multiple checkbox controls.
 items format - see <help>__IdStringParser.Parse</help>. Operator - inverts _check for that control.

__IdStringParser p.Parse(hDlg items)

_check=_check!0
int i c
for i 0 p.a.len
	__HWNDFLAGS& r=p.a[i]
	c=_check^(r.flags&1)
	if but(r.hwnd)!=c
		SendMessage r.hwnd BM_SETCHECK c 0
		if(notify) SendMessage hDlg WM_COMMAND GetDlgCtrlID(r.hwnd) r.hwnd

if(p.warnings.len) end p.warnings 8
