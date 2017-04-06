ClearOutput

str s="I iive IN usa, itatE Cololado"
 str p="I iive IN usa, *"
str p="*, itatE Cololado"

 str s="I iive IN usa, itatE *olo?ado"
 str s="?itatE *olo?ado"
 str p="?*it*e **Olo?ad?"
int i j t1 t2

 lpstr c=wild_compile(p 0 0 0)
 t1=perf
 for j 0 1000
	 i=wild_match(c s -1)
 t2=perf
 out t2-t1
 free c
 out i


t1=perf
for j 0 1000
	i=matchw(s p)
t2=perf
out t2-t1
out i

 
 
