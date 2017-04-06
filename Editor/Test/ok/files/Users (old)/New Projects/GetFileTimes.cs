 /
function $sFile [DATE&tMod] [DATE&tCreat] [DATE&tAccess] [utc]

 Gets file times.
 Use 0 for times that are not needed.
 If utc is nonzero, times are interpreted as UTC times. Default - local times.

 EXAMPLE
 DATE d
 GetFileTimes "$desktop$\test.txt" d


#error "not finished"

str sf.expandpath(sFile)
__HFile hfile.Create(sf OPEN_EXISTING FILE_READ_ATTRIBUTES); err end _error

FILETIME ft ftm ftc fta; FILETIME* pftm pftc pfta
if(tMod) pftm=&ftm; tMod.tofiletime(ft); if(utc) ftm=ft; else FileTimeToLocalFileTime &ft &ftm
if(tCreat) pftc=&ftc; tCreat.tofiletime(ft); if(utc) ftc=ft; else FileTimeToLocalFileTime &ft &ftc
if(tAccess) pfta=&fta; tAccess.tofiletime(ft); if(utc) fta=ft; else FileTimeToLocalFileTime &ft &fta

if(!GetFileTime(hfile pftc pfta pftm)) end ES_FAILED
