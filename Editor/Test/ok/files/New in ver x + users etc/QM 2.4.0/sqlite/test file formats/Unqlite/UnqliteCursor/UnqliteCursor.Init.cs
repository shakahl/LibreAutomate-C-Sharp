function Unqlite&db

 Creates unqlite database cursor and initializes this variable.
 Error if fails.
 Calls unqlite_kv_cursor_init.


opt noerrorshere 1; opt nowarningshere 1
Delete
if(__unqlite.unqlite_kv_cursor_init(db &c)) end F"{ERR_FAILED}" 2
m_db=&db
