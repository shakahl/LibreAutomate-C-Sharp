Sqlite& x=_qmfile.SqliteBegin
ARRAY(str) a
PF
x.Exec("PRAGMA quick_check" a)
PN
x.Exec("PRAGMA integrity_check" a)
PN;PO
_qmfile.SqliteEnd
