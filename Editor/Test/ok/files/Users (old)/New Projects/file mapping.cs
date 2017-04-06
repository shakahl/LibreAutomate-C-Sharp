 str s.getfile("E:\MyProjects\app\winapiqm2.txt")
__HFile hf.Create("E:\MyProjects\app\winapiqm2.txt" OPEN_EXISTING)
int i n=WINAPI.GetFileSize(hf 0)
int hmap=WINAPI.CreateFileMapping(hf 0 WINAPI.PAGE_READWRITE 0 0 0)
byte* m=WINAPI.MapViewOfFile(hmap WINAPI.FILE_MAP_WRITE 0 0 0)
out "%i %i" hmap m
 m[100]=0
 lpstr s=m
 out s
for(i 0 n 1024*16) out m[i]
5
WINAPI.UnmapViewOfFile(m)
CloseHandle(hmap)
