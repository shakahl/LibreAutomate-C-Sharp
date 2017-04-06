 /
function [$screensaver_file] [flags] ;;flags: 1 natural

 Runs screen saver.
 screensaver_file can be screen saver file (scr), or "" to run default screen saver. Most screen saver files are in Windows folder.
 By default, screen saver will not be password-protected.
 If flags includes 1, runs default screen saver (screensaver_file is not used) which, depending on Control Panel settings, may be password-protected.


if(flags&1)
	SendMessageW _hwndqm WM_SYSCOMMAND SC_SCREENSAVE 0
	
	  or
	 double+ __ss_timeout=ScreenSaverGetTimeout
	 atend __SS_RestoreTimeout
	 ScreenSaverSetTimeout 1.0/60 1
	 BlockInput 1
	 1
	 rep(100) 0.1; if(ScreenSaverIsRunning) break
	 BlockInput 0
	 __SS_RestoreTimeout
else
	if empty(screensaver_file)
		if(rget(_s "scrnsave.exe" "Control Panel\Desktop")>1) screensaver_file=_s ;;or, if set in group policies, probably will be in HKLM\Software\Microsoft\Windows\CurrentVersion\Policies\...
		else ret ;;None
	run screensaver_file; err end _error
