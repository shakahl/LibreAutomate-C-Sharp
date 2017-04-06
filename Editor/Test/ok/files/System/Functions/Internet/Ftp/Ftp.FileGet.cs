function# $ftpfile $localfile [failifexists] [ascii] [flags] ;;flags: 32 run in other thread

 Downloads file.
 Returns: 1 success, 0 failed.

 ftpfile - ftp file to download.
 localfile - local file to save to.
 failifexists - if nonzero, fails if local file exists.
 ascii - if nonzero, uses ASCII mode. Default: binary.
 flags (QM 2.3.2):
    32 - run in other thread. Then current thread can receive messages, and therefore this function can be used in a dialog.


if(failifexists and FileExists(ftpfile)) lasterror="localfile already exists"; ret

str s=localfile
ret FileGetStr(ftpfile s ascii flags|0x20000)
