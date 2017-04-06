function $sql [cbFunc] [!*param]

 Executes one or more SQL statements.
 Same as Exec, but for SELECT queries this function calls a user-defined function instead of placing results into array. If cbFunc is omitted or 0, this function is same as Exec.
 SQL reference: <link>http://www.sqlite.org/lang.html</link>.
 Error if fails.

 sql - SQL statement. Should be SELECT. Can be several statements delimited with semicolon.
 cbFunc - address of <help "Callback_Sqlite_ExecF">callback function</help> that will be called for each row of SELECT results.
   A template is available in menu -> File -> New -> Templates.
 param - something to pass to the callback function.

 REMARKS
 This function uses <link "http://www.sqlite.org/c3ref/exec.html">sqlite3_exec</link>.


if(!m_db) end ERR_INIT ;;or exception
lpstr es
int r=__sqlite.sqlite3_exec(m_db sql cbFunc param &es)
if r
	_s=es; __sqlite.sqlite3_free es
	end F"{ERR_FAILED}. Sqlite error {r}. {_s}."
