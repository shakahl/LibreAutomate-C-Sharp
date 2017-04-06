 /
function !*ptr nBytes [flags] ;;flags: 1 also show as text

 Displays binary data in QM output.

 ptr - pointer to the data. For example, str variable containing the data.
 nBytes - data length.

 Added in: QM 2.3.0.


out _s.encrypt(8 _s.fromn(ptr nBytes) "" 1)
if(flags&1)
	str s.all(nBytes*3 2 32)
	int i c
	for(i 0 nBytes)
		c=ptr[i]
		if(c<32) continue
		if(c>=128 and _unicode) continue
		s[i*3+1]=c
	out s
