 displays Unicode character codes, characters, and number of UTF-8 bytes when QM is running in Unicode mode
out
int i
for i 1 0x10000
	str s=UnicodeCharToString(i)
	out "%i %s    [%i]" i s s.len
