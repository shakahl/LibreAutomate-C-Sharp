function$ str&ss $sql

 Executes single statement that returns single text value in single row.


opt noerrorshere 1
SqliteStatement p
p.Prepare(this sql)
if(p.FetchRow) ss=__sqlite.sqlite3_column_text(p.p 0)
ret ss
