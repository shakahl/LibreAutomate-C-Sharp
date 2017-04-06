out
 act "Notepad"

 Acc a=acc(200 10 0 1)
 Acc a=acc(240 85 _hwndqm 2)
 Acc a=acc(247 64 _hwndqm 0)

 Acc a=acc("" "" _hwndqm "" "" 32|0x100 224 24)
 Acc a=acc("Run" "PUSHBUTTON" _hwndqm "" "" 0)
 Acc a=acc("" "" _hwndqm "" "ok" 0)
 Acc a=acc("" "" _hwndqm "" "Displays" 0x400)
 Acc a=acc("" "PUSHBUTTON" _hwndqm "ToolbarWindow32" "" 0)
 Acc a=acc("Find" "" _hwndqm "SysTreeView32" "" 0)

 int w=win("Options" "#32770")
 Acc a=acc("" "TEXT" w "Edit" "" 0x1805 0x00000040 0x20000040)

 int w=win("Options" "#32770")
 Acc a=acc("" "TEXT" w "Edit" "" 0x9005 &Function80 STATE_SYSTEM_READONLY)

a.Role(_s); out _s
out a.Name
out a.Value
