int w1=win("Internet Explorer" "IEFrame")
act w1
Acc a=acc("" "PAGETAB" w1 "DirectUIHWND" "*http://www.google.*" 0x1404)
a.DoDefaultAction
Acc ac=acc("Close Tab (Ctrl+W)" "PUSHBUTTON" w1 "DirectUIHWND" "" 0x1001)
ac.DoDefaultAction

 could not find a way to do it in firefox
