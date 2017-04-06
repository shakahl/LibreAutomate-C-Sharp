out
int w=win("Firefox" "Mozilla*WindowClass" "" 0x804)
ARRAY(str) a at; int i
FirefoxGetLinks w a at
for i 0 a.len
	out F"{at[i]%%-35s} {a[i]}"
