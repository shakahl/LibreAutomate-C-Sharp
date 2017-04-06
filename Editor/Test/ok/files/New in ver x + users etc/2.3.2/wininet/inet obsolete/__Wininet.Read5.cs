function# hi str&data hfile

int size(4) sizeAll sizeSent prog(m_hdlg or m_fa)
str s; str* sp=iif(hfile &s &data)

if prog
	if(m_isFtp) sizeAll=FtpGetFileSize(hi &_i)
	else if(!HttpQueryInfo(hi HTTP_QUERY_CONTENT_LENGTH|HTTP_QUERY_FLAG_NUMBER &sizeAll &size 0)) sizeAll=-1
	if(!Progress(sizeAll 0 sp)) ret -1

rep
	if(!InternetQueryDataAvailable(hi &size 0 0)) ret
	if(!size) break
	if hfile
		s.all(size 2)
		if(!InternetReadFile(hi s size &size)) ret
		if(!WriteFile(hfile s size &size 0)) lasterror.dllerror; ret -2
	else
		data.all(data.len+size 1)
		if(!InternetReadFile(hi data+data.len size &size)) ret
		data.fix(data.len+size)
	sizeSent+size
	if(prog and !Progress(sizeAll sizeSent sp)) ret -1

ret 1
err+ lasterror.format("'%s' at '%s'" _error.description _error.line); ret -10
