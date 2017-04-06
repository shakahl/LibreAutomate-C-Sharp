 out
Dir d
foreach(d "$QM$\*.ico" FE_Dir)
	str sPath=d.FileName(1)
	str s.getfile(sPath) ss
	
	type NEWHEADER @wReserved @wResType @wResCount 
	type ICONDIRENTRY !bWidth !bHeight !bColorCount !bReserved @wPlanes @wBitCount dwBytesInRes dwImageOffset 
	
	int open=0
	NEWHEADER* ph=s
	ICONDIRENTRY* pi=ph+6
	int i n=ph.wResCount
	ss.format("%-20s: %i,  " d.FileName n)
	for i 0 n
		ICONDIRENTRY& ide=pi[i]
		BITMAPINFOHEADER* bi=s+ide.dwImageOffset
		ss.formata(" %i/%i" bi.biWidth bi.biBitCount)
		if(bi.biWidth!16) open=1
	out ss
	 if(open and n>2) run "$program files$\IcoFX 1.6\IcoFX.exe" sPath; 2
	if(n>1) run "$program files$\IcoFX 1.6\IcoFX.exe" sPath; 2
