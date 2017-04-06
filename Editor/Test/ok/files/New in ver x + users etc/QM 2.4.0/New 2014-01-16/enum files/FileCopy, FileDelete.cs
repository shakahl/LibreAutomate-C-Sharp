 FileDelete "$temp$\test\import.qml"
 FileDelete "$temp$\test\junction"
 FileDelete "$temp$\test"

 if !RemoveDirectoryW(@_s.expandpath("$temp$\test\admin"))
	 out GetLastError
 FileDelete "$temp$\lnk136F.tmp"

iff("$temp$\test") del- "$temp$\test"
PF
 cop- "$my qm$\test" "$temp$\test"
FileCopy "$my qm$\test" "$temp$\test"
 FileCopy "$my qm$\test\sym" "$temp$\sym"
 ret
PN
 if(mes("Delete folder?" "" "OC")!='O') ret
 del- "$temp$\test"
FileDelete "$temp$\test"
 FileDelete "$temp$\sym"
PN
PO
