File f ff
int i eof
str buff

ARRAY(str) files.create(2) ;;input files
files[0]="file1"
files[1]="file2"

ff.Open("file3" "w") ;;output file

for i 0 files.len
	eof=0
	f.Open(files[i])
	rep
		f.ReadToStr(buff 4096); err eof=1
		if(buff.len) ff.Write(buff buff.len 1)
		if(eof) break
