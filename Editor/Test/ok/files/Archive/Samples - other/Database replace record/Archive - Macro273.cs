 Database: My Documents\db1.mdb
 Table: TestTable
 Fields: Name (text), Email (text), Age (number)

 will insert or replace these values
str n="Test"
str e="a@b.c"
str a=100

 connect
Database db7
str connString=db7.CsAccess("$personal$\db1.mdb")
db7.Open(connString)

  create test table
db7.Query("DROP TABLE TestTable"); err ;;drop if exists
db7.Query("CREATE TABLE TestTable(Name CHAR, Email CHAR, Age NUMBER)")

 if record exists, update values
str sql.format("UPDATE TestTable SET Email='%s', Age=%s WHERE Name='%s'" e a n)
if(db7.Query(sql)=0) ;;returns the number of affected records, so it will be 0 if the record does not exist
	 insert new record
	sql.format("INSERT INTO TestTable (Name,Email,Age) VALUES('%s','%s',%s)" n e a)
	db7.Query(sql)

 disconnects automatically
