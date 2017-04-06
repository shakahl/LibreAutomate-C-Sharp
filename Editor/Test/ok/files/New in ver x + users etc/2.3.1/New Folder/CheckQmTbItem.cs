 /
function! hwndTb menuItemId check

int+ __tb_hmenu
if(!__tb_hmenu)
	POINT p; xm p
	rig 4 4 hwndTb
	int w2=wait(10 WV win("" "#32768" "qm"))
	__tb_hmenu=SendMessage(w2 MN_GETHMENU 0 0)
	clo w2
	mou p.x p.y
ret CheckMenuItem(__tb_hmenu menuItemId iif(check MF_CHECKED 0))>=0
err+
