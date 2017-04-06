function# $cmd [str&data] [ascii] [_]

 Executes FTP command.
 Returns: 1 success, 0 failed.

 cmd - FTP command.
 data - str variable that receives data (eg file) from ftp. Must be omitted or 0 with commands hat don't receive data.
 ascii - 1 ASCII mode, 0 binary mode.
 _ - if nonzero, writes data to ftp.
    This is for backward compatibility.
    This function should not be used with commands that send data to ftp (STOR, APPE).
    It uses API function FtpCommand, which does not support sending data properly:
       1. May store partial file, because sends asynchronously, and there is no way to know when sending is complete.
       2. Most other Ftp functions stop working in that connection.

 REMARKS
 This function is not reliable. With some commands it may fail, hang, or behave differently with different OS/SP/IE versions.

 REFERENCES
 <link>http://www.nsftools.com/tips/RawFTP.htm</link>
 <link>http://cr.yp.to/ftp.html</link>
 <link>http://www.faqs.org/rfcs/rfc959.html</link>

 EXAMPLES
  Change file permissions (chmod):
 Ftp f.Connect("ftp.x.com" "user" "password")
 f.DirSet("public_html")
 f.Cmd("SITE chmod 644 test.txt")

  Get ftp current directory contents.
 str s
 if(f.Cmd("LIST" &s)) out s


__HInternet h
int usedata=&data!0
ascii=iif(ascii FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY)
if(!FtpCommandW(m_hi usedata ascii @cmd 0 &h)) ret Error
if(!usedata) ret 1
if(!h) lasterror="Invalid response handle."; ret

int r n nn bs(4096)
if _ ;;fbc
	if(!data.len) ret 1
	rep
		n=data.len-nn; if(n>bs) n=bs
		r=InternetWriteFile(h data+nn n &n)
		if(!r) Error; break
		nn+n; if(nn>=data.len) break
	1 ;;make more reliable. InternetWriteFile is async; InternetCloseHandle aborts the transfer without completing (although completes if the handle is opened not with FtpCommand); there is no way to know when completed.
else
	data.fix(0)
	rep
		data.all(data.len+bs 1)
		r=InternetReadFile(h data+data.len bs &n)
		if(!r) Error; break
		if(n=0) break
		data.fix(data.len+n)
	 info: don't use InternetQueryDataAvailable. It does not work eg after Dir.

ret r
err+ end F"'{_error.description}' at '{_error.line}'"
