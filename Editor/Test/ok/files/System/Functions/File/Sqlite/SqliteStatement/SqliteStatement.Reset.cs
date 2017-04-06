
 Call this function after Exec if you call it more than 1 time.
 This function does not reset bindings. Call ResetBindings if need.
 Don't call this function after FetchRow when fetching rows of the same query. Call only between queries, if you do it more than 1 time.


__sqlite.sqlite3_reset(p)
