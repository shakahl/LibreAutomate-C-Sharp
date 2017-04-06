function$ column [int&nBytes]

 Returns value of a column of current row of SELECT results.

 column - 0-based column index in results. Also can be +"name", however it is slower.
 nBytes - if used, receives text length.

 REMARKS
 This and other GetX functions always get data in the requested format. They convert if needed.
 The return value is temporary. It may become invalid after calling a sqlite function.
 This and other GetX functions return 0 if failed.

 Errors: <SqliteStatement.Col>


if(column&0xffff0000) column=Col(+column)
lpstr s=__sqlite.sqlite3_column_text(p column)
if(&nBytes) nBytes=__sqlite.sqlite3_column_bytes(p column)
ret s
