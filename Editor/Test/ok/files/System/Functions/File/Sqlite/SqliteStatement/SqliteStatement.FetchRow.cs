function!

 Executes SELECT prepared statement and fetches next row of results.
 Returns 1 if fetched next row, or 0 if there are no more rows.
 If returned 1, you can call GetX functions to get values. Then call this function again to get next row, and so on, until it returns 0.
 Error if failed.

 Errors: sqlite errors


sel __sqlite.sqlite3_step(p)
	case __sqlite.SQLITE_ROW ret 1
	case __sqlite.SQLITE_DONE ret
E
