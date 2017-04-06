function! msg NOTIFYICONDATAW*nd [timeout] ;;timeout - number of seconds to wait until succeeds

timeout*2
rep
	if(Shell_NotifyIconW(msg nd)) ret 1
	if(!timeout) ret
	timeout-1
	0.5

 info: Sometimes random error at Windows startup, eg ERROR_NO_TOKEN. Succeeds eg after 1 s.
