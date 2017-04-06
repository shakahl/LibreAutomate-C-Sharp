 /Macro1404
function# $cmd [str&data] [ascii] [write]

 Executes FTP command.
 Returns 1 if successful, 0 if failed.

 cmd - FTP command.
 data - str variable that receives server response string (if write is 0), or contains data to write to ftp (if write is 1). Must be omitted or 0 for commands hat don't read or write data.
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


__HInternet h
int usedata(&data!0)
ascii=iif(ascii FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY)
if(!FtpCommandW(m_hi usedata ascii @cmd 0 &h)) ret Error
if(!usedata) ret 1
if(!h) lasterror="Invalid response handle."; ret

str exc
int r n nn bs(1000)
if write
	if(!data.len) ret 1
	int t1=timeGetTime
	 _i=1000
	 out InternetSetOption(h INTERNET_OPTION_WRITE_BUFFER_SIZE &_i 4)
	 int iqo=4
	 if(InternetQueryOption(h INTERNET_OPTION_WRITE_BUFFER_SIZE &_i &iqo)) out _i
	rep
		 bs=iif(nn 1 data.len-1)
		 wbs=data.len
		 int packetSize=data.len-1
		 int packetSize=10000
		n=data.len-nn; if(n>bs) n=bs
		r=InternetWriteFile(h data+nn n &n)
		out "%i %i %i %i" r n nn data.len
		if(!r) Error; break
		 0.1
		nn+n; if(nn>=data.len) break
	 0.001
	out timeGetTime-t1
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

ret r
err+ end "'%s' at '%s'" 0 _error.description _error.line
