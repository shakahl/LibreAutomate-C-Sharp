 /
function# Dir&d $pat [flags] [DATE'date1] [DATE'date2] ;;flags: 0 files, 1 folders, 2 any, 4 +subfolders, 8 only subfolders, 16 date-created, 32 skip symbolic-link subfolders, 64 skip hidden-system.

 This function is used with foreach, to enumerate files.
 To insert code for this function, use the "Enumerate files" dialog.

 d - a Dir variable that receives file properties.
 pat - full file path, where the filename can include <help #IDP_WILDCARD>wildcard characters</help> * and ?.
    Examples: "c:\f\*.txt", "c:\f\*", "c:\f\file.txt".
 flags:
   0, 1, 2 - file/folder type. If 0, enumerates only files. If 1 - only folders. If 2 - all.
   4 - also enumerate files in subfolders.
   8 - enumerate files only in subfolders, not in this folder.
   16 - date1 and date2 are time-created.
   32 (QM 2.4.0) - don't enumerate files in subfolders that have attribute FILE_ATTRIBUTE_REPARSE_POINT. It is used for symbolic links, junctions and volume mount points, that actually are not subfolders of that folder.
   64 (QM 2.4.0) - skip hidden system files and folders (eg thumbnail cache files).
 date1, date2 - lower and upper bound of time-modified. If 0 or omitted, there is no limit.


opt noerrorshere 1
int re
long d1 d2
FILETIME ft

if !re
	re=1
	if(flags&8) flags|4
	if(date1) date1.tofiletime(ft); LocalFileTimeToFileTime(&ft +&d1)
	if(date2) date2.tofiletime(ft); LocalFileTimeToFileTime(&ft +&d2)
	if(!d.__e) d.__e=CreateEnumFiles
	d.__e.Begin(pat flags&0xff)

 gNext
if(!d.__e.Next) d.fd=0; ret

if(flags&8 and d.__e.Level<1) goto gNext
if d1 or d2
	WIN32_FIND_DATAU& r=d.__e.Data
	long& k=+iif(flags&16 &r.ftCreationTime &r.ftLastWriteTime)
	if(d1 and k<d1) goto gNext
	if(d2 and k>d2) goto gNext

d.fd=d.__e.Data
ret 1
