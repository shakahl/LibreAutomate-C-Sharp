/exe
1
dll "qm.exe" #SqliteCB param ncol $*data $*colNames
str sf.expandpath("$my qm$\test\ok.db3")
 str sf.expandpath("$my qm$\test\main.db3")
PF
Sqlite x.Open(sf)
PN
ARRAY(str) a
 x.Exec("select name from items" a)
 x.Exec("select name from items where instr(text, 'dlg')" a); out a.len
 x.Exec("select * from items" a)
 x.ExecF("select name from items" &Callback_Sqlite_ExecF2)
 x.ExecF("select * from items" &Callback_Sqlite_ExecF2)
 x.ExecF("select name from items" &SqliteCB)
 x.ExecF("SELECT text FROM items" &SqliteCB)
x.ExecF("select * from items" &SqliteCB)

 SqliteStatement y.Prepare(x "SELECT text FROM items WHERE rowid==7065")
PN
SqliteStatement y.Prepare(x "SELECT text FROM items")
 SqliteStatement yy.Prepare(x "SELECT text FROM items WHERE rowid==7065")
 SqliteStatement yyy.Prepare(x "SELECT text FROM items WHERE rowid==?1")
PN
int i
for i 0 10000
	 x.Exec("select text from items WHERE name=='Macro4'" a) ;;loooong
	 x.Exec("select text from items WHERE id==7065" a)
	 x.Exec("SELECT text FROM items WHERE rowid==7065" a) ;;1.5 s
	if(!y.FetchRow) out "not found"; break ;;1.1 s
	_s=y.GetText(0);; out _s.len
	 if(!yy.FetchRow) out "not found"; break ;;1.2 s
	 yy.Reset
	 yyy.BindInt(1 i+1)
	 if(!yyy.FetchRow) out "not found"; break
	 yyy.Reset
PN
PO
out a.len
out _s.len

 BEGIN PROJECT
 main_function  Macro2056
 exe_file  $my qm$\Macro2056.qmm
 flags  6
 guid  {30F582CB-1C98-4A10-84EA-C38D7ED29E37}
 END PROJECT

 speed: 1132743  135521  75334  59  86952  
 after defragmentation (was 123 fragments):
 speed: 105123  33056  70987  68  80643  

