 mkdir "$Desktop$\Newf"; err ret
 ChDir "$Favorites$"

 if(dir("$Desktop$\test.txt"))
	 out 1
 if(!dir("C:\Documents and Settings\All Users\Documents" 1))
	 out 0
 iff("C:\Documents and Settings\a\Desktop\test.txt")
	 out 1
 iff("notepad.exe")
	 out 1

 Dir d; str n
 foreach(d "$Desktop$\*.txt" FE_Dir)
	 n=d.FileName
	 out n

 Dir d; str p
 foreach(d "$Desktop$\*.txt" FE_Dir 0xC)
	 p=d.FileName(1)
	 out p

 Dir d; str n
 foreach(d "$Desktop$\*" FE_Dir 0x0 "2004.05.01")
	 n=d.FileName
	 out n
