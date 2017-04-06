 /exe

dll- nthookengine
	#HookFunction OriginalFunction HookFunction
	UnhookFunction OriginalFunction
	#GetOriginalFunction HookFunction

out
rep 2
	outb &ExtTextOutW 16
	if(!HookFunction(&ExtTextOutW &MyExtTextOutW3)) end "failed"
	outb &ExtTextOutW 16
	UnhookFunction &ExtTextOutW
	 UnhookFunction &MyExtTextOutW3
	 UnhookFunction GetOriginalFunction(&ExtTextOutW)
	outb &ExtTextOutW 16

 BEGIN PROJECT
 main_function  Macro1791
 exe_file  $my qm$\Macro1791.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {8C7ABA74-053B-44F3-B1EC-345771A9B6A1}
 END PROJECT
