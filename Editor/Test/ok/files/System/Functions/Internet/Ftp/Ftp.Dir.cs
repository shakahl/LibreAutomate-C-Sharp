function$ [$_file] [flags] [WIN32_FIND_DATA&info] ;;flags: 0 file, 1 folder, 2 both, 3 previous.

 Finds or enumerates ftp files.
 Returns filename. Returns 0 if not found.

 _file - file name.
   Can contain <help #IDP_WILDCARD>wildcard characters</help> (*?).
   If omitted or "", finds next file that matches previously used filename with wildcard characters.
   Cannot contain spaces.
 info - variable that receives file properties. The same properties are stored in member variable m_fd.fd.

 REMARKS
 Does not support Unicode.


if(!m_fd) m_fd._new
if(getopt(nargs)=0 or flags&3=3) flags=m_fd.flags; else m_fd.flags=flags

if !empty(_file) ;;find first file
	if(m_fd.hfind) InternetCloseHandle(m_fd.hfind)
	m_fd.hfind=FtpFindFirstFile(m_hi _file &m_fd.fd 0 0)
	if(!m_fd.hfind) ret
	 filter
	int isdir=m_fd.fd.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY
	sel(flags)
		case 0: if(isdir) goto next ;;must be file
		case 1: if(!isdir) goto next ;;must be folder
	if(&info) info=m_fd.fd
	ret &m_fd.fd.cFileName ;;and leave open handle for enumeration
else ;;find next file
	if(!m_fd.hfind) ret
	 next
	if(InternetFindNextFile(m_fd.hfind &m_fd.fd)) goto filter
	 nomore
	InternetCloseHandle(m_fd.hfind); m_fd.hfind=0
