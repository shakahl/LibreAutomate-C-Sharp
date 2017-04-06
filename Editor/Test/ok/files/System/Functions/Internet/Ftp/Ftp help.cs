 To use FTP functions, declare variable of type Ftp, and call its member function Connect.
 Calling Disconnect is optional. It is implicitly called when variable goes out of scope.
 By default, Connect on error ends macro. Most other Ftp functions return 1 on success, 0 if failed.
 If a function failed, error description in most cases is stored in member variable lasterror.

 To separate ftp path parts use /.
 In some cases, ftp file name or path must be relative to current ftp directory.

 EXAMPLES

 1. Download file.

 Declare variable and start FTP session
Ftp f.Connect("ftp.server.com" "myname" "password")

 If need, change current ftp directory to the directory where the file is
if(!f.DirSet("public_html/zipfiles")) end "failed to set FTP current directory"

 Download to desktop
f.SetProgressDialog(1)
if(!f.FileGet("filename.zip" "$desktop$\filename.zip")) end "failed to download"

mes "downloaded"

 ____________________________________

 2. Enumerate ftp files and upload file.

 Declare variable and start FTP session:
Ftp f2.Connect("ftp.server.com" "myname" "[*074A294684CB7360*]")

 Change current ftp directory:
if(!f2.DirSet("public_html/images")) end "failed to set FTP current directory"

 Enumerate files and folders in current ftp directory:
WIN32_FIND_DATA fd
lpstr s=f2.Dir("*" 2 &fd) ;;get first filename
rep
	if(s=0) break
	if(fd.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY) out "%s (directory)" s; else out s
	s=f2.Dir("" 2 &fd) ;;get next filename

 Upload file:
if(!f2.FilePut("c:\my documents\my pictures\one.gif" "one.gif")) mes "FTP error: 'one.gif' upload failed."

 End session:
mes "FTP session finished."
