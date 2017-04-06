str color="color:0x5AD3F7"
rep
	if(scan(color))
		out "yes"
		1 ;;don't make this too small, or this macro will eat too much CPU. Look in Task Manager.
	else
		out "no"
		wait 5 S color; err
