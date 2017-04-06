function $files
str f
foreach f files
	 if(!RegisterComComponent(f 6)) mes "failed"
	RegisterComComponent(f 6|8)
