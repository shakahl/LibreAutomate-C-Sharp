del- "$my qm$\test\test.db3"; err
PF
Sqlite x.Open("$my qm$\test\test.db3" 0 1)
 Sqlite x.Open("" 0 1)
 Sqlite x.Open(0 0 0)
 Sqlite x.Open(":memory:" 0 0)
 Sqlite x.Open(":MEMORY:" 0 1)
PN
 x.Exec("PRAGMA foreign_keys=ON;")
 x.Exec("PRAGMA page_size=512;")
x.Exec("PRAGMA application_id=512;")
PN
x.Exec("CREATE TABLE items (id INTEGER PRIMARY KEY,name TEXT)")
PN
 rep(100) x.Exec("INSERT INTO items (name) VALUES ('test')")
PN;PO
