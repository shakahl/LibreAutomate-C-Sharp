out
str sf="$my qm$\test\ok.db3"
Sqlite x.Open(sf 0 2)

PF
str sql=
F
 PRAGMA foreign_keys=ON;
 UPDATE items SET flags={0x80000000} WHERE id=11400;
x.Exec(sql)
PN;PO
 UPDATE items SET flags={0x80000000} WHERE (id>=20 AND id<=30);
 UPDATE items SET flags={0x80000000} WHERE (id>=11000 AND id<=11400);

SHChangeNotify SHCNE_CREATE SHCNF_PATHW +@_s.expandpath(sf) 0
