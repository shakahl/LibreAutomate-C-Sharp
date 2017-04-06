 /
function $sFile [DATE'tMod] [DATE'tCreat] [DATE'tAccess] [utc] ;;obsolete, use FileSetAttributes

 Sets file times.

 sFile - file.
 tMod - time last modified. If 0, does not change.
 tCreat - time created. If 0, does not change.
 tAccess - time last accessed. If 0, does not change.
 utc - must be 1 if times are UTC times. Default: times are local times.


str sf.expandpath(sFile)
__HFile hfile.Create(sf OPEN_EXISTING FILE_WRITE_ATTRIBUTES); err end _error

FILETIME ft ftm ftc fta; FILETIME* pftm pftc pfta
if(tMod) pftm=&ftm; tMod.tofiletime(ft); if(utc) ftm=ft; else LocalFileTimeToFileTime &ft &ftm
if(tCreat) pftc=&ftc; tCreat.tofiletime(ft); if(utc) ftc=ft; else LocalFileTimeToFileTime &ft &ftc
if(tAccess) pfta=&fta; tAccess.tofiletime(ft); if(utc) fta=ft; else LocalFileTimeToFileTime &ft &fta

if(!SetFileTime(hfile pftc pfta pftm)) end ERR_FAILED
