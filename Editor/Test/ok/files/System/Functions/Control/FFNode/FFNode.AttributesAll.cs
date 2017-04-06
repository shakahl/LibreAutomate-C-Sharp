function ARRAY(str)&a [flags] ;;flags: 4 no empty attributes

 Gets all attributes.

 a - variable for attributes.
   Will create 2-dim array.
   In first dimension will be 2 elements: for names and for values.
   If fails, a will be empty.

 EXAMPLE
 ARRAY(str) a; int i
 f.AttributesAll(a)
 for(i 0 a.len) out "%s=%s" a[0 i] a[1 i]


if(!node) end ERR_INIT
a=0
int i; word n(20) nr
 g1
ARRAY(BSTR) an.create(n) av.create(n); ARRAY(word) ansid.create(n)
node.get_attributes(n &an[0] &ansid[0] &av[0] &nr); err ret
if(!nr) ret
if(nr=n) n*2; goto g1

a.create(2 nr)
for i 0 nr
	a[0 i]=an[i]
	a[1 i]=av[i]

if(flags&4) for(i a.len-1 -1 -1) if(!a[1 i].len) a.remove(i)
