function# timeout_s $action $data [str&responsepage] [$headers] [inetflags] [str&responseheaders] [&unused]

 Posts web form data. Returns 1 on success, 0 on failure. Error on timeout.
 This function cannot post files. To post files use PostFormData.

 timeout_s - timeout in seconds.
 data - urlencoded string, eg "name=John+Smith&email=j.smith@xxx.com".
 action, responsepage, headers, inetflags, responseheaders - the same as with PostFormData.
 unused - used internally.

 See also: <Http.PostFormData>.


if(!&unused)
	if(!m_hi) end ERR_INIT
	__HInternet _hi; int th _ret
	th=mac("__Http_PostWithTimeout" "" &this action data &responsepage headers inetflags &responseheaders &_hi &_ret)
	wait timeout_s H th
	err
		InternetCloseHandle _hi; _hi.handle=0 ;;ends thread
		wait 0 H th ;;because the thread uses this. ~0.5ms
		end _error
	ret _ret

lpstr sh="Content-Type: application/x-www-form-urlencoded"
if(empty(headers)) headers=sh
else if(findrx(headers "(?i)^Content-Type *:" 8)<0) headers=_s.from(sh "[]" headers)

unused=HttpOpenRequest(m_hi "POST" action 0 0 0 INTERNET_FLAG_RELOAD|inetflags 0); if(!unused) ret Error
int option(5) optionlen(4); if(!InternetSetOption(m_hi INTERNET_OPTION_MAX_CONNS_PER_SERVER &option &optionlen)) ret Error

if(!HttpSendRequest(unused headers -1 data len(data))) ret Error

if(&responseheaders and !GetResponseHeaders(unused responseheaders)) ret Error

if(&responsepage) ret Read(unused responsepage)
ret 1


 function# timeout_s $action $data [str&responsepage] [$headers] [inetflags] [str&responseheaders] [&unused]
 
  Posts web form data. Returns 1 on success, 0 on failure. Error on timeout.
  This function cannot post files. To post files use PostFormData.
 
  timeout_s - timeout in seconds.
  data - urlencoded string, eg "name=John+Smith&email=j.smith@xxx.com".
  action, responsepage, headers, inetflags, responseheaders - the same as with PostFormData.
  unused - used internally.
 
  See also: <Http.PostFormData>.
 
 
 if(!&unused)
	 if(!m_hi) end ERR_INIT
	 __HInternet _hi; int th _ret
	 th=mac("__Http_PostWithTimeout" "" &this action data &responsepage headers inetflags &responseheaders &_hi &_ret)
	 wait timeout_s H th
	 err
		 InternetCloseHandle _hi; _hi.handle=0 ;;ends thread
		 wait 0 H th ;;because the thread uses this. ~0.5ms
		 end _error
	 ret _ret
 
 lpstr sh="Content-Type: application/x-www-form-urlencoded"
 if(empty(headers)) headers=sh
 else if(findrx(headers "(?i)^Content-Type *:" 8)<0) headers=_s.from(sh "[]" headers)
 
 unused=HttpOpenRequest(m_hi "POST" action 0 0 0 INTERNET_FLAG_RELOAD|inetflags 0); if(!unused) ret Error
 int option(5) optionlen(4); if(!InternetSetOption(m_hi INTERNET_OPTION_MAX_CONNS_PER_SERVER &option &optionlen)) ret Error
 if(!HttpSendRequest(unused headers -1 data len(data))) ret Error
 
 if(&responseheaders and !GetResponseHeaders(unused responseheaders)) ret Error
 
 if(&responsepage) ret Read(unused responsepage)
 ret 1
