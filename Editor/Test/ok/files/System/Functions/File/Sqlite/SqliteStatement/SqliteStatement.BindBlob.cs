function sqlParam !*data nBytes

 Sets value for a SQL parameter.
 See <help>SqliteStatement.BindText</help>.

 Errors: sqlite errors


if(sqlParam&0xffff0000) sqlParam=__sqlite.sqlite3_bind_parameter_index(p +sqlParam)
if(__sqlite.sqlite3_bind_blob(p sqlParam data nBytes __sqlite.SQLITE_TRANSIENT)) E
