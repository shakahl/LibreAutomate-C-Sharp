
 Executes prepared statement.
 Don't use this function for SELECT. Instead use FetchRow.
 Error if failed.

 Errors: sqlite errors


if(__sqlite.sqlite3_step(p)!=__sqlite.SQLITE_DONE) E
