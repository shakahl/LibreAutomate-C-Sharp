 spe 1000
 run "$qm$\qmcs\qmcs.sln"
 run "$qm$\sqlite\sqlite.sln"
 run "$qm$\TextCapture\TextCapture.sln"
 run "$qm$\qmshex\qmshex.sln" "" "" "" 0x10000 ;;need admin because runs regsvr32

 rep 10
	 run "notepad.exe"

rep 3
	run "firefox.exe" "http://www.quickmacros.com/forum"
