str sf="$my qm$\test\ok.db3"
Sqlite x.Open(sf 0 2)
x.Exec("PRAGMA foreign_keys=ON;BEGIN TRANSACTION")
int i nErrors
for i 1 11000
	rep() int j=RandomInt(1 11000); if(j!i) break
	str sql=
	F
	 UPDATE items SET id={-1} WHERE id={i};
	 UPDATE items SET id={-2} WHERE id={j};
	 UPDATE items SET id={i} WHERE id={-2};
	 UPDATE items SET id={j} WHERE id={-1};
	x.Exec(sql);; err nErrors+1
x.Exec("END TRANSACTION")
 out nErrors
