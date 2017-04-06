function# param double&a double&b

&a=&a+(2*sizeof(a))
&b=&b+(2*sizeof(b))

if(a<b) ret -1
if(a>b) ret 1
