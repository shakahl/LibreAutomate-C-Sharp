str sf; rget sf "file debug" "Software\GinDi\QM2\settings"
Sqlite x.Open(sf 0)
 out x.ExecGetText(_s "PRAGMA journal_mode")
 out x.ExecGetInt("PRAGMA freelist_count")

ARRAY(str) a
PF
x.Exec("PRAGMA quick_check" a)
 x.Exec("PRAGMA integrity_check" a)
PN;PO
out a
