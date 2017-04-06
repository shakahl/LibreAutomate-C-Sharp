function# $sql

 Executes single statement that returns single int value in single row, eg "SELECT count(*) FROM items" or "PRAGMA get_something".


opt noerrorshere 1
SqliteStatement p
p.Prepare(this sql)
if(p.FetchRow) ret __sqlite.sqlite3_column_int(p.p 0)
 else out "no row"
