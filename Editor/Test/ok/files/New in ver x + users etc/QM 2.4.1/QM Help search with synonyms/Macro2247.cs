str s.getmacro("phpbb_search_wordlist.csv")
ICsv x._create
x.FromString(s)

ICsv xx._create
IStringMap m._create
m.Flags=4
int i n nUnique
for i 0 x.RowCount
	lpstr w=x.Cell(i 1)
	if(findcs(w "_0123456789")>=0) continue
	str stem=w; stem.stem
	lpstr k=m.Get(stem)
	lpstr s1 s2 s3
	if !k
		s1=stem;
		n=xx.AddRowLA(-1 3)
	else n=val(k)
	m.IntAdd(stem n+1)
out "%i %i" x.RowCount nUnique

#ret
x.Sort(4|0x100)
x.ToString(s)
ShowText "" s
 s.setmacro("phpbb_search_wordlist.csv")
