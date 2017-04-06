function!

 Returns 1 if folder, 0 if file.


if(!fd) end ERR_INIT

ret fd.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY!=0
