out
 del- "$my qm$\test\sett.db3"
Sqlite x.Open("$my qm$\test\sett.db3" 0 4)
 x.Exec("PRAGMA journal_mode=WAL")
 x.Exec("CREATE TABLE test(N TEXT, V TEXT)")
 x.Exec("INSERT INTO test VALUES('one', 'ooooooooooo')")
 x.Exec("INSERT INTO test VALUES('two', 'ttttttttt')")
 x.Exec("INSERT INTO test VALUES('three', 'rrrrrrrrrrr')")

str s1("ooooooooooo") s2("ttttttttt") s3("rrrrrrrrrrr")
int i c
for i 0 4
	c=i+'A'
	s1[0]=c; s2[0]=c; s3[0]=c
	PF
	rset s1 "one" "\test sqlite"
	rset s2 "two" "\test sqlite"
	rset s3 "three" "\test sqlite"
	PN
	rget _s "one" "\test sqlite";; out _s
	rget _s "two" "\test sqlite";; out _s
	rget _s "three" "\test sqlite";; out _s
	
	PN
	x.Exec(F"UPDATE test SET V='{s1}' WHERE N='one'");; out __sqlite.sqlite3_changes(x)
	x.Exec(F"UPDATE test SET V='{s2}' WHERE N='two'");; out __sqlite.sqlite3_changes(x)
	x.Exec(F"UPDATE test SET V='{s3}' WHERE N='three'");; out __sqlite.sqlite3_changes(x)
	PN
	ARRAY(str) a
	x.Exec("SELECT V FROM test WHERE N='one'" a);; out a
	x.Exec("SELECT V FROM test WHERE N='two'" a);; out a
	x.Exec("SELECT V FROM test WHERE N='three'" a);; out a
	PN
	PO
