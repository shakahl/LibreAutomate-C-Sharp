dll "qm.exe"
	!TestFolderCopy $folderFrom $folderTo [flags] [cbFunc] [cbParam]
	!TestFolderDelete $folder [cbFunc] [cbParam]
//flags: 1 error if exist, 2 delete existing (default is merge), 4 let folderTo be parent folder, 8 no recurse, 16 skip hidden system files, 32 don't create empty folders.

 iff- "$My QM$\test\SYSTEM.sy"
	 _s.setfile("$My QM$\test\SYSTEM.sy")
	 SetAttr "$My QM$\test\SYSTEM.sy" FILE_ATTRIBUTE_HIDDEN|FILE_ATTRIBUTE_SYSTEM

out
out "COPY"
 del- "$temp$\test"; err
out TestFolderCopy("$My QM$\test" "$temp$\test" 0)
 out TestFolderCopy("$My QM$\test" "$temp$\test" 16|32 &TestFolderCopyDelete_Callback 100)
 out TestFolderCopy("$My QM$\test\sym" "$temp$\sym")
 ret

 run "$temp$\test"

if(mes("Delete folder '$temp$\test' ?" "" "OC")!='O') ret
out "DELETE"
out TestFolderDelete("$temp$\test")
 out TestFolderDelete("$temp$\test" &TestFolderCopyDelete_Callback 200)
 out TestFolderDelete("$temp$\sym")
