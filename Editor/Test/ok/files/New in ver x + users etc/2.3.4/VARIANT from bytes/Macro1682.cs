VARIANT d
VariantFromBytes2 "52 80 00 d2" d
int i
for i 0 d.bstrVal.len
	out "0x%x" d.bstrVal[i]

 output:
 0x52
 0x80
 0x0
 0xd2

 _s=d
 for i 0 _s.len
	 out "0x%x" _s[i]
