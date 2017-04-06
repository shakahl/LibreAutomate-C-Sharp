 Use functions of Database class to get/insert/delete/etc data from databases.
 Internally these functions use ADO (ActiveX Data Objects). <google "site:microsoft.com ActiveX Data Objects (ADO)">ADO reference</google>. MS Access Help also includes ADO reference.
 
 To connect to a database, use function Open.
 You must know the connection string for that database type. To easily create connection string for Access, Excel and Text (CSV), use functions CsAccess, CsExcel and CsText.
 Connection strings for other databases: <link>http://www.connectionstrings.com</link>, <link>http://www.codeproject.com/database/connectionstrings.asp</link>, <link>http://www.carlprothman.net/Default.aspx?tabid=81</link>.

 To make database queries, use SQL (structured query language). You can find SQL help on the Internet. MS Access Help also includes SQL reference.
 Use Query for queries that do not return data (eg INSERT). Use QueryArr or QueryRs for queries that return data (eg SELECT). QueryArr is simpler to use because immediately stores the data into an array. QueryRs retrieves Recordset object (read QueryRs help). RsX functions can be used to retrieve data from a Recordset object.

 All these functions, except CsX functions, may generate errors (incorrect connection string, incorrect SQL syntax, something does not exist, etc).

 You can use <help "Sqlite help">Sqlite</help> instead.

 QM 2.3.3. QM declares ADO type library version 2.5. Previously 2.0.

 EXAMPLES

 Retrieves Table1 from MS Access database My Documents\db1.mdb
Database db.Open(db.CsAccess("$documents$\db1.mdb"))
ARRAY(str) a; int r c
db.QueryArr("SELECT * FROM Table1" a)
for r 0 a.len(2)
	out "-- Record %i --" r+1
	for c 0 a.len(1)
		out a[c r]


 Opens database for exclusive access, creates table TestTable and adds one record
Database db7
str connString=db7.CsAccess("$documents$\db1.mdb")
db7.Open(connString "" "" 12)
db7.Query("DROP TABLE TestTable"); err ;;drop if exists
db7.Query("CREATE TABLE TestTable(FirstName CHAR, LastName CHAR, DateBirth DATETIME)")
str s1("Jennifer") s2("Aniston") s3("2/11/1969")
s1.SqlEscape; s2.SqlEscape ;;make the strings safe to pass to database
str sql.format("INSERT INTO TestTable VALUES('%s','%s','%s')" s1 s2 s3)
db7.Query(sql)


 Excel. As table name, use [worksheet name$]. First row is used for headers.
Database db2.Open(db2.CsExcel("$documents$\book1.xls"))
ARRAY(str) a2; int c2
db2.QueryArr("SELECT * FROM [Sheet1$] WHERE Country='USA'" a2 1)
for c2 0 a2.len
	out a2[c2]


 Text (CSV). As table name, use filename. First row is used for headers.
Database dbt.Open(dbt.CsText("$documents$"))
ARRAY(str) at
dbt.QueryArr("SELECT * FROM Table1.txt" at)
 ...


 dBase
str path="$documents$"; path.expandpath
str cs.format("Driver={Microsoft dBase Driver (*.dbf)};Dbq=%s;" path)
Database db6.Open(cs)
 ...


 MySQL on local computer (using MySQL ODBC connector driver version 5.1, http://dev.mysql.com/downloads/)
Database db5.Open("DRIVER={MySQL ODBC 5.1 Driver};DATABASE=menagerie;USER=root;PASSWORD=xxxx;")
ARRAY(str) am
db5.QueryArr("SELECT * FROM pet" am)
 ...


 MySQL on web server
Database db8.Open("Driver={MySQL ODBC 5.1 Driver};Server=db1.database.com;Port=3306;Option=131072;Database=mydb;User=myUsername;Password=myPassword")
 ...


 Using Recordset and RsX functions
Database db4.Open(db4.CsAccess("$documents$\db1.mdb"))
ADO.Recordset rs4; ARRAY(str) a4
db4.QueryRs("SELECT * FROM Table1" rs4)
db4.RsGetAll(rs4 a4)
 ...


 Using Recordset directly
Database db3.Open(db3.CsAccess("$documents$\db1.mdb"))
ADO.Recordset rs; ADO.Field f; int r3; str v
db3.QueryRs("SELECT * FROM Table1" rs)
for r3 0 rs.RecordCount
	out "-- Record %i --" r3+1
	foreach f rs.Fields
		v=f.Value
		err v.all ;;error if empty; clear v
		out v
	rs.MoveNext


 Sqlite cannot be used with Database class, unless you have ODBC driver for it.
 Instead use Sqlite class.
