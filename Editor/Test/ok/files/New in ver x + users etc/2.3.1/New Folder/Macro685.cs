spe
Dir d
foreach(d "Q:\MP3\*.jpg" FE_Dir 0x4)
	str sPath=d.FileName(1)
	 out sPath
	del sPath
