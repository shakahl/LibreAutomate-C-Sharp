 /
function# str&s reserved

if &s
	LogFile s
else
	if(FileExists(_logfile)) del- _logfile

ret 1
