function^ column

 Returns value of a column of current row of SELECT results.
 See <help>SqliteStatement.GetText</help>.

 Errors: <SqliteStatement.Col>


if(column&0xffff0000) column=Col(+column)
ret __sqlite.sqlite3_column_double(p column)
