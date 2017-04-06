 /Macro2148
function# Dir&dr $pat [flags] [DATE'date1] [DATE'date2] ;;flags: 0 files, 1 folders, 2 any, 4 include subfolders, 8 only subfolders, 16 datecreated, 32 skip hidden.

 This function is used with foreach, to enumerate files.
 To insert code for this function, use the "Enumerate files" dialog.

 dr - receives file properties.
 pat - folder path and filename pattern with wildcard characters, like "c:\f\*.txt".
 flags:
   0, 1, 2 - file/folder filter. If 0, enumerates only files. If 1 - only folders. If 2 - all.
   4 - also enumerate files in subfolders.
   8 - enumerate files only in subfolders, not in this folder.
   16 - date1 and date2 are time created.
 date1, date2 - lower and upper bound of time modified. If 0 or omitted, there is no limit.


type ___FE_DIR Dir'd re ~s1
ARRAY(___FE_DIR) a
int re level sub dc
long d1 d2
lpstr s
str sfn s2
___FE_DIR& f

if(!re)
	if(empty(pat)) ret
	re=1
	s2.flags=1
	a.create(1)
	if(date1) FILETIME ft; date1.tofiletime(ft); LocalFileTimeToFileTime(&ft +&d1); else d1=0
	if(date2) date2.tofiletime(ft); LocalFileTimeToFileTime(&ft +&d2); else d2=0
	if(flags&16) flags~16; dc=1
	if(flags&12)
		sub=1
		flags~4
		sfn.getfilename(pat 1)
		if(flags&8)
			flags~8
			&f=&a[level]
			f.d.__path=pat
			goto g2
 g1
&f=&a[level]

if(!f.re) f.re=1; s=f.d.dir(pat flags)
else
	 g3
	s=f.d.dir

if(s)
	 out F"<><c 0xff0000>{s}</c>"
	if(flags&32)
		 if(f.d.fd.dwFileAttributes&FILE_ATTRIBUTE_HIDDEN) out F"HIDDEN: {s}"; goto g3
		 if(f.d.fd.dwFileAttributes&FILE_ATTRIBUTE_HIDDEN) out F"HIDDEN: {s}"; goto g3
		if(f.d.fd.dwFileAttributes&FILE_ATTRIBUTE_REPARSE_POINT) out F"SYMLINK: {s}"; deb; goto g3
	if(d1 or d2)
		long& l=+iif(dc &f.d.fd.ftCreationTime &f.d.fd.ftLastWriteTime)
		if(d1 and l<d1) goto g3
		if(d2 and l>d2) goto g3
	dr=f.d; ret 1

if(!sub) ret

 g2
if(!f.s1.len)
	f.s1.getpath(f.d.__path); f.s1+"*"
	s=f.d.dir(f.s1 1)
else s=f.d.dir

if(s)
	pat=s2.fromn(f.s1 f.s1.len-1 s -1 "\" 1 sfn sfn.len)
	level=a.redim(-1)
	goto g1

if(level)
	a.redim(level); level-1
	&f=&a[level]
	goto g2
