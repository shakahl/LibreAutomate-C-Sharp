men 40132 "Outlook Express"
but id(1023 "Find Message")
int hwnd=wait(0 win("Outlook Express" "#32770"))
0.1
Acc a=acc("Local Folders" "OUTLINEITEM" hwnd "SysTreeView32" "" 0x1001 0 0 "" 5)
a.Select(SELFLAG_TAKEFOCUS|SELFLAG_TAKESELECTION)
but id(1 win("Outlook Express" "" "" 0x9))
act id(1027 "Find Message")
