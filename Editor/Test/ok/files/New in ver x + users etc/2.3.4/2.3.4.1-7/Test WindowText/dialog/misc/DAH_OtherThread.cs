 \dlg_apihook
0.2; ret
 out 1
 ret
__MemBmp mb.Create(100 20)
RECT r
int- t_out=1
rep
	 out DrawText(mb.dc "h" 1 &r DT_CALCRECT)
	 Q &q
	 DrawText(0 "" 0 &r DT_CALCRECT)
	ExtTextOutW(0 0 0 0 0 L"" 0 0)
	 ExtTextOutW(mb.dc 0 0 0 0 L"T" 1 0)
	 0.001
	 Q &qq
	 outq