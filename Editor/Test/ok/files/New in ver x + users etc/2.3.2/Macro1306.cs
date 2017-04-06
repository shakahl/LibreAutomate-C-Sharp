str folder="$Documents$" ;;replace this

 ______________________________

out ;;clear qm output
folder.expandpath
ARRAY(str) a
GetFilesInFolder a folder "*.xls"
int i
for(i 0 a.len) a[i].getfilename
a.sort
str files=a
i=list(files "Click file from which to begin.")
if(i=0) ret ;;Cancel

for i i-1 a.len
	str fn=a[i]
	str sPath.from(folder "\" fn)
	out sPath
