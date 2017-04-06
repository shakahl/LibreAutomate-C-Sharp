out
str sf="$my qm$\test\ok.db3"
Sqlite x.Open(sf 0 2)
PF
ARRAY(str) a
 x.Exec("SELECT ord FROM items WHERE ord>=1 AND ord<=100" a)
 x.Exec("SELECT ord FROM items WHERE ord BETWEEN 1 AND 100" a)
x.Exec("SELECT ord FROM items WHERE ord BETWEEN 1 AND 100 ORDER BY ord" a)
 x.Exec("SELECT ord FROM items WHERE ord BETWEEN 1 AND 1000" a)
 x.Exec("SELECT ord FROM items WHERE ord BETWEEN 10000 AND 11000" a)
 x.Exec("SELECT ord FROM items WHERE ord BETWEEN 10000 AND 10100" a)
PN;PO
 out a.len
out _s.From2dimArray(a 1)
