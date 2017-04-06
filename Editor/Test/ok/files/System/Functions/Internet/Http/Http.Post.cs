function# $action $data [str&responsepage] [$headers] [inetflags] [str&responseheaders] [flags] ;;flags: 16 download to file, 32 run in other thread

 Posts web form data.
 Returns: 1 success, 0 failed.
 This function cannot post files. Use PostFormData.

 data - string containing field names and values. Syntax: "name1=value1&name2=value2&...". Values must be urlencoded. Example: "name=John%20Smith&email=j.smith@xxx.com".
 action, responsepage, headers, inetflags, responseheaders, flags (QM 2.3.2) - the same as with PostFormData. For https use INTERNET_FLAG_SECURE in inetflags.

 See also: <Http.PostFormData>.


if(flags&0x10000) goto g1 ;;thread
if(flags&0x10000=0) if(m_dlg or flags&32) ret Thread(2 &action "Posting" action)

 g1
lpstr sh="Content-Type: application/x-www-form-urlencoded"
if(empty(headers)) headers=sh
else if(findrx(headers "(?i)^Content-Type *:" 8)<0) headers=_s.from(sh "[]" headers)

__HInternet hi=HttpOpenRequest(m_hi "POST" action 0 0 0 INTERNET_FLAG_RELOAD|inetflags 0); if(!hi) ret Error
if(!HttpSendRequest(hi headers -1 data len(data))) ret Error

if(&responseheaders and !GetResponseHeaders(hi responseheaders)) ret Error

if &responsepage
	if(m_hdlg) _s="(downloading)"; _s.setwintext(id(3 m_hdlg))
	ret Read(hi responsepage flags&16)
ret 1
