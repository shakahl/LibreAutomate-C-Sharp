/exe
 out
 Sqlite+ x; if(!x.conn) x.Open("$my qm$\test\ok.db3")
Sqlite x.Open("$my qm$\test\ok.db3")
 Sqlite x.Open("$my qm$\test\ok.db3" 6|__sqlite.SQLITE_OPEN_FULLMUTEX)
 Sqlite x.Open("$my qm$\test\ok.db3" 0 1)
__sqlite.sqlite3_busy_timeout(x.conn 1000)
int t=GetTickCount
int n
rep
	n+1
	x.ExecF("SELECT * FROM items" &Callback_Sqlite_ExecF2)
	 int e=0
	 x.Exec("BEGIN TRANSACTION")
	rep(10)
		x.Exec(F"UPDATE items SET trigger='{_s.RandomString(5 20 `a-z`)}' WHERE id={RandomInt(1 1000)}")
		 x.Exec(F"INSERT INTO items (triggerEtc) VALUES ('{_s.RandomString(5 20 `a-z`)}')")
		 err e+1
	 x.Exec("END TRANSACTION")
	 out e
	0.01
	if(GetTickCount-t>2000) break

out n

 BEGIN PROJECT
 main_function  Function255
 exe_file  $my qm$\Function255.qmm
 flags  6
 guid  {5CB5FCD6-C350-4926-A1B0-B66D6756505D}
 END PROJECT
