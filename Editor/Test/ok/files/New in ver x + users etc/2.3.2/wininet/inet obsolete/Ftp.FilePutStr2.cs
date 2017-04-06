function# str&data $ftpfile [failifexists] [ascii]

 Same as Ftp.FilePut, but the source data is in variable, not in file.


if(failifexists and Dir(ftpfile)) lasterror="ftpfile already exists"; ret

ascii=iif(ascii FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY)
__HInternet h=FtpOpenFileW(m_hi @ftpfile GENERIC_WRITE ascii 0); if(!h) ret Error
if(!data.len) ret 1

int r n nn bs(4096)
rep
	n=data.len-nn; if(n>bs) n=bs
	r=InternetWriteFile(h data+nn n &_i)
	if(!r or _i!=n) Error; break
	nn+n; if(nn>=data.len) break

ret r
err+ end "'%s' at '%s'" 0 _error.description _error.line
