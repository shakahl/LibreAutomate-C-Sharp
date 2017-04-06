 /exe 1
str dbfile="$desktop$\test.db3"
str sql

 Creates or opens database with table 'table1', columns 'A' and 'B'. Writes 1 row.

Sqlite db1.Open(dbfile)
db1.Exec("BEGIN TRANSACTION") ;;don't save until END TRANSACTION. Makes faster whe calling Exec multiple times.
db1.Exec("CREATE TABLE IF NOT EXISTS table1 (A,B)")
str a("one") b("two") ;;data in variables
a.SqlEscape; b.SqlEscape ;;if there are ' characters, they must be replaced to ''
sql.format("INSERT INTO table1 VALUES ('%s','%s')" a b)
db1.Exec(sql)
db1.Exec("END TRANSACTION") ;;save now

 This is a simpler example. Just writes 1 row into an existing table.
Sqlite db2.Open(dbfile)
db1.Exec("INSERT INTO table1 VALUES ('value1','value2')")

 Opens database and gets all cells in table 'table1'.

Sqlite db3.Open(dbfile)
ARRAY(str) ar; int r
db3.Exec("SELECT * FROM table1" ar)
for r 0 ar.len(2) ;;for each row
	out "%-20s %-20s" ar[0 r] ar[1 r]

 BEGIN PROJECT
 main_function  Macro1357
 exe_file  $my qm$\Macro1357.exe
 icon  <default>
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {4AF10398-14EC-4755-AF10-76C4DD02843A}
 END PROJECT
