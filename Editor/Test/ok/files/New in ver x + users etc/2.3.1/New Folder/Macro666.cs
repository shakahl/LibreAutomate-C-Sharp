spe
int w1=win("TOOLBAR" "QM_toolbar") ;;change this

 need to show menu, or men would not work
int+ __tb_hmenu
if(!__tb_hmenu)
	POINT p; xm p
	rig 4 4 w1; err ret
	int w2=wait(10 WV win("" "#32768" "qm"))
	__tb_hmenu=SendMessage(w2 MN_GETHMENU 0 0)
	clo w2
	mou p.x p.y
CheckMenuItem __tb_hmenu 32831 MF_CHECKED

men 32831 w1 ;;Follow Owner
