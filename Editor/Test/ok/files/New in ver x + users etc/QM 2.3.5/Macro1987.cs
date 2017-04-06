__HFile f.Create("$appdata$\GinDi\Quick Macros" OPEN_EXISTING GENERIC_READ|FILE_WRITE_ATTRIBUTES FILE_SHARE_READ|FILE_SHARE_WRITE FILE_FLAG_BACKUP_SEMANTICS)
 out f
FILETIME t
GetFileTime(f &t 0 0)
out "%i %i" t.dwHighDateTime t.dwLowDateTime
t.dwLowDateTime=0
SetFileTime(f &t 0 0)

#ret
30271498 1231394811
2013 ‎m. ‎sausio ‎1 ‎d., ‏‎12:25:14

30271498 0
2013 ‎m. ‎sausio ‎1 ‎d., ‏‎12:23:11

30271498 -1
2013 ‎m. ‎sausio ‎1 ‎d., ‏‎12:30:20
