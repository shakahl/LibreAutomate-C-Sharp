 /
function ^waitmax $_file ;;waitmax: 0 is infinite.

 Function WaitFor is template to create functions that wait for some condition.
 waitmax is max number of seconds to wait. 0 or negative is infinite.

 EXAMPLE
  Function that waits until window becomes invisible:

  /
 function ^waitmax hwnd
 int infinite=waitmax<=0
 opt waitmsg -1
 rep
	 0.2
	 if(!IsWindowVisible(hwnd)) ret
	 if(!infinite) waitmax-0.2; if(waitmax<=0) end "wait timeout"


int infinite=waitmax<=0
opt waitmsg -1
str sf
if(!
rep
	int h=CreateFile(sf
	0.2
	 if(condition) ret
	if(!infinite) waitmax-0.2; if(waitmax<=0) end "wait timeout"
