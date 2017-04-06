spe 100
int hwnd=val(_command)
Acc a=acc("Microsoft Office Document Image Writer" "LISTITEM" hwnd "SysListView32" "" 0x1001 0 0 "" 5)
a.Select(3); 0.1
but 1 hwnd
hwnd=wait(5 "Save As")
str s.expandpath("$desktop$\print.mdi")
key (s)
0.5
but 1 hwnd
hwnd=wait(2 "Confirm Save As"); err ret
but child("&Yes" "Button" hwnd)
1
run s
