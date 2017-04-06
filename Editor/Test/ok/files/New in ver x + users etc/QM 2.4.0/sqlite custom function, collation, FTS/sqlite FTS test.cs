out
Sqlite x.Open("")

if(__sqlite.sqlite3_create_collation(x "NOCASE" __sqlite.SQLITE_UTF8 0 &_SqliteCollationNC)) out "error"
if(__sqlite.sqlite3_create_function(x "lower" 1 __sqlite.SQLITE_UTF8 0 &_SqliteLower 0 0)) out "error"
if(__sqlite.sqlite3_create_function(x "upper" 1 __sqlite.SQLITE_UTF8 0 &_SqliteUpper 0 0)) out "error"

x.Exec("CREATE VIRTUAL TABLE test USING fts4(a, tokenize=porter);INSERT INTO test(a) VALUES('ansi'),('ANSI'),('ĄČĘ'),('ąčę')")
ARRAY(str) a; int i
 x.Exec("SELECT a FROM test WHERE a MATCH 'ansi'" a)
x.Exec("SELECT a FROM test WHERE a MATCH 'ąčę'" a)
out "-----"
for(i 0 a.len) out a[0 i]
