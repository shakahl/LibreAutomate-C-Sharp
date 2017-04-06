 /Macro420
function# $action $data [str&responsepage] [$headers]

 Cannot post raw headers because HttpSendRequest always inserts some headers.


__HInternet hi=HttpOpenRequest(m_hi "POST" action 0 0 0 INTERNET_FLAG_RELOAD 0); if(!hi) ret Error

str hd.all(1000 2)
if(!HttpQueryInfo(hi HTTP_QUERY_RAW_HEADERS_CRLF hd.lpstr &hd.len 0)) ret Error
out hd

if(!HttpSendRequest(hi headers -1 data len(data))) ret Error

if(&responsepage) ret Read(hi responsepage)
ret 1
