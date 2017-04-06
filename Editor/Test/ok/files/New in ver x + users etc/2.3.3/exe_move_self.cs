 /exe

 note: the main function of the exe must be function, not macro.
 note: this function should be called somewhere at the beginning of the main function.


str myExe=ExeFullPath
 out myExe
str correctFolder.expandpath("$desktop$\My Files")
str correctExe.from(correctFolder "\" _s.getfilename(myExe 1))

if myExe~correctExe ;;I am in correct folder
	 what is my command line?
	str s
	if !empty(_command) and findrx(_command "/del ''(.+?)''" 0 1 s 1)
		 my command line tells me to delete that incorrect exe
		1 ;;wait until that incorrect exe's process ends
		del- s; err out "Failed to delete %s" s
		mes "File moved to folder ''My Files'' on desktop. "
		end ;;maybe this is not needed
else ;;I am somewhere else
	 copy me to correct folder; cannot move or delete me now, because i'm running.
	mkdir correctFolder
	cop- myExe correctFolder
	 run me from correct folder and pass command line that tells to delete me
	str cmdLine.format("/del ''%s''" myExe)
	run correctExe cmdLine
	 exit
	end

 BEGIN PROJECT
 main_function  selfmoving
 exe_file  $my qm$\selfmoving.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {9AACACC1-77F6-41CF-BBE3-17AAE769AEB2}
 END PROJECT
