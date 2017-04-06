IStringMap m._create
m.Flags=4
 m.Flags=2
str s.getmacro("phpbb_posts.csv")
s.lcase
s.stem(3)
ARRAY(lpstr) a
tok s a -1 " " 1
int i j n
for i 0 a.len
	lpstr k=a[i]
	j=len(k)
	if(j<2)
		if(SelStr(0 k "a" "i")) continue
	if(findcs(k "_0123456789")>=0) continue
	 m.Add(k)
	
	if(m.IntGet(k n)) n+1; else n=1
	m.IntAdd(k n)

 m.GetList(_s)
m.GetList(_s ",")
 out _s
ICsv x._create
x.FromString(_s)
x.Sort(0x104 1)
x.ToString(_s)
out _s
