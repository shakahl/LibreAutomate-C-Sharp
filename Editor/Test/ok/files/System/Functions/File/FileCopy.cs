 /
function $from $to [flags] ;;flags: 1 error if exists, 2 to is not a destination folder, 4 don't copy folder content

 Copies file or folder.
 Error if fails.
 Faster than cop (especially when need to copy many small files) but does not have many of cop features: Cannot copy multiple files, auto rename, show a dialog (progress, conflict resolving, error), does not create destination folder, does not copy connected files, does not copy security attributes, etc.

 from - full path of the source file. Must be single file.
 to - full path of the destination file or folder. If it is an existing folder, copies into it, unless flag 2 used.
 flags:
   1 - error if the destination file already exists. Without this flag, at first deletes it.
   2 - if to is existing folder, replace it instead of copying to it.
   4 (QM 2.4.0) - if folder, create empty folder but don't copy files.

 Added in: QM 2.3.0.


opt noerrorshere 1
str s1 s2 s3
sub_sys.FileCopyMove_Prepare(from to flags s1 s2)
BSTR b1(s1) b2(s2)

int attr=GetFileAttributesW(b1); if(attr=-1) goto ge

if !(attr&FILE_ATTRIBUTE_DIRECTORY)
	if(CopyFileExW(b1 b2 0 0 0 COPY_FILE_FAIL_IF_EXISTS)) ret
	goto ge

if(attr&FILE_ATTRIBUTE_REPARSE_POINT and !(flags&4)) mkdir s1 ;;create normal folder, don't copy link
else if(!CreateDirectoryExW(b1 b2 0)) goto ge
if(flags&4) ret

s1+iif(s1.end("\") "*" "\*")
if(!s2.end("\")) s2+"\"
Dir d
foreach d s1 FE_Dir 2|4|32
	lpstr rp=d.RelativePath
	b1=d.FullPath; b2=s3.from(s2 rp)
	if(d.IsFolder) if(!CreateDirectoryExW(b1 b2 0)) goto ge
	else if(!CopyFileExW(b1 b2 0 0 0 COPY_FILE_FAIL_IF_EXISTS)) goto ge
ret
 ge
end ERR_FILECOPY 16
