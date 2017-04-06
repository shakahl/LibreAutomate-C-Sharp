ICsv x._create
rep 10
	x.AddRow2(-1 "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb")

str sf="$my qm$\test\safe.csv"
PF
x.ToFile(sf 0x300)
PN;PO
x=0

ICsv xx._create
xx.FromFile(sf)
xx.ToString(_s); out _s.len
