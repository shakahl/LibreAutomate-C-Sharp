function# $ftpfile str&data [ascii]

 Same as Ftp.FileGet, but stores file data in variable, not in file.


ascii=iif(ascii FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY)
__HInternet h=FtpOpenFileW(m_hi @ftpfile GENERIC_READ ascii 0); if(!h) ret Error

int r n bs(4096)
data.fix(0)
rep
	data.all(data.len+bs 1)
	r=InternetReadFile(h data+data.len bs &n)
	if(!r) Error; break
	if(n=0) break
	data.fix(data.len+n)

ret r
err+ end "'%s' at '%s'" 0 _error.description _error.line
