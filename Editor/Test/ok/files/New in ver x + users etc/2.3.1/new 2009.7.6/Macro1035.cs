int w1=win("Pažintys ''Draugas.lt'' - Klubo nariai - Mozilla Firefox" "MozillaUIWindowClass")
ARRAY(int) a
child "" "" w1 0 0 0 a
int i
out
for i 0 a.len
	int h=a[i]
	if(hid(h)) continue
	Acc e=acc(h)
	if(e.State&STATE_SYSTEM_INVISIBLE) continue
	e.Navigate("f")
	 e.Navigate("f"); err
	if(e.State&STATE_SYSTEM_INVISIBLE) continue
	zw h
	e.Role(_s); out _s
	if(e.Role!=ROLE_SYSTEM_DOCUMENT) continue
	int x y cx cy
	e.Location(x y cx cy)
	RECT r; r.left=x; r.top=y; r.right=x+cx; r.bottom=y+cy
	OnScreenRect 1 &r
	2
	OnScreenRect 2 &r
	