 AccClick("Match case" "CHECKBUTTON" "Find" "Button" "" 0x1001)
 Acc a=acc("Down" "RADIOBUTTON" "Find" "Button" "" 0x1001)
 a.DoDefaultAction

 AccSelect(SELFLAG_TAKEFOCUS "Find Next" "PUSHBUTTON" "Find" "Button" "" 0x1001)
 Acc a=acc("Match case" "CHECKBUTTON" "Find" "Button" "" 0x1001)
 a.Select(SELFLAG_TAKEFOCUS)
 AccSelect(SELFLAG_TAKESELECTION "Links" "OUTLINEITEM" "Internet Explorer" "SysTreeView32" "" 0x1001)

 Acc a=acc("Find what:" "TEXT" "Find" "Edit" "" 0x1801 0x0 0x20000040)
 int x y cx cy
 a.Location(x y cx cy)
 out "%i %i %i %i" x y cx cy

 Acc a=acc("Up" "RADIOBUTTON" "Find" "Button" "" 0x1001)
 str name=a.Name
 out name

 Acc a=acc("Find what:" "TEXT" "Find" "Edit" "" 0x1801 0x0 0x20000040)
 str value=a.Value
 out value

 a.SetValue("dada")

 Acc a=acc("Context help" "PUSHBUTTON" "Find" "#32770" "" 0x1001)
 str descr=a.Description
 out descr

 Acc a=acc("Up" "RADIOBUTTON" "Find" "Button" "" 0x1001)
 int ro=a.Role(_s)
 out _s