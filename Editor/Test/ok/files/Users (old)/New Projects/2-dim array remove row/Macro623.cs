out
ARRAY(str) a.create(2 4) ;;2 columns, 4 rows
int i
for i a.lbound a.ubound+1
	a[0 i]=i
	a[1 i]=i+10

ArrayStrRemoveRow a 1

for i a.lbound a.ubound+1
	out "%s %s" a[0 i] a[1 i]

