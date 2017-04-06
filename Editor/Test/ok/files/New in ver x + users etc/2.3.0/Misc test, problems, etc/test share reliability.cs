 test how works when CPU busy

out
spe
int h close
 h=win("Word")
 h=win("app")
h=win("Internet Explorer")
 h=_hwndqm
if(!h)
	close=1
	 run "$program files$\Microsoft Office\OFFICE11\WINWORD.EXE" "" "" "" 0x800 "Word" h
	run "iexplore.EXE" "" "" "" 0x800 "Internet Explorer" h

out share(h)
err out _error.description

if(close) clo h
 if(close) PostMessage(h, WM_SYSCOMMAND, SC_CLOSE, 0)
