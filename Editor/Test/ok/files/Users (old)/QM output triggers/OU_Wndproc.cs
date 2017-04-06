 /
function# hwnd message wparam lparam

 This function is called on each message sent to QM output window.

if(message=EM_REPLACESEL) ;;text added
	lpstr ss=+lparam; str s
	foreach s ss ;;for each line, because several output strings may be collected for better performance
		if(s.beg("Error ")) ;;if line begins with "Error "
			if(find(s "OU_OnError")<0) ;;prevent triggering from errors in itself
				if(s.beg("Error (RT) ")) ;;if line begins with "Error (RT) "
					mac "OU_OnRtError" "" s
				 else mac "OU_OnError" s
			 case "some other string" ...

ret CallWindowProc(g_ou_pwndproc hwnd message wparam lparam)
