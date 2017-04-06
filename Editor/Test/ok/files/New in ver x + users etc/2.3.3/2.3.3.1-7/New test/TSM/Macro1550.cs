out
int i
BSTR b.alloc(1)
for i 1 1000
	b[0]=i
	_s=b
	out "%s  %i %i" _s IsCharAlphaNumericW(i) IsCharAlphaW(i)
