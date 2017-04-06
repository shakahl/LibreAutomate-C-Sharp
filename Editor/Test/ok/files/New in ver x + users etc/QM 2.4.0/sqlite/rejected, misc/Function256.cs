 /test sqlite speed + dll loading
function $sf ARRAY(int)&a

 dll "qm.exe" #SqliteCB param ncol $*data $*colNames

PN
Sqlite x.Open(sf 0 2)
 x.Exec("BEGIN TRANSACTION")
PN
rep 1
	 x.ExecF("SELECT * FROM items" &SqliteCB)
	 x.ExecF("SELECT id,name,triggerEtc,flagsEtc,folder FROM items" &SqliteCB)
	x.ExecF("SELECT id,name,triggerEtc,flagsEtc,folder FROM items" &Callback_Sqlite_ExecF2)

PN
int i
#if 1
SqliteStatement p.Prepare(x "SELECT text FROM texts WHERE id=?1")
 SqliteStatement p.Prepare(x "SELECT text FROM items WHERE id=?1")
 a.create(x.GetTableRowCount("items")); for(i 0 a.len) a[i]=i
 a.shuffle
 a.redim(a.len/10)
for i 0 a.len
	 out a[i]
	 spe
	p.Reset
	p.BindInt(1 a[i])
	if(!p.FetchRow) continue
	p.GetText(0)
	 byte* b=p.GetBlob(0 _i); _s.fromn(b _i); _s.decrypt(32);; out _s
#else
 x.ExecF("SELECT text FROM texts" &SqliteCB)
 x.ExecF("SELECT text FROM texts" &Callback_Sqlite_ExecF2)

str q="SELECT text FROM texts WHERE"
for(i 0 a.len) q+F" {iif(i `OR ` ``)}id={a[i]}"
 out q; ret
x.ExecF(q &Callback_Sqlite_ExecF2)
#endif
 x.Exec("END TRANSACTION")
