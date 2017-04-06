 /
function $textFile $dbFile $tableName $columnName [$columnType]

 Converts text file to Sqlite database file.

 textFile - text file. Data in it must be stored as a simple multiline list.
 dbFile - Sqlite database file to create. Can have .db3 or other extension.
 tableName - name of table to create in the database file.
 columnName - name of the data column in the table.
 columnType - type of the data column. If used, can be TEXT, INTEGER or REAL.

 REMARKS
 In the database file this function creates a table that has 2 columns:
   id - numbers 1,2,3... . Its type is INTEGER PRIMARY KEY.
   columnName - lines from the text file. Its type is columnType.


str s ss.getfile(textFile)
if(dir(dbFile)) del- dbFile

Sqlite x.Open(dbFile)
x.Exec(F"BEGIN TRANSACTION;CREATE TABLE {tableName}(id INTEGER PRIMARY KEY, {columnName} {columnType});")
foreach s ss
	x.Exec(F"INSERT INTO {tableName}({columnName}) VALUES('{s.SqlEscape}')")
x.Exec("END TRANSACTION")

err+ end _error
