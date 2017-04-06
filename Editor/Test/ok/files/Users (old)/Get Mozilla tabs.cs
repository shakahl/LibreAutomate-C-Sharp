type MOZILLATAB ~name ~url
ARRAY(MOZILLATAB) am
Acc a aa

a=acc("" "PROPERTYPAGE" "Mozilla Firefox" "MozillaWindowClass" "" 0x1080 0 0 "f"); err ret
rep
	a.Navigate("f" aa)
	MOZILLATAB& mt=am[am.redim(-1)]
	mt.name=aa.Name
	mt.url=aa.Value
	a.Navigate("n"); err break

int i
for i 0 am.len
	out "name: ''%s'', url: ''%s''" am[i].name am[i].url
