out
str folder="Q:\MP3"
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

 enumerate files in the folder and display all non-empty properties
Shell32.FolderItem k
foreach k f.Items
	if(k.IsFolder) continue
	str name=k.Name
	out F"<><Z 0x80E080>{name}</Z>"
	 display data of specified columns of the file
	for i 0 colIndices.len
		int j=colIndices[i]
		_s=f.GetDetailsOf(k j)
		out F"{columns[j]}={_s}"
