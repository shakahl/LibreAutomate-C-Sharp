out

 Create test array
ARRAY(VARIANT) a.create(3 12)
int i j; str s ss
ss=
 january
 february
 march
 april
 may
 june
 july
 august
 september
 october
 november
 december
for i 0 a.len
	a[0 i]=i+1
	s.getl(ss -i)
	a[1 i]=s
out "[]Unsorted array:"
for(i 0 a.len) out a[1 i]

 Sort by second column
SortVariantArray2Dim a 1
out "[]Sorted by second column:"
for(i 0 a.len) out a[1 i]

 Sort by first column
SortVariantArray2Dim a 0
out "[]Sorted by first column:"
for(i 0 a.len) out a[1 i]
