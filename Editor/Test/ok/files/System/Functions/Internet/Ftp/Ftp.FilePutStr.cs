function# str&data $ftpfile [failifexists] [ascii] [flags] ;;flags: 32 run in other thread

 Uploads file from variable.
 Same as <help>Ftp.FilePut</help>, but the source data is in variable, not in file.


if(flags&0x10000=0) if(m_dlg or flags&32) ret Thread(11 &ftpfile-4 "Uploading" ftpfile)

if(failifexists and Dir(ftpfile)) lasterror="ftpfile already exists"; ret

ascii=iif(ascii FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY)
__HInternet h=FtpOpenFileW(m_hi @ftpfile GENERIC_WRITE ascii 0); if(!h) ret Error

ret Write(h data flags&0x20000)
