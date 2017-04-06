 /exe
out
 int w=act(win("Microsoft Excel - Book1.xls" "XLMAIN"))
 int w=act(win("Microsoft Excel - Book2.xls" "XLMAIN"))

int w=win("Book2.xls" "MS-SDIa")
 int w=win("Untitled - Notepad" "Notepad")
 mac "Function301"
act w

 out min(w)
 AllowActivateWindows
 GetForegroundWindow
 if IsIconic(w)
	 ShowWindow(w SW_RESTORE)
	 0.5
	 outw win
	 res w
	 out IsIconic(w)
 0.001
 SetForegroundWindow w
  0.005
 rep 30
	 outw win
	 0.001
	  
 BEGIN PROJECT
 main_function  Macro2485
 exe_file  $my qm$\Macro2485.qmm
 flags  6
 guid  {1FC5B435-7B08-452D-B6EB-7BFE0DE54ED1}
 END PROJECT
