out
str sf="$my qm$\test\ok.db3"
Sqlite x.Open(sf 0 2)

PF
str sql=
 PRAGMA foreign_keys=ON;
 DELETE FROM items WHERE (id>=11000 AND id<=11400);
x.Exec(sql)
 DELETE FROM items WHERE (id>=2 AND id<=5);
 PN
 sql=
  DELETE FROM texts WHERE (id>=2 AND id<=11300);
 x.Exec(sql)
PN;PO
 INSERT INTO items (name) VALUES ('name4');
 x.Exec("VACUUM")

SHChangeNotify SHCNE_CREATE SHCNF_PATHW +@_s.expandpath(sf) 0
