function# $localfile $ftpfile [failifexists] [ascii]

if(!dir(localfile)) lasterror="localfile not found"; ret
if(failifexists and Dir(ftpfile)) lasterror="ftpfile already exists"; ret

InternetSetStatusCallback(m_hi &Ftp_Callback)

ascii=iif(ascii FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY)
if(FtpPutFileW(m_hi @_s.expandpath(localfile) @ftpfile ascii 2)) ret 1
Error
