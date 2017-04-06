 \
function Sqlite&x
 1
rep 10
	out __sqlite.sqlite3_wal_checkpoint_v2(x "main" __sqlite.SQLITE_CHECKPOINT_FULL 0 0)
	