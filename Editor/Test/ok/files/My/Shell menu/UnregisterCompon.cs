function $files
str f
foreach f files
	 if(!RegisterComComponent(f 7)) mes "failed"
	RegisterComComponent(f 7|8)
