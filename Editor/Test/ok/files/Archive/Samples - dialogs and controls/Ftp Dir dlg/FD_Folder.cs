 /FtpDirDlg
function FTPDIRDATA&f hparent [$ftpdir]

if(!empty(ftpdir)) if(!f.ftp.DirSet(ftpdir)) ret

ARRAY(str) a
int i j
lpstr se

WIN32_FIND_DATA fd
lpstr s=f.ftp.Dir("*" 2 &fd)
if(!empty(ftpdir)) TvAdd f.htv hparent ".." 0 f.ifoldericon
rep
	if(s=0) break
	
	if SelStr(0 s "." "..")
	else if(fd.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY) TvAdd f.htv hparent s 0 f.ifoldericon
	else a[]=s
	
	s=f.ftp.Dir("" 2 &fd)

for i 0 a.len
	 out s
	s=a[i]
	
	 get icon index from file extension
	j=findcr(s '.'); if(j>=0) se=s+j; else se=".~unknown"
	SHFILEINFO fi
	SHGetFileInfo(se 0 &fi sizeof(fi) SHGFI_SYSICONINDEX|SHGFI_USEFILEATTRIBUTES|SHGFI_SMALLICON)
	
	TvAdd f.htv hparent s 0 fi.iIcon
