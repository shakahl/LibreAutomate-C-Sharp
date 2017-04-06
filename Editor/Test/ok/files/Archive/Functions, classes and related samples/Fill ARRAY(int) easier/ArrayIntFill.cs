 /
function ARRAY(int)&a [e0] [e1] [e2] [e3] [e4] [e5] [e6] [e7] [e8] [e9] [e10] [e11] [e12] [e13] [e14] [e15] [e16] [e17] [e18] [e19] [e20] [e21] [e22] [e23] [e24] [e25] [e26] [e27] [e28] [e29]

 Creates int array with up to 30 elements.

 a - array variable.
 e0...e29 - element values.

 EXAMPLE
 ARRAY(int) a
 ArrayIntFill a 5 8 2
 int i
 for(i 0 a.len) out a[i]


int i n=getopt(nargs)-1
int* p=&e0
a.create(n)
for(i 0 n) a[i]=p[i]
