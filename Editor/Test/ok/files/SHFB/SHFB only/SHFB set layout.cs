int w=TriggerWindow
if w=0
	w=win("Sandcastle Help File Builder" "*.Window.*")
	act w

spe 10
int c=wait(10 WV child("Project Explorer" "" w))
key F3 CSp F5
lef+ 180 68 w 1
lef- 0.47 0.5 c
rep 20
	0.05
	lef+ -2 0.5 c; err continue
	break
lef- 0.5 0.5 c
