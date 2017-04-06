int i j

Q &q
for i 0 1000000
	 j=(i&0xffff)|(i<<16)
	j=(5&0xffff)|(5<<16)

Q &qq
for i 0 1000000
	 j=MakeInt2(i i)
	j=MakeInt2(5 5)

Q &qqq
for i 0 1000000
	 j=MakeInt(i i)
	j=MakeInt(5 5)

Q &qqqq
outq
