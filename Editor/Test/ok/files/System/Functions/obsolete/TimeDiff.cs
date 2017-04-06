 /
function% SYSTEMTIME*time1 SYSTEMTIME*time2

 Obsolete. Use <help>DATE</help> variables.


type ___FTLONG FILETIME'ft [0]%l
___FTLONG ft1 ft2
if(!SystemTimeToFileTime(time1 &ft1.ft) or !SystemTimeToFileTime(time2 &ft2.ft)) end ERR_BADARG

ret (ft1.l/10000000)-(ft2.l/10000000)
