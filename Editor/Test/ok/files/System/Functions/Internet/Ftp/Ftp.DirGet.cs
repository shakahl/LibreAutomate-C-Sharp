function# str&curdir

 Gets current ftp directory.
 Returns: 1 success, 0 failed.

 curdir - variable for result.


int ln=MAX_PATH
BSTR b.alloc(ln)
if(FtpGetCurrentDirectoryW(m_hi b &ln))
	curdir.ansi(b)
	ret 1
Error
