out
str h="$qm$\misc\unqlite.h"
str t="$qm$\unqlite.txt"
lpstr incl=
 $program files$\Microsoft Visual Studio\VC98\Include
ConvertCtoQM h t incl "" 0 "" "" "$qm$\winapiv_pch.txt" "$qm$\unqlite" ""
 "sqlite3_int64 %[]sqlite_int64 %[]sqlite3_uint64 %[]sqlite_uint64 %"

 note: will be several errors on structs containing functions. Ignore it.

 out "<>Tasks:  <open>Check ref</open>."
