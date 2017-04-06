int w1=win(" - Mozilla Firefox" "MozillaUIWindowClass")
act w1
Acc a=acc("Sysinternals Forums  " "OUTLINEITEM" w1 "MozillaUIWindowClass" "" 0x1011)
a.DoDefaultAction
