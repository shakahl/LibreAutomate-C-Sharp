 run "qmtul.exe" "" "" "" 0x30400 ;;show settings dialog

del- "$qm$\qmtul.log"; err ;;delete log file
shutdown 6 ;;lock PC
 ScreenSaverRun "" 1
5 ;;wait until locked; change this if locks slowly.
out EnsureLoggedOn(1) ;;unlock. Returns 0 if failed.
 1
 run "$qm$\qmtul.log" ;;see log file
