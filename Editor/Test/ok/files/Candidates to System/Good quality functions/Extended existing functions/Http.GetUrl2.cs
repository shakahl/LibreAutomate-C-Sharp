function# $url str&s [flags] [inetflags] [dlg] [fa] [fparam] [str&responseheaders] [$headers] ;;flags: 1-3 use cache, 16 download to file, 32 run in other thread

 Downloads web page or other file.
 Returns: 1 success, 0 failed.
 Don't call Connect. This functions connects and disconnects itself.
 You can use <help>IntGetFile</help> instead. Differences: IntGetFile has more flags (4, 8), and throws error instead of returning 0.


if(flags&0x10000) goto g1 ;;thread

if(!Cache(flags inetflags)) ret
SetProgressDialog(dlg); SetProgressCallback(fa fparam)

if(flags&32 or dlg or getopt(waitmsg 1) or GetThreadWindows) ret Thread(0 &url "Downloading" url)
 g1
if(!m_hitop and !Init) ret
__HInternet hi=InternetOpenUrl(m_hitop url headers -1 inetflags 0); if(!hi) ret Error(1)
if(&responseheaders and !GetResponseHeaders(hi responseheaders)) ret Error(1)
ret Read(hi s flags&16)

 note: InternetOpenUrlW does not support Unicode too.
