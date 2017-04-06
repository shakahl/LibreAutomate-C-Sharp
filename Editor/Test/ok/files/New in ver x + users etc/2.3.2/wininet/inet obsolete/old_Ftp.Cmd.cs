function# $cmd [str&data] [ascii] [write]

 Executes FTP command.
 Returns 1 if successful, 0 if failed.

 cmd - FTP command.
 data - str variable that receives server response string (if write is 0), or contains data to write to ftp (if write is 1).
 ascii - 1 ASCII mode, 0 binary mode.
 write - if nonzero, writes data to ftp.

 This function is not reliable. Even if used properly, it often hangs macro or reports error.

 REFERENCES
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


int h
str exc
ascii=iif(ascii FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY)
if(!FtpCommandW(m_hi &data!0 ascii @cmd 0 &h)) ret Error
if(!&data) ret 1
if(!h) lasterror="Invalid response handle."; ret

int r n nn
if(write)
	rep
		r=InternetWriteFile(h data+nn data.len-nn &n)
		if(!r) Error; break
		nn+n; if(nn>=data.len) break
else
	rep
		r=InternetQueryDataAvailable(h &n 0 0)
		if(!r) Error; break
		if(n)
			data.all(data.len+n 1)
			r=InternetReadFile(h data+data.len n &n)
			if(!r) Error; break
		data.fix(data.len+n)
		if(n=0) break

err+
	exc.format("'%s' at '%s'" _error.description _error.line)
	r=0
InternetCloseHandle(h)
if(exc.len) end exc
ret r
