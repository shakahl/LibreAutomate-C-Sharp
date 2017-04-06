function# hi str&data [useFile]

__HFile hfile

if(useFile) hfile.Create(data CREATE_ALWAYS GENERIC_WRITE); err this.lasterror=_error.description; ret
else data.all

int r=sub.Read2(hi data hfile)

if r<=0
	if(useFile) hfile.Close; del- data; err
	else data.all
	if(r<0) ret
	Error(1)

ret r


#sub Read2 c
function# hi str&data hfile

int size(4) sizeAll sizeSent prog(m_hdlg or m_fa)

if prog
	if(m_isFtp) sizeAll=FtpGetFileSize(hi &_i)
	else if(!HttpQueryInfo(hi HTTP_QUERY_CONTENT_LENGTH|HTTP_QUERY_FLAG_NUMBER &sizeAll &size 0)) sizeAll=-1
	if(_i or sizeAll<0) sizeAll=-1 ;;does not support files >= 2 GB
	if(!Progress(sizeAll 0 &data)) ret -1

if hfile
	int na nb nbMax=8192
	str sb.all(nbMax)

rep
	if(!InternetQueryDataAvailable(hi &size 0 0)) ret
	if(!size) break
	if hfile
		na=size
		rep ;;read in parts, because size may be too big, eg when from cache
			nb=na; if(nb>nbMax) nb=nbMax
			if(!InternetReadFile(hi sb nb &nb)) ret
			if(!WriteFile(hfile sb nb &_i 0)) lasterror.dllerror; ret -2
			na-nb; if(na<=0) break
	else
		data.all(data.len+size 1)
		if(!InternetReadFile(hi data+data.len size &size)) ret
		data.fix(data.len+size)
	sizeSent+size
	if(prog and !Progress(sizeAll sizeSent &data)) ret -1

ret 1
err+ lasterror.format("'%s' at '%s'" _error.description _error.line); ret -10
