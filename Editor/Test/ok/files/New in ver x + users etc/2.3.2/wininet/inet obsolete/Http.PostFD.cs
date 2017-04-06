 /
function# $action !*_a str&responsepage $headers fa fparam inetflags str&responseheaders flags ;;flags: 16 download to file, 32 run in other thread

if(flags&0x10000=0) if(m_dlg or flags&32) ret Thread(3 &action "Posting" action)

ARRAY(POSTFIELD)& a=_a

type ___POSTFILE index __HFile'hfile size str'sf str'sh
type ___POSTPROGRESS fa fparam nbtotal wrtotal

int i size size2 bufsize(4096) wrfile
str s s2 sb bound sh buf
ARRAY(___POSTFILE) af
POSTFIELD& p
___POSTFILE& f
___POSTPROGRESS pp

for(i 0 a.len) if(a[i].isfile) af[].index=i

 set headers
INTERNET_BUFFERS b.dwStructSize=sizeof(b)
bound="[]--7d23542a1a12c2"
b.lpcszHeader="Content-Type: multipart/form-data; boundary=7d23542a1a12c2"
if(len(headers)) s=headers; s.trim("[]"); b.lpcszHeader=sh.from(s "[]" b.lpcszHeader)
b.dwHeadersLength=len(b.lpcszHeader)

 format non-file fields and store into b
for i 0 a.len
	&p=a[i]
	if(p.isfile) continue
	sb.formata("%s[]Content-Disposition: form-data; name=''%s''[][]%s" bound+iif(i 0 2) p.name p.value)
b.lpvBuffer=sb
b.dwBufferLength=sb.len
b.dwBufferTotal=sb.len+bound.len+4

 open files, format headers, calc total data size
for i 0 af.len
	&f=af[i]; &p=a[f.index]
	if(!f.sf.searchpath(p.value)) lasterror.format("file not found: %s" p.value); ret
	if(!GetFileContentType(f.sf s2)) s2="text/plain"
	f.hfile.Create(f.sf OPEN_EXISTING GENERIC_READ FILE_SHARE_READ); err lasterror=_error.description; ret
	f.size=GetFileSize(f.hfile 0)
	b.dwBufferTotal+f.size
	f.sh.format("%s[]Content-Disposition: form-data; name=''%s''; filename=''%s''[]Content-Type: %s[][]" bound+iif(i||sb.len 0 2) p.name f.sf s2)
	b.dwBufferTotal+f.sh.len

 init progress
pp.fa=fa; pp.fparam=fparam; pp.nbtotal=b.dwBufferTotal
if(PostProgress(0 pp)) ret

 open request, send non-file fields
__HInternet hi=HttpOpenRequest(m_hi "POST" action 0 0 0 INTERNET_FLAG_RELOAD|inetflags 0); if(!hi) goto e
if(!HttpSendRequestEx(hi &b 0 0 0)) goto e
pp.wrtotal=sb.len

 send files
if(af.len) buf.all(bufsize)
for i 0 af.len
	&f=af[i]
	if(PostProgress(1 pp f.sf f.size)) ret
	if(!InternetWriteFile(hi f.sh f.sh.len &size2)) goto e
	for wrfile 0 f.size 0
		if(!ReadFile(f.hfile buf bufsize &size 0)) ret
		if(!InternetWriteFile(hi buf size &size2)) goto e
		wrfile+size2; pp.wrtotal+size2
		if(PostProgress(1 pp f.sf f.size wrfile)) ret
	f.hfile.Close

 write last boundary, end request
bound+"--[]"
if(!InternetWriteFile(hi bound bound.len &size2) or size2!=bound.len) goto e
if(!HttpEndRequest(hi 0 0 0)) goto e
if(PostProgress(2 pp)) ret

 get response headers
if(&responseheaders and !GetResponseHeaders(hi responseheaders)) goto e

 read response
if &responsepage
	if(PostProgress(3 pp)) ret
	if(!Read(hi responsepage flags&16)) ret
if(PostProgress(4 pp)) ret
ret 1
 e
Error
