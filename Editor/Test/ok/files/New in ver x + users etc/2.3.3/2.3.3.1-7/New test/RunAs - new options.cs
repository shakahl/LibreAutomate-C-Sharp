 RunAs "notepad.exe" "" "User" "" "[*F251E9E5C7A037E606*]"
 RunAs22 "notepad.exe" "User" "" "pp"

str f="c:\test.bat"
 str f.expandpath("$desktop$\test\test.bat")
_s=
 notepad.exe
_s.setfile(f)

 out GetCurDir
 RunAs f "" "User" "" "[*F251E9E5C7A037E606*]" 0 0 "*"
 RunAs f "" "User" "" "[*F251E9E5C7A037E606*]" 0 16
 RunAs f "" "User" "" "[*F251E9E5C7A037E606*]" 0 3
out RunAs(f "" "User" "" "[*F251E9E5C7A037E606*]" 0 16|0x400)
 RunAs f "" "User" "" "[*F251E9E5C7A037E606*]" 0 0 "$desktop$"
 out RunAs("cmd.exe" F"/c ''{f}''" "User" "" "[*F251E9E5C7A037E606*]" 0 0x400)
 RunAs22 f "G" "" "p" 0 1
