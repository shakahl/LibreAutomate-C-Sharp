out
str s.all(100)
int i
for i 1 128
	GetKeyNameText i<<16 s 100
	 GetKeyNameText (i<<16)|0x01000000 s 100 ;;extended
	out "0x%X %s" i s.lpstr
	