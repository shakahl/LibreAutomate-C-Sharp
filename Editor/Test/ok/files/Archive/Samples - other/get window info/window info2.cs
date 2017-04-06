str wtxt wcls ctxt ccls prog progfull
int h cid cstyle wstyle cexstyle wexstyle

h=child(mouse)
if(h)
	ccls.getwinclass(h)
	ctxt.getwintext(h)
	cid=GetWinId(h)
	cstyle=GetWinStyle(h)
	cexstyle=GetWinStyle(h 1)

h=win(mouse)
wcls.getwinclass(h)
wtxt.getwintext(h)
wstyle=GetWinStyle(h)
wexstyle=GetWinStyle(h 1)

prog.getwinexe(h)
progfull.getwinexe(h 1)
