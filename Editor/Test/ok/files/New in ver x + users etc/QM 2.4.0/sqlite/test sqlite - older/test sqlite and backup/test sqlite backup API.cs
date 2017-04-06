out
str sx("$my qm$\test\ok.db3") sy("$my qm$\test\ok-backup.db3") sz("$my qm$\test\ok-z.db3")

PF
Sqlite x.Open(sx)
Sqlite y.Open(sy)

byte* b=__sqlite.sqlite3_backup_init(y "main" x "main")
 out b
if(!b) ret

int r=__sqlite.sqlite3_backup_step(b -1)
if(r!__sqlite.SQLITE_DONE) out r

r=__sqlite.sqlite3_backup_finish(b)
if(r!__sqlite.SQLITE_OK) out r

x.Close
y.Close
PN

FileCopy sx sz
PN; PO

out "---"
out GetFileFragmentation(sx)
out GetFileFragmentation(sy)
out GetFileFragmentation(sz)
