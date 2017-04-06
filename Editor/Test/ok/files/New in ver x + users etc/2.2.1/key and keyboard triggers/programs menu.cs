/exe
 MinimizeProcessMemory win("" "IEFrame")
 MinimizeProcessMemory win("Macromedia")
str s
sel PopupMenu("IE[]Dreamweaver[]Firefox[]Word[]Winmail[]Command Prompt[]Dialog[][]Begin memory stress[]End memory stress")
	case 1 s="Internet Ex"
	case 2 s="*Dreamweaver */test.htm*"
	case 3 s="Untitled Document - Mozilla Firefox"
	case 4 s="Word"
	case 5 s="New Message"
	case 6 s="Prompt"
	 case 7 s="Dialog A"; act s; but 3 s; ret
	case 7 s="Dialog"
	 -
	case 9 mac "eat_memory2"; ret
	case 10 shutdown -6 0 "eat_memory2"; ret
	case else ret

TEST_KEY s
 TEST_CLIPBOARD s
 TEST_CLICK s

 BEGIN PROJECT
 main_function  programs menu
 exe_file  $my qm$\programs menu.exe
 icon  $qm$\keyboard.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {61B736AF-79BE-47E8-B5FF-2586323FEBFC}
 END PROJECT
