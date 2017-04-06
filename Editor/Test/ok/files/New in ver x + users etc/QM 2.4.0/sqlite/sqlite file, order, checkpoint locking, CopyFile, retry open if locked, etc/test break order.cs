 out
str sf; int rowid
if 1
	sf="$my qm$\test\ok.qml"
	rowid=5
else
	sf="$my qm$\test\sqlite export.qml"
	rowid=4

Sqlite x.Open(sf)
x.Exec(F"UPDATE items SET ord=x'000001' WHERE id={rowid}")
 x.Exec(F"UPDATE items SET ord=x'000901' WHERE id={rowid}")
