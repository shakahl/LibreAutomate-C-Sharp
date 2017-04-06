function sqlParam $value

 Sets value for a SQL parameter (a SQL string part like ?1 or $name).
 Error if fails.

 sqlParam - 1-based SQL parameter index. The same as used in SQL.
   If it is a named parameter (eg $name), can be +"$name".
 value - value.

 Errors: sqlite errors


if(sqlParam&0xffff0000) sqlParam=__sqlite.sqlite3_bind_parameter_index(p +sqlParam)
if(__sqlite.sqlite3_bind_text(p sqlParam value -1 __sqlite.SQLITE_TRANSIENT)) E
 info: SQLITE_TRANSIENT makes safer and ~2% slower
