out
str folder="Q:\MP3"
str filename="05 - DEUTER - Sound Of Invisible Waters.mp3"
str getColumns="Title[]Authors[]Length"

Shell32.Shell x._create
Shell32.Folder f=x.NameSpace(folder)

 get column names
ARRAY(str) columns
int i
for(i 0 1000000) _s=f.GetDetailsOf(0 i); if(_s.len) columns[]=_s; else break
 for(i 0 columns.len) out columns[i]

 find indices of columns specified in getColumns
ARRAY(int) colIndices
foreach _s getColumns
	for(i 0 columns.len) if(_s=columns[i]) colIndices[]=i; break

 display data of specified columns of the file
Shell32.FolderItem k=f.ParseName(filename)
str name=k.Name
out F"<><Z 0x80E080>{name}</Z>"
for i 0 colIndices.len
	int j=colIndices[i]
	_s=f.GetDetailsOf(k j)
	out F"{columns[j]}={_s}"
