int create=0
if(create) del- "$my qm$\test\test.db3"; err
Sqlite x.Open("$my qm$\test\test.db3")
if create
	x.Exec("PRAGMA journal_mode=WAL;CREATE TABLE test(a INT, b INT, UNIQUE(a));BEGIN")
	int i
	for i 0 1000
		x.Exec(F"INSERT INTO test VALUES({i}, {i*10})")
	x.Exec("END")

SqliteStatement p.Prepare(x "SELECT b FROM test WHERE a=?1 LIMIT 1")
PF
for i 0 1000
	p.BindInt(1 i)
	if(p.FetchRow) p.GetInt(0); p.Reset; else out "not found"
PN;PO
