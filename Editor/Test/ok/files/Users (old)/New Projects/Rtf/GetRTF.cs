 /
function# hre str&s

s.all
EDITSTREAM es
es.pfnCallback=&GetRTF_Proc
es.dwCookie=&s
SendMessage(hre EM_STREAMOUT SF_RTF &es)
ret !es.dwError
