function!* ;;Gets connection handle to be used with sqlite dll functions.

 Returns sqlite database connection handle that has been created by Open.
 In sqlite documentation its type is sqlite3*. In QM use byte* instead.
 It can be used with sqlite API (dll) functions. You'll find the documentation in <link>http://www.sqlite.org/cintro.html</link>.

 EXAMPLE
 Sqlite db.Open(dbfile)
 int rc=__sqlite.sqlite3_prepare(db.conn ...)
 ...


ret m_db
