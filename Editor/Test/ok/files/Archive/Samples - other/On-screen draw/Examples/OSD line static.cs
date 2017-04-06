OnScreenDisplay "Click 2 times. The macro will draw line between clicks." 3

 wait for 2 clicks and get their coordinates
POINT p1 p2
wait 0 ML
xm p1
wait 0 ML
xm p2

 draw line
int color=ColorFromRGB(255 0 0)
OnScreenLine(p1.x p1.y p2.x p2.y color 2)

wait 5
