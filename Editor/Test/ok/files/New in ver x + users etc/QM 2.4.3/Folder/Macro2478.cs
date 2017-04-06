str s="863756251345236492865298367426457364235864275632746bar"
int i
PF
rep 1000
	i=findrx(s "\d+999")
PN
rep 1000
	i=findrx(s "(?>\d+)999")
PN
rep 1000
	i=findrx(s "\d++999")
PN
PO
