 add tray icon
AddTrayIcon "$qm$\mouse.ico" "Macro ''click_4_rectangles''.[]To end macro, Ctrl+click me."

 create array with coordinates of mouse clicks. Change the values.
ARRAY(POINT) a.create(4)
a[0].x=100
a[0].y=100

a[1].x=500
a[1].y=100

a[2].x=500
a[2].y=300

a[3].x=100
a[3].y=300

 wait for user right click, and click next point from the array
 g1
int i
for i 0 a.len
	wait 0 MR
	lef a[i].x a[i].y
goto g1
