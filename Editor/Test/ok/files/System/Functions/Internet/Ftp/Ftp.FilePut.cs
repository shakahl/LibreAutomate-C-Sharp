function# $localfile $ftpfile [failifexists] [ascii] [flags] ;;flags: 32 run in other thread

 Uploads file.
 Returns: 1 success, 0 failed.

 localfile - local file to upload.
 ftpfile - ftp file to save to.
 failifexists - if nonzero, fails if ftp file exists.
 ascii - if nonzero, uses ASCII mode. Default: binary.
 flags (QM 2.3.2):
    32 - run in other thread. Then current thread can receive messages, and therefore this function can be used in a dialog.


if(!FileExists(localfile)) lasterror="localfile not found"; ret

str s=localfile
ret FilePutStr(s ftpfile failifexists ascii flags|0x20000)
