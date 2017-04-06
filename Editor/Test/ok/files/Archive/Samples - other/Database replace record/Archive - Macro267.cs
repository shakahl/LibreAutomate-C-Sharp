str n="Test"
str e="a@b.c"
str a=30

 connect
Database db7
str connString=db7.CsAccess("$personal$\db1.mdb")
db7.Open(connString)

  create test table
db7.Query("DROP TABLE TestTable"); err ;;drop if exists
db7.Query("CREATE TABLE TestTable(Name CHAR, Email CHAR, Age NUMBER)")

 delete record if exists
str sql
sql.format("DELETE FROM TestTable WHERE Name='%s'" n)
db7.Query(sql)
 insert record
sql.format("INSERT INTO TestTable VALUES('%s','%s',%s)" n e a)
db7.Query(sql)

 disconnects automatically
