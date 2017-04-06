dll "qm.exe" !_FolderCopy $from $to [flags]
dll "qm.exe" !_FolderDelete $folder
dll "qm.exe" !_FileCopySpec $fileSpec $toFolder [$defFromDir]

out
 _FolderCopy "$my qm$\test\*" "$temp$" 2|4
 _FolderCopy "$my qm$\*" "$temp$" 2|128
 _FolderCopy "$my qm$\*.ico" "$temp$"

 PF
 _FolderCopy "$my qm$\*" "$temp$" 2
  _FolderCopy "$my qm$\*" "$temp$" 2|0x1000
 PN
 PO

 out _FolderCopy("$my qm$\test" "$temp$\test" 2)

 if !dir("$temp$\test" 1)
	 out _FolderCopy("$my qm$\test" "$temp$\test" 0)
 else
	 out _FolderDelete("$temp$\test")

 out _FileCopySpec("$my qm$\test" "$temp$")
 out _FileCopySpec("test" "$temp$" "$my qm$")
 out _FileCopySpec("$my qm$\item2.bmp" "$temp$\test")
 out _FileCopySpec("$my qm$\*.ico" "$temp$\test")
 out _FileCopySpec("*.ico" "$temp$\test" "$my qm$")

 out _FileCopySpec("$my qm$\imagelists\*.xml" "$temp$\test")
out _FileCopySpec("imagelists\*.xml" "$temp$\test" "$my qm$")
 out _FileCopySpec("$my qm$\imagelists\*.xml" "$temp$\test" "$my qm$")
 out _FileCopySpec("$my qm$\imagelists\more\*.xml" "$temp$\test" "$my qm$\")
 out _FileCopySpec("imagelists\*.xml" "$temp$\test" "$qm$")
 out _FileCopySpec("$my qm$\imagelists\*.xml" "$temp$\test" "$qm$")

 out _FileCopySpec("imagelists\*.no" "$temp$\test" "$my qm$")
 out _FileCopySpec("*.no" "$temp$\test" "$my qm$\")
 out _FileCopySpec("file.no" "$temp$\test" "$my qm$")

#ret
 *.bmp
 *.ico
 $my qm$\toolbars.ini
 test
 $my qm$\Aspell\en.pws
 $my qm$\imagelists\*.xml
 nofile.bam
 $my qm$\*.txt
