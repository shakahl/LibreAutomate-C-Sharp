str dbfile="$desktop$\test.db3"
rep 10
	lock log_err_sqlite "Global\QM_log_err_sqlite"
	Sqlite db1.Open(dbfile)
	db1.Exec("CREATE TABLE IF NOT EXISTS table1 (A,B)")
	db1.Exec(F"INSERT INTO table1 VALUES ('one','two')")
	db1.Close
	lock- log_err_sqlite
