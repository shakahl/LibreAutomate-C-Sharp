RECT r1 r2 r3
SetRect &r1 0 0 100 100
 SetRect &r2 50 50 150 150
SetRect &r2 110 110 150 150
out IntersectRect(&r2 &r1 &r2)
zRECT r2
