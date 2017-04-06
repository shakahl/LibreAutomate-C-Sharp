del- "$my qm$\test\sub"; err
Sqlite x.Open("$my qm$\test\sub\g.db3")
x.Exec("CREATE TABLE items2 (id)")
 x.Exec("CREATE TABLE IF NOT EXISTS items2 (id)")
x.Exec("INSERT INTO items2 (id) VALUES (3)")
