Sqlite x.Open("")
ARRAY(str) a; int i
x.Exec("PRAGMA compile_options;" a)
for i 0 a.len
	out a[0 i]


 .7.17
 ENABLE_COLUMN_METADATA
 ENABLE_FTS3
 ENABLE_FTS3_PARENTHESIS ;;only in our dll, not in the downloaded
 ENABLE_RTREE
 SYSTEM_MALLOC
 THREADSAFE=1
