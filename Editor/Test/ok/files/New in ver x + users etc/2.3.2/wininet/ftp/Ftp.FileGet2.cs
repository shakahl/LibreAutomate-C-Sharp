function# $ftpfile $localfile [failifexists] [ascii]

InternetSetStatusCallback(m_hi &Ftp_Callback)

ascii=iif(ascii FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY)
if(FtpGetFileW(m_hi @ftpfile @_s.expandpath(localfile) failifexists!0 FILE_ATTRIBUTE_NORMAL ascii 1)) ret 1
Error
