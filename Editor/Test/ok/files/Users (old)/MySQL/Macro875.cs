Database db.Open("DRIVER={MySQL ODBC 5.1 Driver};DATABASE=menagerie;USER=root;PASSWORD=p;")
ARRAY(str) a; int r c
db.QueryArr("SELECT * FROM pet" a)
for r 0 a.len(2)
	out "-- Record %i --" r+1
	for c 0 a.len(1)
		out a[c r]

