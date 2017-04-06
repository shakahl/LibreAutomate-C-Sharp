 /
function [^waitMax] [moreParameters] ;;waitmax: 0 is infinite.

 Function WaitFor is template to create functions that wait for some condition.

 waitmax - max time to wait, in seconds. 0 is infinite. Error on timeout.


if(waitMax<0 or waitMax>2000000) end ERR_BADARG
opt waitmsg -1

int wt(waitMax*1000) t0(GetTickCount)
rep
	0.1
	
	 here insert code that checks for some condition
	 and returns (ret or break) if it is true
	
	if(wt and GetTickCount-t0>=wt) end ERR_TIMEOUT
