function Sqlite&x $sql

 Prepares (compiles) SQL statement. Does not execute it.
 To execute, call Exec or FetchRow, one or more times.

 Errors: sqlite errors


Delete
conn=x.conn
if(__sqlite.sqlite3_prepare_v2(conn sql -1 &p 0)) E
