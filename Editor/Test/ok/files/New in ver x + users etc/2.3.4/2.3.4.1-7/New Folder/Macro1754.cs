typelib Word {00020905-0000-0000-C000-000000000046} 8.0
str files
if(!inp(files "Please specify the location of the files:" "TestDocRetrieve" "Q:\Test")) ret

ARRAY(str) a
GetFilesInFolder a files "*.doc"

int i
for i 0 a.len
	out a[i]
	
	Word.Document d._getfile(a[i])
	Word.Range r
	
	 get all text
	r=d.Range
	lpstr sAllText=r.Text
	 out sAllText
