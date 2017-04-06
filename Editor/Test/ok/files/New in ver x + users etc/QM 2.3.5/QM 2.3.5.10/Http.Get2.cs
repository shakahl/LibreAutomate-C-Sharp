function# $remoteFile str&data [flags] [inetFlags] [str&responseHeaders] [$sendHeaders] ;;flags: 1-3 cache flags, 16 download to file, 32 run in other thread

 Downloads web page or other file.
 Returns: 1 success, 0 failed.

 remotefile - file to download, relative to server. Examples: "index.htm", "images/earth.jpg".
 data - variable that receives file data. If flag 16 - variable that contains local file name.
 flags, inetflags, responseheaders - see IntGetFile. Does not support flags: 4, 8.

 REMARKS
 At first call Connect to connect to web server.

 Added in: QM 2.3.2.

 See also: <IntGetFile>


if(flags&0x10000) goto g1 ;;thread

if(!Cache(flags inetFlags)) ret

if(flags&0x10000=0) if(m_dlg or flags&32) ret Thread(1 &remoteFile "Downloading" remoteFile)
 g1
__HInternet hi=HttpOpenRequest(m_hi "GET" remoteFile 0 0 0 inetFlags 0); if(!hi) ret Error
if(!HttpSendRequest(hi sendHeaders -1 0 0)) ret Error
if(&responseHeaders and !GetResponseHeaders(hi responseHeaders)) ret Error
ret Read(hi data flags&16)

 note: HttpOpenRequestW does not support Unicode too.
