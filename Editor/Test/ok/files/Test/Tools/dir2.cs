 mkdir "C:\Documents and Settings\G\Desktop\qmsetup"
 mkdir "kko" "$My Music$"
 ChDir "C:\Documents and Settings\All Users\Documents"
 if(!dir("C:\Documents and Settings\G\My Documents\Run.reg" 2))
		
	 mkdir "C:\Documents and Settings\All Users\Documents"
	 ChDir "$Favorites$" 1; err ret

Dir d; DATE da.getclock; str sPath
foreach(d "*" FE_Dir 0x0 da-1 da-3)
	sPath=d.FileName(1)
	out sPath
	