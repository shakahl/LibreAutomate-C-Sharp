function# hi str&data [useFile] [hdlg] [fa] [fparam]

int size sizeAll sizeSent bs(4096) prog(hdlg or fa)
str s; str* sp=iif(useFile &s &data)
__HFile hfile

if useFile
	hfile.Create(data OPEN_EXISTING GENERIC_READ FILE_SHARE_READ); err this.lasterror=_error.description; ret
	s.all(bs)

if prog
	sizeAll=iif(useFile GetFileSize(hfile 0) data.len)
	if(!Progress(hdlg sizeAll 0 fa fparam sp)) ret

rep
	if useFile
		if(!ReadFile(hfile s bs &size 0)) lasterror.dllerror; ret
		if(!size) break
		if(!InternetWriteFile(hi s size &size)) goto g1
	else
		if(sizeSent>=data.len) break
		size=data.len-sizeSent; if(size>bs) size=bs
		if(!InternetWriteFile(hi data+sizeSent size &size)) goto g1
	sizeSent+size
	if(prog and !Progress(hdlg sizeAll sizeSent fa fparam sp)) ret

ret 1
 g1
Error(1)
err+ lasterror.format("'%s' at '%s'" _error.description _error.line)
