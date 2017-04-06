function $sql [ARRAY(str)&ar]

 Executes one or more SQL statements.
 SQL reference: <link>http://www.sqlite.org/lang.html</link>.
 Error if fails.

 sql - SQL statement. Can be several statements delimited with semicolon.
 ar - if used, receives query results as 2-dim array. Use with SELECT.


type ___SQLITEARRAY ARRAY(str)*a n all
___SQLITEARRAY a ;;used to avoid frequent ar reallocations. Makes ~2 times faster when many rows. Other ways (table, LL API) don't make faster.

if &ar
	ar=0
	a.a=&ar
	int cb=&sub.Callback

ExecF(sql cb &a)
err end _error

if(&ar) ar.redim(a.n) ;;free extra


#sub Callback
function[c]# ___SQLITEARRAY&a ncol $*data $*colNames

int i r

if !a.all
	a.all=1
	a.a.create(ncol a.all)
else if a.n=a.all
	a.all+a.all/4+4
	a.a.redim(a.all)
a.n+1
r=a.n-1

ARRAY(str)& ar=a.a
for i 0 ncol
	ar[i r]=data[i]

err+ ret 1
