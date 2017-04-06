 RunAs "$windows$\notepad.exe" "" "User"
 RunAs "notepad.exe" "" "User" "" "" ;;access denied (not always) if not full path

 _s.expandpath("$desktop$\text.txt")
_s.expandpath("$qm$\license.txt")
RunAs "$windows$\notepad.exe" _s "User" "" "[*422577CA11F5F70E07*]"

 RunAs "C:\Program Files\Internet Explorer\iexplore.exe" "" "User" "" "[*422577CA11F5F70E07*]"
