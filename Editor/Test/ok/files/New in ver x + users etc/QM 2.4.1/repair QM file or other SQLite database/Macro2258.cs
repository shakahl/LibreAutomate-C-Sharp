Sqlite x.Open("$my qm$\test\test2 - Copy.qml")
ARRAY(str) a
 x.Exec("VACUUM" a)
 x.Exec("PRAGMA integrity_check;" a)
x.Exec("PRAGMA quick_check;" a)
out a

