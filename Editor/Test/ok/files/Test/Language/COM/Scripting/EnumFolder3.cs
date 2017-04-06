 /Enumerate files, write file
function Scripting.Folder'folder Scripting.TextStream't indent

Scripting.Folders folders=folder.SubFolders
Scripting.Folder subfolder
Scripting.Files files=folder.Files
Scripting.File file

str si.set(9 0 indent)

ENUMERATOR e.enum=folders._NewEnum
rep
	if(!e.enum.Next(1 &e.item)) break
	subfolder=e.item
	if(si.len) t.Write(si)
	t.WriteLine(subfolder.Name); err out _s.from("Failed to write: " file.Path)
	EnumFolder3 subfolder t indent+1

e.enum=files._NewEnum
rep
	if(!e.enum.Next(1 &e.item)) break
	file=e.item
	if(si.len) t.Write(si)
	t.WriteLine(file.Name); err out _s.from("Failed to write: " file.Path)
