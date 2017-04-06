function# $action $data [str&responsepage] [$headers] [inetflags] [str&responseheaders]

 Posts web form data. Returns 1 on success, 0 on failure.
 This function cannot post files. To post files use PostFormData.

 data - urlencoded string, eg "name=John+Smith&email=j.smith@xxx.com".
 action, responsepage, headers, inetflags, responseheaders - the same as with PostFormData.

 See also: <Http.PostFormData>.


lpstr sh="Content-Type: application/x-www-form-urlencoded"
if(empty(headers)) headers=sh
else if(findrx(headers "(?i)^Content-Type *:" 8)<0) headers=_s.from(sh "[]" headers)

__HInternet hi=HttpOpenRequest(m_hi "POST" action 0 0 0 INTERNET_FLAG_RELOAD|inetflags 0); if(!hi) ret Error

int option(10) optionlen(4); if(!InternetSetOption(hi INTERNET_OPTION_MAX_CONNS_PER_SERVER &option &optionlen)) ret Error

if(!HttpSendRequest(hi headers -1 data len(data))) ret Error

if(&responseheaders and !GetResponseHeaders(hi responseheaders)) ret Error

if(&responsepage) ret Read(hi responsepage)
ret 1
