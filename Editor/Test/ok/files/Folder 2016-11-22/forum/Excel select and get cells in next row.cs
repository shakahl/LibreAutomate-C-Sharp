 int w2=win("gpserv - Remote Desktop Connection" "TscShellContainerClass")
 ...
	ExcelSheet es.Init
	Excel.Range r=es._Range("sel")
	str s1=r.Value
	str s2=r.Offset(0 1).Value
	r.Offset(1 0).Select
	out F"{s1} {s2}" ;;delete this line
	
	 act w2
	 paste s1
	 'TT
	 paste s2
	 'TTT
