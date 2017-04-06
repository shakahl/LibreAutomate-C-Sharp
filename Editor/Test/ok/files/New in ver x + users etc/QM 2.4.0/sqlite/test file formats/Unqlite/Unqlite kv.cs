 Don't use unqlite instead of sqlite, because:
   Retrieving all records from uncached 1-frag file is ~10 times slower than sqlite. A little faster if cached.
   Not tested speed with J9x, but it probably would be much slower.
   Bigger file size because uses 4kb pages. Smallest file is 8 or 12 kb. In sqlite we can set page size.
   Once make PC to hang (100% disk) so that even CAD did not work, need to reset PC.
   New, possible more bugs.
   No database editor/viewser programs.
   Inconvenient to store something more than single key-value array. Need to learn and use J9x language etc. Anyway, if need to store more structured data, we can store it in sqlite eg as XML.
   Like sqlite, makes file fragmented, maybe less. It makes loading much slower.
   Would be difficult to optimize so that text would be stored separately like in sqlite.
   Does not have many useful features that sqlite has.

str sf="$my qm$\test\test.uql"

#compile "__Unqlite"
ref __unqlite
Unqlite x

#if 0
del- sf; err
x.Open(sf 0 0)

if(unqlite_kv_store(x "kee" 3 "DATAAA" 6)) end "error"
if(unqlite_kv_store(x "myy" 3 "WWWWWW" 6)) end "error"

rep 10000
	str s1.RandomString(5 25) s2.RandomString(50 350)
	if(unqlite_kv_store(x s1 s1.len s2 s2.len)) end "failed"

out GetFileFragmentation(sf)
ret
#else
DeleteFileCache sf
DeleteFileCacheAll "Q:"
1
#endif

PF
 _s.getfile(sf); PN; _s.all ;;1 frag 161925
FileAddToCache sf; PN ;;1 frag 115512, 38 frag 587103

x.Open(sf 0 0)
UnqliteCursor c.Init(x)
PN

int- t_n
unqlite_kv_cursor_first_entry(c.c)
rep
	if(!unqlite_kv_cursor_valid_entry(c.c)) break
	
	if(unqlite_kv_cursor_key_callback(c.c &UnqliteCursor_Callback 0)) x.E
	if(unqlite_kv_cursor_data_callback(c.c &UnqliteCursor_Callback +1)) x.E
	
	unqlite_kv_cursor_next_entry(c.c)
PN;PO
 out t_n/2

 file: 10000 records, 8.8 MB
 16 fragments:
 reload: 70  83399
 fresh: 70  2805417  
 1 fragments:
 reload: 70  53348
 fresh: 70  1502548   
 38 fragments:
 reload: 70  53348
 fresh: 70  4336132   
