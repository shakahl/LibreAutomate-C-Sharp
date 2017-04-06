function$ !*ptr nBytes [flags] ;;flags: 1 also show as text

 Formats binary data as hex string.

 ptr - pointer to the data. For exampe, str variable containing the data.
 nBytes - data length.

 Added in: QM 2.4.0.


encrypt(8 _s.fromn(ptr nBytes) "" 1)
if(flags&1)
	str s.all(nBytes*3 2 32)
	int i c
	for(i 0 nBytes)
		c=ptr[i]
		if(c<32) continue
		if(c>=128 and _unicode) continue
		s[i*3+1]=c
	addline(s)
ret this
