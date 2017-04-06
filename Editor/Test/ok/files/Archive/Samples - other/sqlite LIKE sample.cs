out

str dbfile="$desktop$\test.db3"

 create database for testing
Sqlite db1.Open(dbfile)
db1.Exec("DROP TABLE table1"); err
db1.Exec("CREATE TABLE table1 (A,B); INSERT INTO table1 VALUES ('one','two'), ('three','four'), ('te','five')")

 search
Sqlite db2.Open(dbfile)
ARRAY(str) ar; int r
db2.Exec("SELECT * FROM table1 WHERE A LIKE 't%e'" ar) ;;with LIKE, % is used instead of * (0 or more characters), _ instead of ? (single character)
 db2.Exec("SELECT * FROM table1 WHERE A GLOB 't*e'" ar) ;;GLOB uses *? and is case-sensitive
for r 0 ar.len
	out F"{ar[0 r]} {ar[1 r]}"
