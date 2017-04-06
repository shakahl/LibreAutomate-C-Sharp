 /
function# $action [ARRAY(POSTFIELD)&a] [str&responsepage] [$headers] [fa] [fparam] [inetflags] [str&responseheaders] [flags] ;;flags: 16 download to file, 32 run in other thread

 Posts web form data.
 Returns: 1 success, 0 failed.
 This function is similar to <help>Http.Post</help>, but has more features. Can post files.

 action - script's path relative to server. See "action" field in form's HTML. Example "forum\login.php".
 a - array containing data.
   QM 2.3.2. Can be 0. Then uses data added with <help>Http.PostAdd</help>.
   POSTFIELD members:
      name - field name. Same as "name" field in form's HTML.
      value - field value. Same as "value" field in form's HTML. If it is file field, must specify file.
      isfile - must be 1 for file fields, 0 for other fields.
 responsepage - receives response page (HTML).
 headers - additional headers to send.
 fa - address of <help "Callback_Http_PostFormData">callback function</help>.
   A template is available in menu -> File -> New -> Templates.
 fparam - some value to pass to the callback function.
 inetflags (QM 2.2.1) - INTERNET_FLAG_x flags. Documented in MSDN library. For example, use INTERNET_FLAG_NO_AUTO_REDIRECT to disable redirection. For https use INTERNET_FLAG_SECURE. Flag INTERNET_FLAG_RELOAD is always added.
 responseheaders (QM 2.2.1) - receives raw response headers.
 flags (QM 2.3.2):
    16 - download to file. responsepage must contain file path.
    32 - run in other thread. Then current thread can receive messages, and therefore this function can be used in a dialog.

 REMARKS
 At first call Connect to connect to web server.
 QM 2.3.2. If fails to open file (for sending), returns 0. In earlier versions would throw error.

 EXAMPLES

 ARRAY(POSTFIELD) a.create(2)
 a[0].name="testtxt"; a[0].value="some text"; a[0].isfile=0
 a[1].name="testfile"; a[1].value="$desktop$\test.gif"; a[1].isfile=1
 Http h.Connect("www.xxx.com"); str r
 if(!h.PostFormData("form.php" a r)) end  "failed"
 out r

  the same with PostAdd
 Http h.Connect("www.xxx.com"); str r
 h.PostAdd("testtxt" "some text")
 h.PostAdd("testfile" "$desktop$\test.gif" 1)
 if(!h.PostFormData("form.php" 0 r)) end  "failed"
 out r


if(flags&0x10000=0) if(m_dlg or flags&32) ret Thread(3 &action "Posting" action)

if(!&a) &a=m_ap

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
if(!empty(headers)) s=headers; s.trim("[]"); b.lpcszHeader=sh.from(s "[]" b.lpcszHeader)
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
err+ lasterror.format("'%s' at '%s'" _error.description _error.line)
