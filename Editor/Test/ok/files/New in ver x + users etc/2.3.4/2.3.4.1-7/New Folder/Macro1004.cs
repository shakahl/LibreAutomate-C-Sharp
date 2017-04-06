out
Dir d
foreach(d "$Program Files$\*.exe" FE_Dir 4|8)
	str path=d.FileName(1)
	sel path 3
		case ["*\Microsoft*","*\Windows*","*\IIS*","*\MS*","*\CE*","*\Debug*","*\Adobe*","*\Java*"] continue
	str s
	if(!GetCertificate(path s)) continue
	out path
	out s
	
