
long+ openExplorer_PreviousRuntime
;; Open My Computer if trigger key pressed twice quickly
long elapsed = elapsedms(openExplorer_PreviousRuntime) 
if  0 and elapsed < doubleKeyPressDelayMS and elapsed > 0
	out "%s: double-keypress detected" fnName
	 tim 0 OpenExplorer2;; Cancel OpenExplorer2 timer
	 openApp "my Computer" "c:\windows\explorer.exe" "/e,"
	 run "$0x11$" ;;My Computer. From Help / Other Info / File Search Paths
	prevExeName=""
	ret

;; Start timer to open next explorer window after double keypress timeout
 tim (0.0+doubleKeyPressDelayMS/1000) OpenExplorer2
OpenExplorer2
