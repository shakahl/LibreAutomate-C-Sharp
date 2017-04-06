function $dbFile [openFlags] [flags2] [busyTimeout] ;;openFlags: 1 read-only, 2 no auto-create.   flags2: 2 PRAGMA synchronous=OFF, 4 PRAGMA synchronous=NORMAL, 0x8000 enable foreign keys

 Opens database connection on file.
 Creates the file if does not exist.
 Error if fails.

 dbFile - database file. Special strings:
   "" - a private, temporary on-disk database is created.
   ":memory:" - a private, temporary in-memory database is created.
 openFlags - __sqlite.SQLITE_OPEN_x flags:
   1 (SQLITE_OPEN_READONLY) - The database is opened in read-only mode. Error if the database does not already exist.
   2 (SQLITE_OPEN_READWRITE) - The database is opened for reading and writing if possible, or reading only if the file is write protected by the operating system. Error if the database does not already exist.
   6,0 (SQLITE_OPEN_READWRITE|SQLITE_OPEN_CREATE, default) - The database is opened for reading and writing, and is created it if it does not already exist. This is the behavior that is used when openFlags is omitted or 0.
   Also there are more flags. See sqlite documentation on the web, function sqlite3_open_v2.
 flags2:
   2 (QM 2.3.6.5) - execute "PRAGMA synchronous=OFF". Will write faster, but a power loss or OS crash may corrupt the database file. Use for example for temporary files.
   4 (QM 2.4.0) - execute "PRAGMA synchronous=NORMAL". Will write faster in WAL journal mode. Read more in SQLite help.
   8 (QM 2.4.0) - execute "PRAGMA locking_mode=EXCLUSIVE". In exclusive mode, the database cannot be used by other. Read more in SQLite help.
   0x8000 (QM 2.4.0) - enable foreign keys.
 busyTimeout (QM 2.4.0) - if not 0, calls sqlite3_busy_timeout with this value.

 REMARKS
 QM 2.3.6.5. Default mode now is synchronous, like previously with flags2 1. Flag 1 now is not used.
 Tip: when making multiple write operations (INSERT, UPDATE, DELETE etc), use transaction. It makes faster.


Close

str sf se
int isTemp=empty(dbFile) or !StrCompare(dbFile ":memory:" 0)
if(isTemp) sf=dbFile; else sf.expandpath(dbFile); if(!_unicode) sf.ConvertEncoding(0 CP_UTF8)
sel(openFlags&7) case 0 openFlags|6; case [1,2,6] case else end F"{ERR_BADARG} openFlags"

int retry
 g1
if __sqlite.sqlite3_open_v2(sf &m_db openFlags 0)
	if(m_db) se=__sqlite.sqlite3_errmsg(m_db); Close
	else se="unknown error"
	
	if !retry and openFlags&4 and !isTemp ;;auto-create folder
		_s.getpath(dbFile "")
		if(_s.len) retry=mkdir(_s); err
		if(retry) goto g1
	
	end F"{ERR_FAILED}. {se}"

if(busyTimeout) __sqlite.sqlite3_busy_timeout(m_db busyTimeout)
if(flags2&0x8000) if(__sqlite.sqlite3_db_config(m_db __sqlite.SQLITE_DBCONFIG_ENABLE_FKEY 1 0)) end ERR_FAILED
if !isTemp
	if(flags2&8) Exec("PRAGMA locking_mode=EXCLUSIVE")
	if(flags2&4) Exec("PRAGMA synchronous=NORMAL"); else if(flags2&2) Exec("PRAGMA synchronous=OFF")

err+
	Close
	end _error
