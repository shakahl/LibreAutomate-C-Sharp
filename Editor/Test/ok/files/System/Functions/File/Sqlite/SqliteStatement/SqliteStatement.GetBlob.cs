function!* column [int&nBytes]

 Returns value of a column of current row of SELECT results.
 See <help>SqliteStatement.GetText</help>.

 nBytes - if used, receives data length.

 REMARKS
 The return value is temporary. It may become invalid after calling a sqlite function.

 Errors: <SqliteStatement.Col>


if(column&0xffff0000) column=Col(+column)
byte* b=__sqlite.sqlite3_column_blob(p column)
if(&nBytes) nBytes=__sqlite.sqlite3_column_bytes(p column)
ret b
