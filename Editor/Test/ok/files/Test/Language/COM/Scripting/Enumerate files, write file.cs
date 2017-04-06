 Enumerates files, folders and subfolders in drive C:
 Writes all to 'Enumerate Files.rtf' on desktop.

typelib Scripting {420B2830-E718-11CF-893D-00A0C9054228} 1.0

int time=GetTickCount

str writefile.expandpath("$desktop$\Enumerate Files.rtf")
Scripting.FileSystemObject fso._create
Scripting.Folder folder=fso.Drives.Item("C:").RootFolder
Scripting.TextStream t=fso.CreateTextFile(writefile -1 0)

out "Wait ..."
EnumFolder3 folder t 0
t.Close

out "Time: %i s" (GetTickCount-time)/1000

run writefile
