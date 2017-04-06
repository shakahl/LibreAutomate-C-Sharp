 This example adds a static icon, then shows progress in its place, then restores the static icon.

#compile "__TrayProgress"
TrayProgress x.Init

 add 2 icons: the first for progress, the second (or more) for static
x.AddIcon("$qm$\empty.ico[]$qm$\copy.ico")

 show the static icon
x.Modify(2)
1

 show progress
int i
for i 0 40
	x.Update(i*100/40)
	0.05

 show static again
x.Modify(2)
1
