 /
function hDlg idTab $tabs [flags] ;;flags: 1 create child dialog to make colors better

 Adds tab control tabs.

int htb=id(idTab hDlg)

TCITEMW ti.mask=TCIF_TEXT
ARRAY(str) a=tabs
int i
for(i 0 a.len) ti.pszText=@a[i]; SendMessage(htb TCM_INSERTITEMW i &ti)

if flags&1
	 Workaround for different colors of tab and controls.
	 Could use EnableThemeDialogTexture, but then need at least 2 dialogs, etc.
	 With SetWindowTheme also not good.
	
	RECT r
	GetClientRect htb &r
	SendMessage htb TCM_ADJUSTRECT 0 &r
	r.left-1
	MapWindowPoints htb hDlg +&r 2
	int hd=CreateControl(0 "#32770" 0 0x54000000 r.left r.top r.right-r.left r.bottom-r.top hDlg 900)
	SetWindowPos htb hd 0 0 0 0 SWP_NOMOVE|SWP_NOSIZE|SWP_NOACTIVATE
