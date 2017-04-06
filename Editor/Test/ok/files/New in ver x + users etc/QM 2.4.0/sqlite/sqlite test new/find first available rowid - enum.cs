out
int w=win("SQLite2009 Pro - The Best GUI to manage your SQLite3 databases." "ThunderRT6FormDC"); if(w) clo w
str sf="$my qm$\test\ok.db3"
if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"
Sqlite y.Open(sf 0 2)
 y.Exec("DELETE FROM items WHERE id=11000")
y.Exec("DELETE FROM items WHERE id>1000 AND id<11000")
y.Close

dll "qm.exe" #TestSqliteFindInsertRow $db3File
out TestSqliteFindInsertRow(sf)

 speed: 5500
