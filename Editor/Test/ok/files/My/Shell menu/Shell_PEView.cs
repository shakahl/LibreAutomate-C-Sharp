function $files
str f
foreach f files
	run "Q:\Programs\PEview\PEview.exe" f "" "" 0x30000
	break
