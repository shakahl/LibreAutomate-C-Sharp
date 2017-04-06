out
shutdown 6
 ScreenSaverRun "" 1
3
 90
 10
 outw win
 outw win(0.5 0.5)
 key F24
 clo win
 PostMessage win WM_KEYDOWN VK_F22 0

 mac "bbbytre"
int unlocked=EnsureLoggedOn(1)
out "unlocked=%i" unlocked ;;should be 2
if(!unlocked and FileExists("$qm$\qmtul.log")) run "notepad.exe" "$qm$\qmtul.log"
 outw win
