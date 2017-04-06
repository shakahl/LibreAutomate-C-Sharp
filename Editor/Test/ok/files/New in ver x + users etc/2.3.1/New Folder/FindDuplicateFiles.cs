 /
function ARRAY(str)&a $parentFolder [$filenamePattern] [flags] ;;flags: 0 files, 1 folders, 2 any, 4 include subfolders, 8 only subfolders, 0x10000 regular expression, 0x20000 evaluate relative path


a=0
ARRAY(str) aa
GetFilesInFolder aa parentFolder filenamePattern flags
IStringMap m=CreateStringMap
int i
for i 0 aa.len
	str s.getfile(aa[i])
	int c=Crc32(s s.len)
	s=c
	m.Add(s aa[i])
	err ;;duplicate
		a[]=aa[i]
		