 /
function $_file [^waitmax]

 Waits until file exists, is not empty, and is not locked.

 waitmax - max number of seconds to wait. 0 is infinite.


if(waitmax<0 or waitmax>2000000) end ES_BADARG
opt waitmsg -1
int wt(waitmax*1000) t1(GetTickCount)

Dir d
rep
	if d.dir(_file) and d.FileSize
		__HFile f.Create(_file OPEN_EXISTING); err
		if(f) f.Close; break
	
	if(wt>0 and GetTickCount-t1>=wt) end "wait timeout"
	0.5

 info: the d.FileSize is for Firefox, which at first creates empty file, then downloads to other file...
