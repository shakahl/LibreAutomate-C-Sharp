 /
function $url str&s [flags] [inetflags] [dlg] [fa] [fparam] [str&responseheaders] [$headers] ;;flags: 1-3 cache flags, 4 go online, 8 go online dialog, 16 download to file (s is file name), 32 run in other thread.

 Downloads web page or other file from the Internet.
 Error if failed.

 url - file to download. Example: "http://www.x.com/index.htm"
 s - variable that receives file data. If flag 16 used - variable that contains local file path.
 flags:
   1 try to retrieve the file from cache, unless the original file is modified.
   2 try to retrieve the file from cache, unless the cached file is expired.
   3 if not online, try to retrieve from cache.
   4 if not online, go online.
   8 if not online, show 'go online' dialog.
   16 download to file. Variable s must contain file path.
   32 (QM 2.3.2) run in other thread.
 inetflags - INTERNET_FLAG_x flags. <google "site:microsoft.com INTERNET_FLAG_NO_AUTO_REDIRECT">Reference</google>. For example, use INTERNET_FLAG_NO_AUTO_REDIRECT to disable redirection. For https use INTERNET_FLAG_SECURE.
 dlg - <help "__Wininet.SetProgressDialog">progress dialog</help>.
 fa - address of <help "Callback_internet_progress">progress callback function</help>, same as with <help "__Wininet.SetProgressCallback">SetProgressCallback</help>.
   A template is available in menu -> File -> New -> Templates.
 fparam - some value to pass to the callback function.
 responseheaders (QM 2.2.1) - variable that receives raw response headers.
 headers (QM 2.4.3) - additional headers to send.

 REMARKS
 If none of flags 1, 2 and 3 is set, cache is not used. When using flag 1 or 2, downloaded file is stored to cache.
 The function fails if the file must be downloaded while offline.

 Runs in other thread if used flag 32, or dlg, or opt waitmsg 1, or current thread has windows.
 When runs in other thread, current thread can receive messages, and therefore this function can be used in a dialog.

 This function simply downloads the file. It does not execute scripts. To execute scripts, instead use class <help "HtmlDoc help">HtmlDoc</help> or web browser control.

 See also: <Http help>.


int ol=1
if(flags&8) ol=IntGoOnline(2)
else if(flags&4) ol=IntGoOnline(1)
else if(flags&3=0) ol=IntIsOnline
if(!ol) end ERR_OFFLINE
err+ end _error

opt waitmsg -1
Http http
if(!http.GetUrl(url s flags inetflags dlg fa fparam responseheaders headers)) end F"{ERR_FAILED}  {http.lasterror}"
