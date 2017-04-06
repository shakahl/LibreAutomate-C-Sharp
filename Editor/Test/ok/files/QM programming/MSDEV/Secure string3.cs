 encrypts string using QuickMix

dll "qm.exe" __Test str*s

str s.getclip ss="char s[ ]=''"
s.trim
__Test &s
int i
for i 0 s.len
	int c=s[i]
	ss.formata("\x%02x" c)
ss+"'';"
out ss
ss.setclip
