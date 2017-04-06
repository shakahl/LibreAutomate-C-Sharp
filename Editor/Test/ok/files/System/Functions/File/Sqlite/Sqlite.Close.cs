
 Closes the database connection.

 REMARKS
 Pending transactions are rolled back. 
 The function is implicitly called when destroying the variable. Also when you call Open again.
 Fails if there are prepared but not finalized statements (it can happen if you use sqlite dll functions).
 If fails, does not throw error, but just displays error in QM output. And you'll have a memory leak.


if(m_db)
	int r=__sqlite.sqlite3_close(m_db)
	if(r) out "Failed to close database. %s." __sqlite.sqlite3_errmsg(m_db)
	else m_db=0
