spe
out
out "DELETE FILES"
Dir d
foreach(d "$temp$\*" FE_Dir 0x4)
	str sPath=d.FileName(1)
	out sPath
	del- sPath; err

out "DELETE FOLDERS"
foreach(d "$temp$\*" FE_Dir 0x1)
	sPath=d.FileName(1)
	out sPath
	del- sPath; err
