out
str h="$qm$\sqlite\sqlite3.h"
str t="$qm$\sqlite.txt"
lpstr incl=
 $program files$\Microsoft Visual Studio\VC98\Include
ConvertCtoQM h t incl "" 0 "" "" "$qm$\winapiv_pch.txt" "$qm$\sqlite3" "" "sqlite3_int64 %[]sqlite_int64 %[]sqlite3_uint64 %[]sqlite_uint64 %"

 note: will be several errors on structs containing functions. Ignore it.

out "<>Tasks:  <open>Check ref</open>."
