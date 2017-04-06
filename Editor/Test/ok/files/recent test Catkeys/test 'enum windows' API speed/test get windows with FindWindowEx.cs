/exe 1

out
PF
 int w=FindWindowEx(0 0 0 0)
int w=GetTopWindow(0)
 PN; w=FindWindowEx(0 w 0 0); PN; w=FindWindowEx(0 w 0 0); PN; w=FindWindowEx(0 w 0 0); PN
 outw w; outw GetTopWindow(0); ret
rep
	if(w=0) break
	 outw2 w
	if GetWinStyle(w 1)&WS_EX_NOREDIRECTIONBITMAP
		outw2 w
	w=FindWindowEx(0 w 0 0)
PN
PO

 BEGIN PROJECT
 main_function  test get windows with FindWindowEx
 exe_file  $my qm$\test get windows with FindWindowEx.qmm
 flags  6
 guid  {72DB86D2-DE90-4D6C-B7A4-54D6DF1A4B7C}
 END PROJECT
