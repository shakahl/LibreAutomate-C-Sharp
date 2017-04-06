 open unknown files with notepad
str s="Open With Notepad"
rset s "" "*\shell\open" HKEY_CLASSES_ROOT
s="Notepad.exe %1"
rset s "" "*\shell\open\command" HKEY_CLASSES_ROOT
 remove "Shortcut To"
int i=0
rset i "link" "Software\Microsoft\Windows\CurrentVersion\Explorer"
 Word scrolling
rset "1" "LiveScrolling" "Software\Microsoft\Office\8.0\Word\Options"