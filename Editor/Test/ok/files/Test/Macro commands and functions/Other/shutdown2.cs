int i=-7

sel i
	case 0 ;;logoff
	shutdown 0
	
	case 1 ;;shutdown
	shutdown 1
	 shutdown 1 0 "" 10 "mmm"
	
	case 2 ;;restart
	shutdown 2
	 shutdown 2 0 "" 10 "mmm"
	
	case 3 ;;poweroff
	shutdown 3
	
	case 4 ;;hibernate
	shutdown 4
	
	case 5 ;;suspend
	shutdown 5
	
	case 6 ;;lock/switch
	shutdown 6
	
	
	case -1 ;;(exit qm)
	shutdown -1
	
	case -2 ;;(restart qm)
	shutdown -2 0 "V"
	
	case [-3,-4] ;;(hide/show qm)
	shutdown -3
	1
	shutdown -4
	
	case -5 ;;(reload file)
	shutdown -5
	
	case -6 ;; (end thread)
	 int iid=qmitem("TO_Text")
	 shutdown -6 0 iid
	shutdown -6 0 "TO_Text"
	
	case -7
	shutdown -7 1 ;;softly end current thread
	5
	
