 \
function mainThreadHandle

atend ResumeThread mainThreadHandle
rep
	 wait for some condition, eg window created and visible
	rep
		0.01
		if(WaitForSingleObject(mainThreadHandle 0)!=WAIT_TIMEOUT) ret ;;macro ended
		int w=win("Notepad" "Notepad" "" 0x400) ;;check the condition (edit this line)
		if(w) break
	
	 suspend main thread
	SuspendThread(mainThreadHandle)
	
	 do something with the window (replace this code with your code)
	sel mes("Now macro is suspended.[][]OK - close window and resume macro.[]Cancel - end macro." "" "OC")
		case 'O'
		clo w; err
		0.1
		
		case else
		EndThread "" mainThreadHandle
		ret
	
	 resume main thread
	ResumeThread(mainThreadHandle)
