 /test dialog image preview
function $folder [flags] ;;flags: 1 include subfolders

 Shows dialog where you can preview jpg/jpeg/png files in the folder.


 thread variables. Add more if need.
type DIPDATA
	str'folder
	hDlg hlv hwb flags
	ARRAY(str)a
DIPDATA- d

d.folder=folder
d.flags=flags

str controls = "4"
str ax4SHD
if(!ShowDialog("DIP_Dialog" &DIP_Dialog &controls)) ret
