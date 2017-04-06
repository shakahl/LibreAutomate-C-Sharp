/exe 1

out
PF
int w=GetTopWindow(0)
rep
	if(w=0) break
	 outw2 w
	if GetWinStyle(w 1)&WS_EX_NOREDIRECTIONBITMAP
		outw2 w
	w=GetWindow(w GW_HWNDNEXT)
PN
PO

 BEGIN PROJECT
 main_function  test get windows with GetWindow
 exe_file  $my qm$\test get windows with GetWindow.qmm
 flags  6
 guid  {3B0A6E56-E6C5-43A0-B8B0-714F84E42CC1}
 END PROJECT
