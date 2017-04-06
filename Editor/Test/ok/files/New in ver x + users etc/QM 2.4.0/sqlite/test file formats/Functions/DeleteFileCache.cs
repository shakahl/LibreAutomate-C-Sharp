 /
function $_file

 Deletes the memory cache of _file so that next time a file read function would get data from disk (and create new cache).


opt noerrorshere 1
__HFile f.Create(_file OPEN_EXISTING GENERIC_READ 0 FILE_FLAG_NO_BUFFERING)
FlushFileBuffers f
