 /
function $_file

 Deletes file or folder.
 Error if fails. Error if does not exist.

 _file - full path of the file or folder.

 REMARKS
 Faster than del.
 Does not move to the Recycle Bin.
 Can delete read-only files.
 If folder, deletes all files/folders in it, or as many as possible.

 Added in: QM 2.3.0.


opt noerrorshere 1
BSTR b=_s.expandpath(_file)

int attr=GetFileAttributesW(b); if(attr=-1) goto ge
if(attr&FILE_ATTRIBUTE_READONLY) SetFileAttributesW(b attr&~FILE_ATTRIBUTE_READONLY)

if attr&FILE_ATTRIBUTE_DIRECTORY
	if !(attr&FILE_ATTRIBUTE_REPARSE_POINT)
		ARRAY(str) a; int i
		GetFilesInFolder a _s "" 2|32; err
		for(i 0 a.len) FileDelete a[i]; err ;;delete as many as possible
	
	rep 5
		if(RemoveDirectoryW(b)) ret
		if(GetLastError!=ERROR_DIR_NOT_EMPTY) break ;;see unic::RemoveDirectoryU
		0.01
else
	if(DeleteFileW(b)) ret

 ge
end ERR_FILEDELETE 16
