function $dbFile [flags] [flags2] ;;flags: 0 open or create, 1 open read-only, 2 open, 0x101 open read-only memory-mapped.  flags2: 1 no auto-save

 Opens database connection on file.
 By default creates the file if does not exist.
 Calls unqlite_open.
 Error if fails.

 dbFile - database file. Special strings:
  ":mem:" or "" - a private, temporary in-memory database is created.
 flags - __unqlite.UNQLITE_OPEN_ flags:
   0,4 (UNQLITE_OPEN_CREATE) - create file if does not exist. Open for read-write.
   1 (UNQLITE_OPEN_READONLY) - open for read. Error if file does not exist.
   2 (UNQLITE_OPEN_READWRITE) - open for read-write. Error if file does not exist.
   0x100|1 - open for read, and map in memory. Makes faster.
   And more. Read in unqlite API reference on the internet.
 flags2:
   Don't auto-save changes when closing. You can save with __unqlite.unqlite_commit(thisVar). See unqlite_config/UNQLITE_CONFIG_DISABLE_AUTO_COMMIT.


opt noerrorshere 1; opt nowarningshere 1
Close

if(dbFile and !dbFile[0]) dbFile=0
str sf.expandpath(dbFile); if(!_unicode) sf.ConvertEncoding(0 CP_UTF8)

_i=__unqlite.unqlite_open(&m_db sf iif(flags flags 4))
if _i
	if(m_db) Close
	end F"{ERR_FAILED} to open database. Unqlite error {_i}"

if(flags2&1) __unqlite.unqlite_config(m_db __unqlite.UNQLITE_CONFIG_DISABLE_AUTO_COMMIT)

 todo, also in Sqlite.Open: auto-create folder, but only if not spec string.
