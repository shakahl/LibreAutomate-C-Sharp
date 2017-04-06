function ARRAY(str)&a [flags] ;;flags: 0 interpolated, 2 as in HTML, 4 no empty attributes

 Gets all attributes.

 a - variable for attributes.
   Will create 2-dim array.
   In first dimension will be 2 elements: for names and for values.
   If fails, a will be empty.

 Added in: QM 2.3.3.

 EXAMPLE
 ARRAY(str) a; int i
 el.AttributesAll(a 2)
 for(i 0 a.len) out "%s=%s" a[0 i] a[1 i]


if(!el) end ERR_INIT
a=0
str s s2; int i
s=HTML; err
if(findrx(s "<\w+([^>]+)>" 0 0 s2 1)<0) ret
findrx(s2 "\b(\w+)\s*=" 0 4 a)
for i a.len-1 -1 -1
	a[0 i]=a[1 i]
	a[1 i]=Attribute(a[0 i] flags&3); err a[1 i]=""
	if(flags&4 and !a[1 i].len) a.remove(i)

 info: IE does not have a normal interface to get all attributes. IHTMLElement5.attributes needs IE8. IHTMLAttributeCollection too slow.
