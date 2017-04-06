str s="[9]static LPCSTR rx1000[ 1000 ]={"
int i
for i 0 1000
	s.formata("''r%i''," i)
s.replace("};[]" s.len-1 1)
out s
