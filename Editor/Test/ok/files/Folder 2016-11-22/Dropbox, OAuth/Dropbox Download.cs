out
#compile "__Dropbox"
Dropbox x
 x.Authorize("kuacy0sv1zqrebo" "mgl8i88y880qxcv")
x.token="bbJgKCNsfDAAAAAAAAAA1cXV1I7w2hblUr2hYr6OHnXbG6hDe9G96qtSk2_NtsT4"

 out x.Download("/test.cs" "" _s)
  out x.Download("id:COLNkyLfeKAAAAAAAAAAAg")
 out _s

if 0
	str tempf="$temp$\dropbox.bin"
	str s="[1][2] [3][4]"; s[2]=0
	 s=""
	 out s
	s.setfile(tempf)
	 out GetFileOrFolderSize(tempf); ret
	
	str dfile="/dropbox.bin"
	out x.Upload(tempf dfile "overwrite")
	out "---------"
else
	dfile="/test.cs"

if 0
	out x.Download(dfile "" _s)
else
	tempf="$temp$\dropbox2.bin"
	out x.Download(dfile tempf)
	_s.getfile(tempf)
out _s
out _s.len
