 speed: 64/82,  29543  1365  70421  111987  149619  
 speed: 57/113,  30127  1651  70231  119769  106145  
 speed: 37/82,  36797  1214  72385  113390  91188  

 speed: 36/59,  32250  1329  71873  112272  96747  
 speed: 60/59,  30961  1272  68423  99999  96660  
 speed: 26/30,  30404  1210  109576  167643  162577  


str sf sfSrc
sf="$my qm$\test\ok.QML"
 sf="$my qm$\test\admin\ok.QML"
 sf="G:\ok.QML" ;;old, 1GB
 sf="I:\ok.QML" ;;new, 8GB
 sf="J:\ok.QML"
 sf="\\GINTARAS\Q\my qm\test\ok.QML"
 sf="$qm$\_qml\ok.QML"
 sf="$my qm$\test\sqlite export.QML"
 sf="C:\Users\G\„Google“ diskas\test\ok.qml"

sfSrc="$qm$\ok.qml"
sfSrc="$my qm$\main.qml"
 sfSrc="$qm$\system.qml"
 sfSrc="$my qm$\empty.qml"
 sfSrc="$my qm$\test\Archive.qml"
 sfSrc="$qm$\_qml\obsolete.qml"
 sfSrc="$my qm$\test\large.qml"
 sfSrc="$my qm$\test\3mb.qml"
 sfSrc="$my qm$\test\sqlite export.old.qml"
rset sf "file debug" "Software\GinDi\QM2\settings"
rset sfSrc "test file src"
 goto g1
 ret
#if 1
PF
 ConvertQmlToSqlite("$my qm$\test\sqlite export.old.qml" "$my qm$\test\sqlite export.QML" 1024*0 1)
 if(!ConvertQmlToSqlite("$qm$\system.qml" "$my qm$\test\System.QML" 1024*0 0)) end "FAILED"
if(!ConvertQmlToSqlite(sfSrc sf 1024*0 1)) end "FAILED"
PN;PO
out GetFileFragmentation(sf)
#else
 out
 Dir d
 foreach(d "$QM$\*.qml" FE_Dir 4)
	 str path=d.FileName(1)
	 out path
	 str s1.getfilename(path) s2.from("$my qm$\test\" s1 ".QML")
	 if(!ConvertQmlToSqlite(path s2)) out "FAILED"
	 out GetFileFragmentation(s2); err
#endif
SHChangeNotify SHCNE_CREATE SHCNF_PATHW +@_s.expandpath(sf) 0

 _s=" "; _s[0]=1; _s.setfile(sf 18 1)

 g1
#ret
 Sqlite x.Open(sf 0)
Sqlite x.Open(sf 1)
 x.Exec("INSERT OR REPLACE INTO sqlite_master(rowid,type,name) VALUES(-1000,'defrag',5)") ;;error: table sqlite_master may not be modified
out x.ExecGetText(_s "PRAGMA journal_mode")
 ARRAY(str) a; x.Exec("SELECT * FROM sqlite_master" a); out _s.From2dimArray(a)
 x.Exec("PRAGMA foreign_keys=ON")
  x.Exec("DELETE FROM items WHERE (id>10)")
 x.Exec(F"UPDATE items SET flags={0x80000000} WHERE (id>=3 AND id<=11410)")

 speed: 166874  75346  615274  = 862
 speed: 2439  170441  277849  = 470
