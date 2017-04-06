ARRAY(int) a
int w=win("dlg_apihook" "#32770")
child "" "" w 0 0 0 a
int i
for i 0 a.len
	outw a[i]
	