out
#compile "__TRAYICONINFO"
ARRAY(TRAYICONINFO) a
EnumTrayIcons 0 a 3
int i
for i 0 a.len
	TRAYICONINFO& r=a[i]
	 if(r.hidden) continue
	RecGetWindowName r.hwnd &_s
	str se.getwinexe(r.hwnd)
	out "%stt='%s'  idcmd=%i  window=%s  exe=%s  hicon=%i  callbackId=%i  callbackMsg=%i" iif(r.hidden "(hidden) " "") r.tooltip r.idCommand _s se r.hicon r.callbackId r.callbackMsg
