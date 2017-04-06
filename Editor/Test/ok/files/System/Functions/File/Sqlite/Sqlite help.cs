 SQLite is a free, lightweight database engine.
 <link>https://www.sqlite.org</link>.

 This class has functions to work with SQLite databases.
 It is simple to use, but you must know SQLite's SQL language. It is similar to other SQL dialects. Documented in the SQLite web site.
 Uses SQLite version 3.8.2 (changed from 3.7.17 in QM 2.4.0; from 3.6.23 in QM 2.3.5). The dll is included with QM.

 How to use: Call Open to open or create database, then call Exec (or ExecF) to execute SQL statements (set/get data, create tables, etc). Calling Close is optional.
 Distributing with exe: Add sqlite3.dll to your zip/setup file. Extract to the exe folder.

 NOTES
 Text in databases is in Unicode UTF-8 format. It is native text format of QM 2.3.0 and later running in Unicode mode (Options -> Unicode). If your QM is running in ANSI mode, use only ASCII text (character codes 0 to 127), because it is the same in UTF-8.
 The class does not work with databases that use UTF-16 encoding.
 Need <help>lock</help> if multiple threads use the same variable or write to the same database.
 Instead of Exec you can use class <help "SqliteStatement help">SqliteStatement</help>.
 Also you can use SQLite API, like __sqlite.func_name(arguments).

 <open "sample_Grid_Sqlite">Example with grid control</open>

 Added in: QM 2.3.2.

 EXAMPLES

str dbfile="$desktop$\test57.db3"
 ___________________

 Creates or opens database with table 'table1', columns 'id' and 'name'. Writes 1 row.

Sqlite db1.Open(dbfile)
db1.Exec("BEGIN TRANSACTION")
db1.Exec("CREATE TABLE IF NOT EXISTS table1(id INTEGER PRIMARY KEY, name TEXT)")
str a.RandomString(4 8 "A-Z")
a.SqlEscape ;;replace ' characters with ''
db1.Exec(F"INSERT INTO table1(name) VALUES('{a}')")
db1.Exec("END TRANSACTION")
db1.Close ;;optional
 Transaction makes faster when executing multiple write statements (INSERT, DELETE, UPDATE etc). Also makes all the changes atomic (all or none), regardless of errors, power loss etc.
 ___________________

 Changes data in 1 row in table 'table1'.

Sqlite db2.Open(dbfile)
db2.Exec("UPDATE table1 SET name='new name' WHERE id=1")
db1.Close ;;optional
 ___________________

 Gets all cells in table 'table1'.

Sqlite db3.Open(dbfile)
ARRAY(str) ar; int r
db3.Exec("SELECT * FROM table1" ar)
for r 0 ar.len ;;for each row
	out "%-10s %s" ar[0 r] ar[1 r]
