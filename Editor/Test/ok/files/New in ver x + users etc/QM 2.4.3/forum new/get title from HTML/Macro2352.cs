out
Dir d
foreach(d "$qm$\htmlhelp\*.html" FE_Dir 4)
	str path=d.FullPath
	out path
	if(GetTitleFromHTML(path _s 1|2)) out _s
	else out "<><c 0xff>NOT FOUND</c>"
	
