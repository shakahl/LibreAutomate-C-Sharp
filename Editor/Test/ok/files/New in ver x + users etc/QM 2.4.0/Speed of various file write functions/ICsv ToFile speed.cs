ICsv x._create
rep 11000
	x.AddRow2(-1 "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb")

str sf.expandpath("$my qm$\test\safe.csv")
PF
x.ToFile(sf)
 _s.flags=3; x.ToString(_s); PN; _s.setfile(sf)
PN;PO
out GetFileFragmentation(sf)

 speed: 190592  , 7-33 frag
 speed: 26022  3752  
 speed: 14567  , 4-5 frag
