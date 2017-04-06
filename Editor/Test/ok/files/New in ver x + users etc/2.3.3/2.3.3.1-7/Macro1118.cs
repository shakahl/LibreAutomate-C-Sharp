out

Dir d
foreach(d "C:\*" FE_Dir 0x6)
	str sPath=d.FileName(1)
	if(d.FileAttributes&FILE_ATTRIBUTE_DIRECTORY)
		 out "-------- folder: %s" sPath
	else
		sel sPath 3
			case "*\$Recycle.Bin\*" continue
			case ["*.log","*.txt"]
			case else continue
		out sPath
		 long size=d.FileSize
		 str sData.getfile(d.FileName(1));; err out "cannot open: %s" sPath; continue
		 str sFolder.getpath(sPath "")
		 str s
		 foreach s sData
			 out s