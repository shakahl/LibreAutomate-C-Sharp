 /test sqlite speed + dll loading
function $sf ARRAY(int)&a

 dll "qm.exe" #SqliteCB param ncol $*data $*colNames

int useArray=1; if(useArray) int+ g_n=0; a.create(15000)
PN
Sqlite x.Open(sf 0 2)
SqliteStatement p
 x.Exec(F"PRAGMA mmap_size={16*1024*1024}") ;;generally makes slower, although sometimes gets texts a little faster
 x.Exec("PRAGMA cache_size = 10")
 x.Exec("PRAGMA main.locking_mode=EXCLUSIVE") ;;same speed
 x.Exec("PRAGMA temp_store = MEMORY") ;;same speed
PN
rep 1
	 x.ExecF("SELECT * FROM items" &Callback_Sqlite_ExecF3 &a)
	 x.ExecF("SELECT * FROM items" &SqliteCB)
	 x.ExecF("SELECT * FROM items" &Callback_Sqlite_ExecF2)
	 x.ExecF("SELECT id,folder,flags,date,name,trigger,image,link FROM items" &Callback_Sqlite_ExecF2)
	 x.ExecF("SELECT id,name,trigger,flags,folder FROM items" &Callback_Sqlite_ExecF2)
	Sqlite_TestSelect x "SELECT * FROM items" 8 iif(useArray &a 0)
PN
rep 1
	if !useArray
		 x.ExecF("SELECT * FROM texts" &Callback_Sqlite_ExecF2)
		Sqlite_TestSelect x "SELECT * FROM texts" 1
		ret
 #ret

int i
#if 1
 p.Prepare(x "SELECT text FROM texts WHERE id=?1")
 SqliteStatement p.Prepare(x "SELECT text FROM items WHERE id=?1")
 a.create(x.GetTableRowCount("items")); for(i 0 a.len) a[i]=i
a.redim(g_n);; for(i 0 a.len) out a[i]
 a.shuffle
 a.redim(a.len/10)

rep 1
	 for i 0 a.len
		  out a[i]
		  spe
		 p.Reset
		 p.BindInt(1 a[i])
		 if(!p.FetchRow) continue
		 p.GetText(0)
		  byte* b=p.GetBlob(0 _i); _s.fromn(b _i); _s.decrypt(32);; out _s
	
	byte* b; int j nb hr
	str st.flags=3; st.all(100000)
	for i 0 a.len
		j=a[i];; out j
		if(b) hr=__sqlite.sqlite3_blob_reopen(b j)
		else hr=__sqlite.sqlite3_blob_open(x 0 "texts" "text" j 0 &b)
		if(hr)
			out F"error (open) at {j}"
			__sqlite.sqlite3_blob_close(b); b=0; continue
		
		nb=__sqlite.sqlite3_blob_bytes(b)
		 out F"{j}={nb}"
		if(st.nc<nb) st.all(nb)
		if(__sqlite.sqlite3_blob_read(b st nb 0)) out F"error (read) at {j}"
		st[nb]=0
		 out st.lpstr
		
	__sqlite.sqlite3_blob_close(b); b=0


#else
 x.ExecF("SELECT text FROM texts" &SqliteCB)
 x.ExecF("SELECT text FROM texts" &Callback_Sqlite_ExecF2)

str q="SELECT text FROM texts WHERE"
for(i 0 a.len) q+F" {iif(i `OR ` ``)}id={a[i]}"
 out q; ret
x.ExecF(q &Callback_Sqlite_ExecF2)
#endif
 x.Exec("END TRANSACTION")
