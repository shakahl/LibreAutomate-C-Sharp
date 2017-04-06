 open multi
ARRAY(str) a
if(OpenSaveDialog(0 0 "" "" 0 "" 0 a))
	for(int'i 0 a.len) out a[i]

  save
 str s
 if(OpenSaveDialog(1 s "" "" 0 "" 0 0))
	 out s
	