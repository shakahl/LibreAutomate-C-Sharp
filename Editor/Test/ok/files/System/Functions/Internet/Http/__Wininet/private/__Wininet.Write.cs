function# hi str&data [useFile]

int size sizeAll sizeSent bs(4096) prog(m_hdlg or m_fa)
__HFile hfile

if useFile
	hfile.Create(data OPEN_EXISTING GENERIC_READ FILE_SHARE_READ); err this.lasterror=_error.description; ret
	str sb.all(bs)

if prog
	sizeAll=iif(useFile GetFileSize(hfile 0) data.len)
	if(!Progress(sizeAll 0 &data)) ret

rep
	if useFile
		if(!ReadFile(hfile sb bs &size 0)) lasterror.dllerror; ret
		if(!size) break
		if(!InternetWriteFile(hi sb size &size)) goto g1
	else
		if(sizeSent>=data.len) break
		size=data.len-sizeSent; if(size>bs) size=bs
		if(!InternetWriteFile(hi data+sizeSent size &size)) goto g1
	sizeSent+size
	if(prog and !Progress(sizeAll sizeSent &data)) ret

ret 1
 g1
Error(1)
err+ lasterror.format("'%s' at '%s'" _error.description _error.line)
