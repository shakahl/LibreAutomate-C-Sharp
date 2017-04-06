function !normal

run "C:\WINDOWS\system32\rundll32.exe" "display.dll,ShowAdapterSettings 1" "" "" 0x4000
int w=wait(10 WA win("Generic PnP Monitor and Intel(R) HD Graphics Family Properties" "#32770"))
Acc a.Find(w "PAGETAB" "Intel® HD Graphics Control Panel" "class=SysTabControl32[]id=12320" 0x1005)
a.DoDefaultAction
a.Find(w "COMBOBOX" "Scaling:" "class=ComboBox[]id=9507" 0x1005 1)
a.CbSelect(1)
a.Find(w "SLIDER" "" "class=msctls_trackbar32[]id=9511" 0x1005)
a.SetValue(100)
if normal
	a.Find(w "SLIDER" "" "class=msctls_trackbar32[]id=9509" 0x1005)
	a.SetValue(100)
but 1 w
