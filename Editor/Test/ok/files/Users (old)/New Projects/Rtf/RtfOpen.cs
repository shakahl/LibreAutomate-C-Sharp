 /
function# hre $filename

EDITSTREAM es
es.pfnCallback=&RtfOpenProc
es.dwCookie=CreateFile(filename GENERIC_READ 0 0 OPEN_EXISTING FILE_ATTRIBUTE_NORMAL 0)
if(es.dwCookie=-1) ret
SendMessage(hre EM_STREAMIN SF_RTF &es)
CloseHandle(es.dwCookie)
ret !es.dwError
