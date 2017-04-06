mkdir "$desktop$\fcopy"
Q &q
int i=2
rep 1
	 del- "$desktop$\fcopy\test.txt"
	 cop- "$desktop$\test.txt" "$desktop$\fcopy"
	 cop- "$desktop$\test.txt" "$desktop$\fcopy" FOF_FILESONLY|FOF_ALLOWUNDO|FOF_NOCONFIRMMKDIR|FOF_SILENT
	FileDelete "$desktop$\fcopy\test.txt"
	FileCopy "$desktop$\test.txt" "$desktop$\fcopy"
	 cop- "$desktop$\f1" "$desktop$\fcopy"
	 cop- "$program files$\Microsoft Office\OFFICE11\EXCEL.EXE" "$desktop$\fcopy" ;;almost 10 MB
	 cop- "$program files$\Microsoft Office\OFFICE11\winword.EXE" "$desktop$\fcopy" ;;11 MB
	 cop- "Q:\win2000.vsv" "$desktop$\fcopy" ;;35 MB
	 cop- "Q:\XP without SP.vsv" "$desktop$\fcopy" ;;60 MB
	 FileCopy "$program files$\Microsoft Office\OFFICE11\winword.EXE" "$desktop$\fcopy" ;;11 MB
	 FileCopy "Q:\win2000.vsv" "$desktop$\fcopy" ;;35 MB
	
	 cop- "$desktop$\qm window.png" "$desktop$\fcopy" FOF_FILESONLY|FOF_ALLOWUNDO|FOF_NOCONFIRMMKDIR|FOF_SILENT
	 ren* "$desktop$\fcopy\qm window.png" "qm window2.png" FOF_FILESONLY|FOF_ALLOWUNDO|FOF_NOCONFIRMMKDIR|FOF_SILENT
	 FileCopy "$desktop$\qm window.png" "$desktop$\fcopy"
	 FileCopy "$desktop$\qm window.png" "$desktop$\fcopy\f1" 2
	 FileDelete "$desktop$\fcopy\qm window.png"
	 del- "$desktop$\fcopy\qm window.png"
	 FileDelete "$desktop$\fcopy\f1"
	
	 FileCopy "$desktop$\f1" "$desktop$\fcopy"
	 FileCopy "$desktop$\f2" "$desktop$\fcopy\f1" 2
	
	 FileCopy "\\GINTARAS\F\software" "$desktop$\fcopy"
	 FileDelete "$desktop$\fcopy\f1"
	 FileMove "$desktop$\qm window.png" "$desktop$\fcopy" 2
	 FileMove "$desktop$\qm window.png" "" 2
	 FileCopy "\\GINTARAS\F\win2000.vhd" "$desktop$\win2000.vhd"
	 FileCopy "$desktop$\test.txt" "\\GINTARAS\F"
	 FileCopy "$desktop$\f2" "\\GINTARAS\F"
	
	 cop "$desktop$\f2" "$desktop$\fcopy"
	 del- "$desktop$\fcopy\f2"
	 FileCopy "$desktop$\f2" "$desktop$\fcopy"
	 FileDelete "$desktop$\fcopy\f2"
	
	 del "$desktop$\fcopy\test.txt"
	 FileDelete "$desktop$\fcopy\test.txt"
	
	 FileDelete "$desktop$\fcoy\test.txt"
	 FileDelete "$desktop$\fcopy\tst.txt"
	
	 SetAttr "$desktop$\fcopy\test.txt" FILE_ATTRIBUTE_HIDDEN|FILE_ATTRIBUTE_READONLY 1; err
	 cop- "$desktop$\test.txt" "$desktop$\fcopy"
	 FileCopy "$desktop$\test.txt" "$desktop$\fcopy"
	
	 FileCopy "$desktop$\test.txt" "$desktop$\test2.txt"
	  SetAttr "$desktop$\fcopy\test2.txt" FILE_ATTRIBUTE_HIDDEN|FILE_ATTRIBUTE_READONLY 1
	 FileMove "$desktop$\test2.txt" "$desktop$\fcopy"
	
	 str s1.format("$desktop$\fcopy\test%i.txt" i); i+1
	 str s2.format("test%i.txt" i)
	  ren* s1 s2
	 FileRename s1 s2
	
Q &qq
outq
