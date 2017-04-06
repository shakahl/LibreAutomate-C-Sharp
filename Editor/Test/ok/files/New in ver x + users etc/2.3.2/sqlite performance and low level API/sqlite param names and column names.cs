 Makes easier to use sqlite prepared statements.
 A variable of this type represents a prepared statement. When used with SELECT, it also represents results.
 Used together with <tip "#Sqlite help">Sqlite</tip> class.

 Prepared statements are used instead of Exec to:
 1. Make faster when the same or similar statement is executed multiple times. Can make multiple INSERT almost 2 times faster. However does not make SELECT faster.
 2. Use values of various types without converting to/from text.

 NOTES

 These functions throw error if failed: Prepare, Exec, FetchRow, all BindX functions. The GetX functions on error return 0.
 SqliteStatement variables must be declared after the Sqlite variable.
 Don't use the same variable in several threads simultaneously. If need, use lock.
 You can use the member variable p with sqlite API (dll) functions as sqlite3_stmt*.

 <link "http://www.sqlite.org/cintro.html">More info</link>.

 EXAMPLE

 Create database for testing, and 1 table with 5 columns of various types.

Sqlite x.Open("$desktop$\test573.db3")
x.Exec("BEGIN TRANSACTION")
x.Exec("DROP TABLE table1"); err
x.Exec("CREATE TABLE table1 (a INTEGER PRIMARY KEY, b REAL, c INTEGER, d TEXT, e BLOB)")

 Insert multiple rows.

SqliteStatement pi.Prepare(x "INSERT INTO table1 VALUES ($a, $b, $c, $d, $e)")
 Prepare() converts SQL text into a prepared statement object. It does not execute the statement.
 The ?1, ?2, $k etc are "SQL parameters". Those with $ are named. You will set the actual values using the BindX functions.

int i; double d; long k; str s
for i 0 10
	 get row data from somewhere
	d=RandomNumber
	s.RandomString(10 20)
	k=d*1000000000000000000
	 set values for the ?1, ?2 etc. If not set, they are NULL.
	pi.BindInt(+"$a" i)
	pi.BindDouble(+"$b" d)
	pi.BindLong(+"$c" k)
	pi.BindText(+"$d" s)
	pi.BindBlob(+"$e" s s.len)
	 execute
	pi.Exec
	 reset to the initial state
	pi.Reset

x.Exec("END TRANSACTION") ;;save

 Select.

 SqliteStatement ps.Prepare(x "SELECT * FROM table1")
SqliteStatement ps.Prepare(x "SELECT a,table1.b,C,d,e AS f FROM table1")
i=0
rep
	 get next row
	if(!ps.FetchRow) break
	
	if i=0
		out "-- columns --"
		int j; for(j 0 ps.GetColumnCount) out ps.GetColumnName(j)
		out "-- rows --"
	
	out "%i  %.10g  %I64i  ''%s''" ps.GetInt(+"a") ps.GetDouble(+"b") ps.GetLong(+"c") ps.GetText(+"d")
	byte* b=ps.GetBlob(+"f" _i); outb b _i
	i+1
	
	 The GetX functions are used to get values and other info of current row of SELECT results.
	 The 0, 1 etc are column indexes.
