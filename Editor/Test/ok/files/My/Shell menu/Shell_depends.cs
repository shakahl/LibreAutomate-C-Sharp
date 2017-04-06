function $files
str f
foreach f files
	run "Q:\Programs\Depends\Depends.exe" f "" "" 0x30000
	break
