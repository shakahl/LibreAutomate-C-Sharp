Htm el=htm("INPUT" "Google Search" "" " Internet Explorer" 0 2 0x421)
Acc a=acc("" "TEXT" " Internet Explorer" "Internet Explorer_Server" "macro" 0x1804 0x0 0x20000040)
a.DoDefaultAction
a.Select(SELFLAG_TAKEFOCUS|SELFLAG_TAKESELECTION)
int x y cx cy
a.Location(x y cx cy)
str name=a.Name
str value=a.Value
a.SetValue("ooioioio")
str descr=a.Description
