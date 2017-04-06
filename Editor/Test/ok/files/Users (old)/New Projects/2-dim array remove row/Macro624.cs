out
 ARRAY(str) a.create(2 4) ;;2 columns, 4 rows
ARRAY(str) a.createlb(2 5 4 -10) ;;2 columns, 4 rows
int i
for i a.lbound a.ubound+1
	a[5 i]=i
	a[6 i]=i+10

 ArrayStrRemoveRow a 1
ArrayStrRemoveRow a -10

for i a.lbound a.ubound+1
	out "%s %s" a[5 i] a[6 i]

