function'DATE

 Returns last access time in DATE format.
 Gets local time, not UTC.


if(!fd) end ERR_INIT

FILETIME t; DATE d
FileTimeToLocalFileTime &fd.ftLastAccessTime &t
ret d.fromfiletime(&t)
