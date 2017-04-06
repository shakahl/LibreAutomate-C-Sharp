 str sf="\\gintaras\Q\app\system.qml"
str sf="$my qm$\test\ok.qml"
PF
Sqlite x.Open(sf 0 4|8)
 Sqlite x.Open(sf 1 4)
x.Exec("SELECT * FROM files")
PN;PO
mes 1
