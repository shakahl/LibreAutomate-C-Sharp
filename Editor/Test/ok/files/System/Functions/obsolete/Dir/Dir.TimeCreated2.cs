function'DATE

 Returns creation time in DATE format.
 Gets local time, not UTC.


if(!fd) end ERR_INIT

FILETIME t; DATE d
FileTimeToLocalFileTime &fd.ftCreationTime &t
ret d.fromfiletime(&t)
