 /
function ARRAY(COMPAREFIF)&a $folder1 $folder2 [$onlyFiles] [$notFiles] [flags] ;;flags: 4 +subfolders, 8 only subfolders, 32 skip symbolic-link subfolders, 64 skip hidden-system, 0x100 add equal

 Compares time and size of files in two folders, and gets paths of files that are different.
 Can be used for example to backup or synchronize files.

 a - array variable for results.
   COMPAREFIF members:
     f1, f2 - full path of file in folder1 and folder2. Both are not empty even if the file is missing in one of folders.
     flags: 1 f1 newer (modified later than f2), 2 f2 newer, 4 f1 missing, 8 f2 missing, 0x100 f1 and f2 are equal.
 folder1, folder2 - folders.
 onlyFiles - list of files to compare. Can be used wildcard characters. Example: "file.exe[]*.dll". Default: "*" (all files).
 notFiles - list of files to skip. Can be used wildcard characters. Example: "file1.dll[]file2.dll".
    If a string in onlyFiles or notFiles begins with \, compares whole relative path, else just filename. Example: "\Folder\*".
 flags:
    4, 8, 32, 64 - same as with <help>GetFilesInFolder</help>.
    0x100 - add equal files to a too.

 REMARKS
 Compares only files, not folders.
 Does not compare file data. It would be slow. Compares only the 'modified' time and file size. It is reliable enough.

 Added in: QM 2.3.5.

 EXAMPLE
 ARRAY(COMPAREFIF) a
 CompareFilesInFolders a "c:\folder" "d:\folder" "*.exe[]*.dll" "\SkipThisFolder\*" 0x104
 int i
 for i 0 a.len
	 COMPAREFIF& x=a[i]
	 out F"0x{x.flags}  {x.f1}  {x.f2}"


a=0
str f1.expandpath(folder1) f2.expandpath(folder2)
if(empty(onlyFiles)) onlyFiles="*"

type ___CFIF ~f %tm %sz
ARRAY(___CFIF) a1 a2
ARRAY(str) ao(onlyFiles) an(notFiles)
int i j

 get all file paths etc from both folders into a1 and a2
for j 0 2
	str& fr=iif(j f2 f1)
	ARRAY(___CFIF)& ar=iif(j a2 a1)
	int all=ao.len>1 or ao[0].beg("\")
	Dir d
	foreach d F"{fr}\{iif(all `*` ao[0])}" FE_Dir flags&(4|8|32|64)
		lpstr sn=d.FileName
		lpstr srel=d.RelativePath
		lpstr s1 s2
		if all
			for(i 0 ao.len)
				s2=ao[i]; if(s2[0]='\') s2+1; s1=srel; else s1=sn
				if(matchw(s1 s2 1)) break
			 if(i=ao.len) out "<><c 0x8000>NOT IN onlyFiles: %s</c>" sn
			if(i=ao.len) continue
		if an.len
			for(i 0 an.len)
				s2=an[i]; if(s2[0]='\') s2+1; s1=srel; else s1=sn
				if(matchw(s1 s2 1)) break
			 if(i<an.len) out "<><c 0xff>IN notFiles: %s</c>" sn
			if(i<an.len) continue
		___CFIF& r=ar[]
		r.f=srel
		r.tm=d.TimeModifiedUTC
		r.sz=d.FileSize
		 out r.f
	 out "-------"

 compare
for i 0 a1.len
	for(j 0 a2.len) if(a1[i].f~a2[j].f) break
	int equal(0) fl(0)
	if(j=a2.len) fl|8 ;;missing in f2
	else if(a1[i].tm>a2[j].tm) fl|1
	else if(a1[i].tm<a2[j].tm) fl|2
	else if(a1[i].sz=a2[j].sz)
		if(flags&0x100) fl|0x100
		else equal=1
	if !equal
		COMPAREFIF& x=a[]
		x.f1.from(f1 "\" a1[i].f)
		x.f2.from(f2 "\" a1[i].f)
		x.flags=fl
	if(j<a2.len) a2[j].f[0]=0 ;;mark as processed
 add files that are in f2 but not in f1
for i 0 a2.len
	if a2[i].f[0]
		&x=a[]
		x.f1.from(f1 "\" a2[i].f)
		x.f2.from(f2 "\" a2[i].f)
		x.flags=4
