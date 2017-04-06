 /
function ARRAY(int)&a $values

 Creates int array and fills with values specified in string.

 a - array variable.
 values - element values, like "4 -2 85". Separators can be spaces, tabs, new lines, commas.

 EXAMPLE
 ARRAY(int) a
 ArrayIntFill2 a "4 -2 85"
 int i
 for(i 0 a.len) out a[i]


ARRAY(lpstr) b
tok values b -1 " [9][],"
int i n=b.len
a.create(n)
for(i 0 n) a[i]=val(b[i])
