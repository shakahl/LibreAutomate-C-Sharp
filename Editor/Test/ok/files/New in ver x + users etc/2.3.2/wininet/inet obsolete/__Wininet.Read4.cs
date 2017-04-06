function# hi str&data [useFile] [hdlg] [fa] [fparam]

__HFile hfile

if(useFile) hfile.Create(data CREATE_ALWAYS GENERIC_WRITE); err this.lasterror=_error.description; ret
else data.all

int r=Read2(hi data hfile hdlg fa fparam)

if r<=0
	if(useFile) hfile.Close; del- data; err
	else data.all
	if(r<0) ret
	Error(1)

ret r
