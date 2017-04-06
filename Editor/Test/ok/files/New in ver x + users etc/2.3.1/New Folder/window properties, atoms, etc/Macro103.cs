out
ARRAY(int) aw ac; int i j ud
win "" "*" "qm" 0x800 0 0 aw
for i 0 aw.len
	child "" "" aw[i] 0 0 0 ac
	out "---------------"
	zw aw[i]
	 out GetWindowLong(aw[i] GWL_USERDATA)
	OutWinProps aw[i]
	for j 0 ac.len
		 ud=GetWindowLong(ac[j] GWL_USERDATA)
		 if(ud) zw ac[j]
		zw ac[j]
		OutWinProps ac[j]
 GWL_USERDATA
