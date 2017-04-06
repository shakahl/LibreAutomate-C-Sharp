 enumerates progids of creatable .NET classes

out

int n i
BSTR b.alloc(1000)

for i 0 1000000000
	n=1000
	if(RegEnumKeyExW(HKEY_CLASSES_ROOT i b &n 0 0 0 0)) break
	str s.ansi(b)
	if(s.begi("System.")) out s
