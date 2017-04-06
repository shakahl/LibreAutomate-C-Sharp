 /
function# hre $filename

EDITSTREAM es
es.pfnCallback=&RtfSaveProc
es.dwCookie=CreateFile(filename GENERIC_WRITE 0 0 CREATE_ALWAYS FILE_ATTRIBUTE_NORMAL 0)
if(es.dwCookie=-1) ret
SendMessage(hre EM_STREAMOUT SF_RTF &es)
CloseHandle(es.dwCookie)
ret !es.dwError
