out

str s=
 she sels
 sea shels

#compile Caspell
Caspell k.Init("en_US")

ARRAY(str) a; int i
tok s a
for i 0 a.len
	str& w=a[i]
	if(k.Check(w)) continue
	ARRAY(str) b
	k.Suggest(w b)
	str sug=b; sug.findreplace("[]" ", ")
	out F"{w}: {sug}"
