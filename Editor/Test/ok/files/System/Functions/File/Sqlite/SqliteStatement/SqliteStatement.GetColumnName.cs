function$ column

 Returns name of a column in SELECT results.

 column - 0-based column index in results.

 REMARKS
 The return value is temporary. It may become invalid after calling a sqlite function.
 If in SELECT used "column AS alias", returns the alias.


ret __sqlite.sqlite3_column_name(p column)
