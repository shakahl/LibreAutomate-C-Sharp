str s
DATE d
if(!GetScheduledTaskStatus("notepad" 0 d s)) end "task does not exist"
if(s.len)
	out "could not start, %s" s
else
	out "successfully started at %s" _s.from(d)
