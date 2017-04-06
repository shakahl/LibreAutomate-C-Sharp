function'DATE

 Returns last write time in DATE format.
 For folders returns creation time.
 Gets local time, not UTC.


if(!fd) end ERR_INIT

FILETIME t; DATE d
FileTimeToLocalFileTime &fd.ftLastWriteTime &t
ret d.fromfiletime(&t)
