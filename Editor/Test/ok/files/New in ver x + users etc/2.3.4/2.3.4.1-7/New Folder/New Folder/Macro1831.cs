int i j=5
for(i 1 j)
	out i
	int w
	run "notepad.exe" "" "" "" 0x2800 win("Untitled - Notepad" "Notepad") w
	'Cv             ;; Ctrl+V
	'A{fa}            ;; Alt+F A
	str fileName.expandpath(F"$desktop$\test save files {i}.txt")
	'(fileName) Y      ;; File Name and Enter
	clo w
