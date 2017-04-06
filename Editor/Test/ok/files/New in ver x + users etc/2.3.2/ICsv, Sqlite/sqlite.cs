out
str dbfile="$desktop$\test.db3"
str sql

 Creates or opens database with table 'table1', columns 'A' and 'B'. Writes 1 row.

Sqlite db1.Open(dbfile)
db1.Exec("BEGIN TRANSACTION") ;;don't save until END TRANSACTION. Makes faster whe calling Exec multiple times.

db1.Exec("DROP TABLE table1")
db1.Exec("CREATE TABLE table1 (A TEXT,B INTEGER,C REAL)")

db1.Exec("INSERT INTO table1 VALUES ('text',10,20.0)")
db1.Exec("INSERT INTO table1 VALUES ('text',10.5,'20.5')")
db1.Exec("INSERT INTO table1 VALUES ('text',10.0,20.5)")
db1.Exec("INSERT INTO table1 VALUES (5.0,'txt10.0','20.0')")

db1.Exec("END TRANSACTION") ;;save now

 Opens database and gets all cells in table 'table1'.

Sqlite db3.Open(dbfile)
ARRAY(str) ar; int r
db3.Exec("SELECT * FROM table1" ar)
for r 0 ar.len(2) ;;for each row
	out "%-20s %-20s %-20s" ar[0 r] ar[1 r] ar[2 r]
