  Creates QM code to run Windows store apps, and displays in QM output and also puts in clipboard.

 open Applications folder, set Details view, add AppUserModelId column
int w=win("Applications" "CabinetWClass")
if w
	act w
	Acc a.Find(w "SPLITBUTTON" "AppUserModelId" "class=DirectUIHWND" 0x1005)
	err w=0
else int closeWindow=1
if !w
	run "shell:AppsFolder"
	w=wait(30 WA win("Applications" "CabinetWClass"))
	key Avd Avh DDVY

 message box
if('O'!mes("Please select the app (or several). Then click OK." "" "OC")) ret

 create code
a.Find(w "LIST" "Items View" "class=DirectUIHWND" 0x1005)
ARRAY(Acc) k; a.ObjectSelected(k)
str s; int i
for i 0 k.len
	a.Find(k[i].a "TEXT" "AppUserModelId" "" 0x1005)
	s+F"run ''shell:AppsFolder\{a.Value}''[]"
s.setclip
out s
act _hwndqm
