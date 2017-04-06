function# $ftpfile str&data [ascii] [flags] ;;flags: 32 run in other thread

 Downloads file to variable.
 Same as <help>Ftp.FileGet</help>, but stores file data in variable, not in file.


if(flags&0x10000=0) if(m_dlg or flags&32) ret Thread(10 &ftpfile "Downloading" ftpfile)

ascii=iif(ascii FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY)
__HInternet h=FtpOpenFileW(m_hi @ftpfile GENERIC_READ ascii 0); if(!h) ret Error

ret Read(h data flags&0x20000)
