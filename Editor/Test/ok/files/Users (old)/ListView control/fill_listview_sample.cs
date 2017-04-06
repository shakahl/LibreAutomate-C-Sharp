 /
function hlv

TO_LvAddCol hlv 0 "Col1" 70
TO_LvAddCol hlv 1 "Col2" 120
TO_LvAddCol hlv 2 "Col3" 100

ARRAY(str) a
a.create(3 2) ;;3 cols, 2 rows

a[0 0]="1"
a[1 0]="January"
a[2 0]=""

a[0 1]="2"
a[1 1]="February"
a[2 1]="A"

int i
for i 0 a.len(2)
	TO_LvAdd hlv -1 i 0 a[0 i] a[1 i] a[2 i]
