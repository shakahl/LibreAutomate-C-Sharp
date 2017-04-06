out
str sf="$my qm$\test\ok.db3"
Sqlite x.Open(sf 0 2)
int i j
ARRAY(str) a
PF
 x.Exec("SELECT name FROM items WHERE instr(name, 'Hook_')" a)
 x.Exec("SELECT name FROM items WHERE id IN(SELECT id FROM texts WHERE length(text)<4)" a)
 x.Exec("SELECT name FROM items WHERE id IN(SELECT id FROM texts WHERE instr(text, 'CsExec'))" a)
x.Exec("SELECT name FROM items WHERE instr(name,'CsScript') AND id IN(SELECT id FROM texts WHERE instr(text, 'CsExec'))" a)
 x.Exec("SELECT id FROM items INTERSECT SELECT id FROM texts WHERE instr(text, 'CsExec')" a) ;;slower
 x.Exec("SELECT text FROM texts WHERE length(text)<4" a)
 x.Exec("SELECT text FROM texts WHERE instr(text, 'CsExec')" a)
PN;PO

out a.len
for i 0 a.len
	_s.getl(a[0 i] 0)
	out _s
	