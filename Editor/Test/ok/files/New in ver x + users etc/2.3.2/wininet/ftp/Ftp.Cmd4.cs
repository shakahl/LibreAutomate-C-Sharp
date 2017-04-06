 /Macro1404
function# $cmd [str&data] [ascii] [write]

 Executes FTP command.
 Returns 1 if successful, 0 if failed.

 cmd - FTP command.
 data - str variable that receives data (eg file) from ftp (if write is 0), or contains data to write to ftp (if write is 1). Must be omitted or 0 for commands hat don't read or write data.
 ascii - 1 ASCII mode, 0 binary mode.
 write - if nonzero, writes data to ftp.

 This function is not reliable. Even if used properly, it often hangs macro or reports error.

 REFERENCES
 <link>http://www.nsftools.com/tips/RawFTP.htm</link>
 <link>http://cr.yp.to/ftp.html</link>
 <link>http://www.faqs.org/rfcs/rfc959.html</link>

 EXAMPLES
  Change file permissions (chmod):
 Ftp f.Connect("ftp.x.com" "user" "[*074A291684CB7360*]")
 f.DirSet("public_html")
 f.Cmd("SITE chmod 644 test.txt")

  Get ftp current working directory contents.
 str s
 if(f.Cmd("LIST" &s)) out s

  Upload test.txt to ftptest.txt, then append string to ftptest.txt.
 str ss.getfile("$desktop$\test.txt")
 f.Cmd("STOR ftptest.txt" ss 1 1)
 ss="append this"
 f.Cmd("APPE ftptest.txt" ss 1 1)


__HInternet h
int usedata(&data!0)
ascii=iif(ascii FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY)
if(!FtpCommandW(m_hi usedata ascii @cmd 0 &h)) ret Error
if(!usedata) ret 1
if(!h) lasterror="Invalid response handle."; ret

 InternetSetStatusCallback(h &Ftp_Callback)

str exc
int r n nn bs(4096)
 int r n nn bs(4096000)
if write
	if(!data.len) ret 1
	int t1=timeGetTime
	int lrh
	rep
		n=data.len-nn; if(n>bs) n=bs
		r=InternetWriteFile(h data+nn n &n)
		out "%i %i %i %i" r n nn data.len
		if(!r) Error; break
		nn+n; if(nn>=data.len) break
	out timeGetTime-t1
	2
else
	data.fix(0)
	rep
		data.all(data.len+bs 1)
		r=InternetReadFile(h data+data.len bs &n)
		if(!r) Error; break
		if(n=0) break
		data.fix(data.len+n)
	 info: don't use InternetQueryDataAvailable. It does not work eg after Dir.

 opt waitmsg 1
 1
 out GetCurrentThreadId
 out InternetWriteFile(h "" 0 &n)
 if(!FtpCommand(m_hi 0 0 "NOOP" 1 &_i))
	 if(InternetGetLastResponseInfo(&n _s &nn)) out "%i %s" n _s
 out InternetQueryDataAvailable(h &n 0 0)
 out InternetReadFile(h _s.all(1) 1 &n)
 out h
 h=0
 InternetCloseHandle(h); h=0
 2
 WIN32_FIND_DATA fd
 if(this.Dir("test.txt" 0 fd)) out fd.cFileName; else Error; out lasterror

ret r
err+ end "'%s' at '%s'" 0 _error.description _error.line
