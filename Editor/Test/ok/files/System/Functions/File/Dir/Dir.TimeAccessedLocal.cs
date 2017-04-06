function'DateTime

 Gets last access time, local.

 REMARKS
 File access time auto-updating on most computers is disabled.

 Added in: QM 2.4.1.


if(!fd) end ERR_INIT
FILETIME t; FileTimeToLocalFileTime &fd.ftLastAccessTime &t
DateTime& R=+&t
ret R
