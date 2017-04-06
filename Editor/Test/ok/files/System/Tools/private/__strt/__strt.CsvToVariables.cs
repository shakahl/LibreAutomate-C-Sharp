function $name1 str&var1 [n2] [~&v2] [n3] [~&v3] [n4] [~&v4] [n5] [~&v5] [n6] [~&v6] [n7] [~&v7] [n8] [~&v8] [n9] [~&v9] [n10] [~&v10]

 Gets values from grid variable.
 The values will be raw, ie does not call S() etc.

 namex - row name (first column).
 varx - receives value (second column) of namex row.

ICsv c._create; if(s.len) c.FromString(s)
lpstr* p=&name1; int i j
for i 0 getopt(nargs) 2
	str& r=+p[i+1]
	j=c.Find(p[i] 1)
	if(j<0) r.all; else r=c.Cell(i 1)
