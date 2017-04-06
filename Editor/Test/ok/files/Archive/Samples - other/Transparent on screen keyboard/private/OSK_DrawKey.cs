 /
function hdc ikey

OSKVAR- v
OSKKEY& k=v.a[ikey]
RECT r
r.left=k.x; r.top=k.y
r.right=r.left+30; r.bottom=r.top+30
DrawText hdc k.name 1 &r DT_NOPREFIX
