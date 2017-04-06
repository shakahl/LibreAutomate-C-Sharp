str sf="$my qm$\test\ok.db3"
if(!ConvertQmlToSqlite("$qm$\ok.qml" sf 1024*0)) out "FAILED"
Sqlite x.Open(sf 0 2)
x.Exec("PRAGMA foreign_keys=ON;BEGIN TRANSACTION")
int i nErrors
for i 1 11000
	rep() int j=RandomInt(1 11000); if(j!i) break
	str sql=
	F
	 UPDATE items SET ord={-1} WHERE ord={i};
	 UPDATE items SET ord={-2} WHERE ord={j};
	 UPDATE items SET ord={i} WHERE ord={-2};
	 UPDATE items SET ord={j} WHERE ord={-1};
	x.Exec(sql);; err nErrors+1
x.Exec("END TRANSACTION")
 out nErrors
