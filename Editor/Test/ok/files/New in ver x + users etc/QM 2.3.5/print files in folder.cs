 Prints all doc and rtf files in My Documents folder.
 The same as if you select the files in Windows Explorer, right click and select Print.

ARRAY(str) a; int i
GetFilesInFolder a "$documents$" ".+\.(doc|rtf)" 0x10000
for i 0 a.len
	run F"{a[i]}" "" "print"
	  or
	 run "winword.exe" F"/q /n /mFilePrintDefault /mFileExit ''{a[i]}''"
