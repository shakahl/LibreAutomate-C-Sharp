out

 Create test array
ARRAY(VARIANT) a.createlb(3 1 12 1)
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
	a[1 i+1]=i
	s.getl(ss -i)
	a[2 i+1]=s
out "[]Unsorted array:"
for(i 0 a.len) out a[2 i+1]

 Sort by second column
SortVariantArray2Dim a 2 1
out "[]Sorted by second column:"
for(i 0 a.len) out a[2 i+1]

 Sort by first column
SortVariantArray2Dim a 1 1
out "[]Sorted by first column:"
for(i 0 a.len) out a[2 i+1]
