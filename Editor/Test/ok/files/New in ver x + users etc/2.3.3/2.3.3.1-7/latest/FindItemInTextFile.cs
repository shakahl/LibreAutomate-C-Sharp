 /
function! $file_ $item

 Finds a line in a text file.
 Returns 1 if found, 0 if not.


str sData.getfile(file_)
if(!sData.len) ret
IStringMap m=CreateStringMap(1|2)
m.AddList(sData "[]")
if(m.Get(item)) ret 1
