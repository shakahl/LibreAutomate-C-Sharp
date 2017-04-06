out
Sqlite x.Open("$my qm$\test\ok.QML" 0 4 500)
 SqliteStatement p.Prepare(x "SELECT flags FROM items WHERE id=100"); p.FetchRow;; p.Reset
#compile "__SqliteBlob"
 SqliteBlob b.Init(x "items" "name"); b.GetStr(50 _s)
int ht=mac("Function259" 0 &x)
rep
	if(!WaitForSingleObject(ht 0)) break
	if(!x.ExecGetInt("SELECT flags FROM items WHERE id=100")) out "SELECT failed"
	x.Exec("UPDATE items SET flags=0 WHERE id=1")
	if(!x.ExecGetInt("SELECT flags FROM items WHERE id=100")) out "SELECT failed"
	 0.1
