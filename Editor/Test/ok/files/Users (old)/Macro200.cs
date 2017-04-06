int+ g_pauseMacroX=1
rep
	if(g_pauseMacroX=0)
		sel wait(0 V g_pauseMacroX)
			case 1
			 ...
			case 2
			 ...
	 ...



int+ g_pauseMacroX=0
 ...
int+ g_pauseMacroX=1 ;;or 2, etc
