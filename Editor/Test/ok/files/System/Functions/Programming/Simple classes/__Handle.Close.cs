 Calls CloseHandle and clears this variable.

if handle
	if(handle!=-1) CloseHandle(handle)
	handle=0
