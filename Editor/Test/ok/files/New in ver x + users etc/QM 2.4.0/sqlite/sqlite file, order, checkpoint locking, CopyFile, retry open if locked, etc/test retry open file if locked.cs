 str sf="$my qm$\test\ok.qml"
str sf="$qm$\system.qml"
str sf2

PF
int readOnly(0) locked retry
 g1
locked=0
Sqlite x.Open(sf readOnly)
if(!readOnly) x.Exec("PRAGMA exclusive=ON")
x.Exec("SELECT ver FROM files WHERE id=0"); err locked=1
if locked and retry<2
	retry=1+readOnly
	out "locked"
	if(!readOnly) readOnly=1
	else sf2.from(sf "-copy"); FileCopy sf sf2; sf=sf2
	goto g1

PN
PO
x.Exec("SELECT ver FROM files WHERE id=0")
out __sqlite.sqlite3_db_readonly(x 0)
x.Exec("DELETE FROM texts WHERE id=(SELECT id FROM items WHERE name='init')")
out __sqlite.sqlite3_changes(x)
 mes 1
