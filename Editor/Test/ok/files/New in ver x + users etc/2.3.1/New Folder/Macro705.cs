out
long file_size=700*1024*1024+10
long part_size=100*1024*1024

 -------------------

int n=file_size/part_size
long last_part_size=file_size%part_size; if(last_part_size) n+1; else last_part_size=part_size
int i
str s
for i 0 n
	s.format("test.part%i.rar" i+1)
	out "%s, offset=%I64i, size=%I64i" s i*part_size iif(i<n-1 part_size last_part_size)
