out
 str sourceFolder="C:\Windows"
str sourceFolder="Phone Model\Mass memory\Logs"
str destFolder="Q:\Test\Sub"
 str files="*.ini"
str files="*.csv"

 get Shell32.Folder of the source folder (Phone Model\Mass memory\Logs)

Shell32.Shell x._create
Shell32.Folder fMC=x.NameSpace(Shell32.ssfDRIVES) ;;My Computer
Shell32.FolderItem fi=fMC.ParseName(sourceFolder)
if(!fi) end "failed"
 out fi.Name
Shell32.Folder folder1=fi.GetFolder

 get Shell32.Folder of the destination folder

Shell32.Folder folder2=x.NameSpace(destFolder)
if(!folder2) end "failed"

 get  source folder items

ARRAY(Shell32.FolderItem) a
foreach fi folder1.Items
	str name=fi.Name
	 out name
	if(files.len and !matchw(name files 1)) continue
	out name
	a[]=fi

 copy

if(!a.len) out "0 matching files"; ret
int i
for i 0 a.len
	folder2.CopyHere(a[i] 4|16) ;;4 no progress, 16 overwrite silently
	