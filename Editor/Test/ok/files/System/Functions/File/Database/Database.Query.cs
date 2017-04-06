function# $sql

 Executes SQL query (usually other than SELECT).
 Returns the number of affected records.
 Use this function for action queries, such as DELETE, UPDATE, etc.
 The return value will be -1 for result-returning queries, such as SELECT (this function does not return results; use QueryArr instead).


VARIANT ra
conn.Execute(sql ra ADO.adExecuteNoRecords)
ret ra

err+ end _error
