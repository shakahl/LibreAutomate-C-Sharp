function# $localfile [$ftpfile] [flags] [fa] [fparam] ;;flags: 1 fail if exists, 2 don't check if exists, 3 msgbox if exists, 4 ascii

 Uploads localfile to ftp server. Similar to Ftp.FilePut, but here you can use a callback function.
 Returns 1 on success, -1 on cancel (if callback function returns 1). On failure returns 0.

 localfile - file to upload.
 ftpfile - destination file. If "", gets filename from localfile.
 flags:
   0-3 - what to do if the file already exists: 0 delete, 1 fail, 2 don't check (faster), 3 message box.
   4 - use ASCII mode. By default, uses binary mode.
 fa - address of callback function that will be called repeatedly while uploading. Can be used to show progress.
   function# nbAll nbRead fparam
   Arguments:
     nbAll - file size.
     nbRead - the number of bytes uploaded so far.
   Return value: 0 continue, 1 cancel.
 fparam - some value to pass to the callback function.


if(!dir(localfile)) ret
if(!len(ftpfile)) ftpfile=_s.getfilename(localfile 1)
if(flags&3!=2 and Dir(ftpfile))
	sel(flags&3)
		case 0 FileDel(ftpfile)
		case 1 out "%s already exists" ftpfile; ret
		case 3 if(mes("%s[][]This file already exists on the FTP server. Overwrite?" "" "OC!" ftpfile)='O') FileDel(ftpfile); else ret -1

__HInternet hi=FtpOpenFile(m_hi ftpfile GENERIC_WRITE iif(flags&4 FTP_TRANSFER_TYPE_ASCII FTP_TRANSFER_TYPE_BINARY) 0)
if(!hi) goto e

__HFile hfile.Create(localfile OPEN_EXISTING GENERIC_READ FILE_SHARE_READ); err out _error.description; ret
int fsize=GetFileSize(hfile 0)
if(fa and call(fa fsize 0 fparam)) ret -1

int wrfile size size2 bufsize(2*1024)
str buf.all(bufsize)
for wrfile 0 fsize 0
	if(!ReadFile(hfile buf bufsize &size 0)) ret
	if(!InternetWriteFile(hi buf size &size2) or size2!=size) goto e
	wrfile+size2
	if(fa and call(fa fsize wrfile fparam))
		InternetCloseHandle hi; hi=0
		FileDel(ftpfile)
		ret -1
	0

ret 1
 e
Error
