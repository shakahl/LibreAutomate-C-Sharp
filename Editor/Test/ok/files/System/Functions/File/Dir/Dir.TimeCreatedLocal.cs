function'DateTime

 Gets creation time, local.

 Added in: QM 2.4.1.


if(!fd) end ERR_INIT
FILETIME t; FileTimeToLocalFileTime &fd.ftCreationTime &t
DateTime& R=+&t
ret R
