str sf="$my qm$\test\ok.db3"
del- sf; err
Sqlite x.Open(sf 0 4|0x8000)
lpstr sql=
 PRAGMA page_size=512;PRAGMA secure_delete=ON;PRAGMA journal_mode=WAL;
 CREATE TABLE items(id INTEGER PRIMARY KEY,ord INT,name TEXT);
x.Exec(sql)

str st; int i
x.Exec("BEGIN")
for i 1 10
	st.all(10 2 'A'+i-1)
	x.Exec(F"INSERT INTO items VALUES({i},{i},'{st}')")
x.Exec("END")

x.Exec(F"UPDATE items SET ord=ord+10 WHERE ord>2")
 x.Exec(F"UPDATE items SET ord=ord+1000 WHERE ord>2")
 x.Exec(F"UPDATE items SET ord=ord+1000 WHERE ord=3")
 x.Exec("VACUUM")

 x.Exec(F"UPDATE items SET t='{s}' WHERE ord=1")
 x.Exec("INSERT INTO items VALUES(3,'E')")
