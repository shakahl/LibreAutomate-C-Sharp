 The final macro.
 From synonyms list in macro "syn" removes all words that are not in macro "QM forum word list".
 Result: still too many synonyms that have no sense.
 Results are in macro "synonym for QM forum words".

out

str sfw.getmacro("QM forum word list")
sfw.fix(sfw.len/8)
IStringMap m._create; m.AddList(sfw)

str s.getmacro("syn")
s.stem(1)
s.findreplace(" ")
 out s
str ss so
foreach ss s
	int i j n=0
	ARRAY(str) a; tok ss a
	 remove duplicates
	for(i a.len-1 -1 -1)
		for(j 0 i) if(a[j]=a[i]) a.remove(i); break
	
	for i 0 a.len
		if(!m.Get(a[i])) if(i) continue; else break
		so+a[i]; so+","
		n+1
	if(n<2) continue
	so.rtrim(",")
	so+"[]"
 out so
 out so.len

 sort by forum word count
m.RemoveAll; m.AddList(so ",")
so.all
foreach s sfw
	lpstr syn=m.Get(s)
	if(syn) so.formata("%s = %s[]" s syn)
	else so.addline(s)
out so
out so.len
