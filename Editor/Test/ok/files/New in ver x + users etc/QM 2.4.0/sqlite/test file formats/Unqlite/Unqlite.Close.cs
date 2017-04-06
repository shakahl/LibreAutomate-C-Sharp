
 Closes database.
 Optional, called implicitly when the variable dies.
 Warning if fails.


if(!m_db) ret
if(__unqlite.unqlite_close(m_db)) end "failed to close database" 8|2
m_db=0
