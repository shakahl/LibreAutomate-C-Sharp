del- "$my qm$\test\test.db3"; err
Sqlite x.Open("$my qm$\test\test.db3")
x.Exec("CREATE TABLE IF NOT EXISTS test(a INT, b TEXT)")
