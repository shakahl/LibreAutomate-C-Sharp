 Tray icon that shows progress of something.

 EXAMPLES

#compile "__TrayProgress"
TrayProgress x.Init
x.AddIcon("empty.ico")

int i
for i 0 40
	x.Update(i*100/40)
	0.2

x.Update(100 " :)")
1
