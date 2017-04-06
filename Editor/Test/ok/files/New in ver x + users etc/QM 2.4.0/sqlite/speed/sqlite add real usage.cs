 out
str sf; rget sf "file debug" "Software\GinDi\QM2\settings"
 g1
str sfSrc; rget sfSrc "test file src"; if(!ConvertQmlToSqlite(sfSrc sf 1024*0 1)) end "FAILED"
 out GetFileFragmentation(sf)

PF
int i nr upd nIns trans(1) nbAded nRep(15)
 nRep=RandomInt(8 18)
ARRAY(str) a
rep nRep/2
 rep 1
	 Sqlite x.Open(sf 0 2)
	Sqlite x.Open(sf 0 0); x.Exec("PRAGMA synchronous=NORMAL")
	 x.Exec("PRAGMA main.locking_mode=EXCLUSIVE") ;;faster when doing multiple changes without transaction (HDD <=2.5 times, flash <=2 times)
	 x.Exec("PRAGMA temp_store = MEMORY") ;;uses journal file anyway
	 x.Exec("PRAGMA journal_mode = TRUNCATE") ;;30% slower. PERSIST same speed.
	 x.Exec("PRAGMA journal_mode = MEMORY") ;;faster 2-4 times by default, but only ~30% if optimized by locking_mode=EXCLUSIVE. Recommended for flash, eg in portable QM.
	 x.Exec("PRAGMA journal_mode = WAL") ;;20% slower. More files used, but same add/delete file count as default.
	 x.Exec("PRAGMA journal_mode = DELETE") ;;default, use to reset WAL when testing
	 x.Exec(F"PRAGMA temp_store_directory = '{_s.expandpath(`$temp$`)}'") ;;does not change journal file directory
	nr=x.ExecGetInt("SELECT max(id) FROM items")+1
	if(trans) x.Exec("BEGIN TRANSACTION")
	rep nRep
		i=RandomInt(1 nr-1) ;;out i
		str name.RandomString(4 30); name.SqlEscape
		str trig.RandomString(0 100*RandomInt(0 1)*RandomInt(0 1)); trig.SqlEscape
		str text.RandomString(0 pow(RandomInt(0 6) RandomInt(0 5))/2+50*RandomInt(0 10)); text.SqlEscape
		 out "%i %i %i" name.len trig.len text.len; continue
		
		upd=RandomInt(0 15)
		sel upd
			case 0
				nIns+1
				x.Exec(F"INSERT into items (name,trigger,flags,pid,ord) VALUES ('{name}','{trig}',1,0,1)")
				if(text.len&1) x.Exec(F"UPDATE texts SET text='{text}' WHERE id={__sqlite.sqlite3_last_insert_rowid(x)}"); else text.fix(0)
				nbAded+name.len+trig.len+text.len+20
			case 1
				x.Exec(F"UPDATE items SET name='{name}' WHERE id=={i}")
			case 2
				x.Exec(F"UPDATE items SET trigger='{trig}' WHERE id=={i}")
				nbAded+trig.len/4
			case else
				x.Exec(F"SELECT text FROM texts WHERE id=={i}" a)
				rep 5
					text+"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
					nbAded+text.len-iif(a.len a[0 0].len 0)
					x.Exec(F"UPDATE texts SET text='{text}' WHERE id=={i}")
	
	 x.Exec("ANALYZE")
	if(trans) x.Exec("END TRANSACTION")
	int nChanges; nChanges+=__sqlite.sqlite3_total_changes(x)
	int pagesOld(x.ExecGetInt("PRAGMA user_version")) pagesNew(x.ExecGetInt("PRAGMA page_count"))
	
	ARRAY(str) aa; x.Exec("PRAGMA integrity_check" aa); _s=aa; if(_s!"ok") end _s
	x.Close
 goto g1
PN; PO
out "Inserted %i rows, add %i KB." nIns nbAded/1024
PF
int frag=GetFileFragmentation(sf)
PN
int counter
__HFile hf.Create(sf OPEN_EXISTING GENERIC_READ FILE_SHARE_READ); _s.all(100); if(ReadFile(hf _s 28 &_i 0)) counter=_s[26]<<8|_s[27]
hf.Close
PN;PO
out "frag=%i, nChanges=%i, counter=%i,  pagesOld=%i pagesNew=%i" frag nChanges counter pagesOld pagesNew

#if 0
1
x.Open(sf)
x.Exec("VACUUM")
x.Close
out GetFileFragmentation(sf)

FileCopy sf F"{sf}.tmp"; FileMove F"{sf}.tmp" sf
out GetFileFragmentation(sf)
#endif
SHChangeNotify SHCNE_CREATE SHCNF_PATHW +@_s.expandpath("$my qm$\test\ok.db3") 0
