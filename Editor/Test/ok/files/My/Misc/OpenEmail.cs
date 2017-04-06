 /exe
int gmail=val(_command)

int w

if gmail
	w=win("Gmail - Inbox*- qmgindi@gmail.com - Google Chrome" "Chrome_WidgetWin_0" "" 0x1)
	if(!w) run "chrome.exe" "https://mail.google.com/mail/?hl=lt&shva=1#inbox" "" "" 0 win("Google Chrome" "Chrome_WidgetWin_0") w
	 info: sometimes google logs out, and then we are not in inbox now
else
	run "thunderbird.exe" "" "" "" 0 win("* Mozilla Thunderbird" "Mozilla*WindowClass" "" 0x805) w

rep
	act w
	err 5; continue
	break

 err+

 BEGIN PROJECT
 main_function  OpenEmail
 exe_file  $program files$\POP Peeper\OpenEmail.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {DB4D6323-3E9F-479C-AA0A-6CF668EFD8F6}
 END PROJECT
