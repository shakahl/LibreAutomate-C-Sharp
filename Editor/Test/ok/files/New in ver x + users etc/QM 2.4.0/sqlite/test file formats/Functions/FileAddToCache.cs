 /
function $_file [percent]

opt noerrorshere 1
__HFile f.Create(_file OPEN_EXISTING GENERIC_READ FILE_SHARE_READ)
int i n=GetFileSize(f 0)
if(percent) n=MulDiv(n percent 100)
if(!n) ret
__Handle h=CreateFileMappingW(f 0 PAGE_READONLY 0 0 0)
if(!h) end "CreateFileMapping failed" 16
byte* m=MapViewOfFile(h FILE_MAP_READ 0 0 0)
if(!m) end "MapViewOfFile failed" 16

for(i 0 n 4096) _i=m[i]

UnmapViewOfFile m
