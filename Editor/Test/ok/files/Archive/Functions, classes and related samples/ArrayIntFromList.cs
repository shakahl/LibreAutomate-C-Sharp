 /
function ARRAY(int)&a $data

 Populates int array from list of numbers.

 a - receives result.
 data - list of numbers, like "1 12 7 2"


ARRAY(lpstr) b
tok data b -1
a.create(b.len)
int i
for i 0 b.len
	a[i]=val(b[i])
