function Sqlite&x $sql nCol [ARRAY(int)&a]

SqliteStatement p
p.Prepare(x sql)
rep
	 if(!p.FetchRow) break
	 for(_i 0 4) p.GetInt(_i)
	 for(_i 0 4)  p.GetText(_i)
	
	if(__sqlite.sqlite3_step(p.p)!=__sqlite.SQLITE_ROW) break
	if nCol=8
		int fid=__sqlite.sqlite3_column_int(p.p 0)
		int folder=__sqlite.sqlite3_column_int(p.p 1)
		int flags=__sqlite.sqlite3_column_int(p.p 2)
		int date=__sqlite.sqlite3_column_int(p.p 3)
		if(&a and flags&0x1000000) a[g_n]=fid; g_n+1
		for(_i 4 8) __sqlite.sqlite3_column_text(p.p _i)
	else
		__sqlite.sqlite3_column_text(p.p 1)
