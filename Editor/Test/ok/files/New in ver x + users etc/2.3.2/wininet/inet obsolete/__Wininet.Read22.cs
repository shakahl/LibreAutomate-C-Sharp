function# hi str&data hfile

int size(4) sizeAll sizeSent prog(m_hdlg or m_fa)
str s; str* sp=iif(hfile &s &data)

if prog
	if(m_isFtp) sizeAll=FtpGetFileSize(hi &_i)
	else if(!HttpQueryInfo(hi HTTP_QUERY_CONTENT_LENGTH|HTTP_QUERY_FLAG_NUMBER &sizeAll &size 0)) sizeAll=-1
	if(!Progress(sizeAll 0 sp)) ret -1

size=4096
rep
	int t=GetTickCount
	byte* b=iif(hfile s.all(size 2) data.all(data.len+size 1)+data.len)
	if(!InternetReadFile(hi b size &size)) ret
	if(!size) break
	if(hfile) if(!WriteFile(hfile s size &size 0)) lasterror.dllerror; ret -2
	else data.fix(data.len+size)
	sizeSent+size
	if(prog and !Progress(sizeAll sizeSent sp)) ret -1
	int td=GetTickCount-t
	 size=MulDiv(size 100 td+10); if(size<4096) size=4096
	if(td<75) size*1.5; if(size>1000000) size=1000000
	else if(td>150) size*0.7; if(size<4096) size=4096
	out "%i %i" td size

ret 1
err+ lasterror.format("'%s' at '%s'" _error.description _error.line); ret -10
