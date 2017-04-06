out
int w=win("Internet Explorer" "IEFrame")
Htm e=htm("BODY" "" "" w "" 0 0x20)
ARRAY(str) a at; int i
e.GetLinks_(1 a at)
for i 0 a.len
	out F"{at[i]%%-35s}  {a[i]}"
