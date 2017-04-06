 open for reading
File f.Open("file")

 read first 10 lines
str s
int i
for i 0 10
	if(!f.ReadLine(s)) break

 read starting from 1 MB
f.SetPos(1024*1024)
f.ReadToStr(s 10)

