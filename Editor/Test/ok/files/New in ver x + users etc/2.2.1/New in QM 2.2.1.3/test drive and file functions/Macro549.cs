out

ARRAY(str) a; int i
 GetFilesInFolder a "$desktop$" "*.html" 4
 GetFilesInFolder a "$desktop$" "wscite\*.html" 4|0x20000
GetFilesInFolder a "$desktop$" "wscite\\.*.html" 4|0x30000
for i 0 a.len
	out a[i]

 GetFilesInFolder a "$system$" "*.exe"
 for i 0 a.len
	 out a[i]

 GetFilesInFolder a "$system$" "^.*\.exe$" 0x10000
 for i 0 a.len
	 out a[i]
