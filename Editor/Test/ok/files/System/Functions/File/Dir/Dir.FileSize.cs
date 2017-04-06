function'long

 Returns file size (number of bytes).
 If folder, gets sum of sizes of all descendant files. It is slow.


if(!fd) end ERR_INIT

if(fd.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY)
	ret GetFileOrFolderSize(FullPath)
else
	ret 0L+fd.nFileSizeHigh<<32+(0U+fd.nFileSizeLow)
