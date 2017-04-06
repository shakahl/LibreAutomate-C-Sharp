ARRAY(str) a="one[]two"
ARRAY(BSTR) b="ONE[]TWO"
ARRAY(int) c
VARIANT v
 a=b
 b=a
 c=b
 b=v.parray

 v=a
v=b
 b=v
a=v

 out a
 out b
