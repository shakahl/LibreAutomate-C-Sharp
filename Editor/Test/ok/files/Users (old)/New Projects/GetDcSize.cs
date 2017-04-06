 /
function! dc SIZE&si

 Note: if dc is of a window, the size is somewhat incorrect. Tested with a dialog; see Dialog83. Correct for a memory DC.


int bm=GetCurrentObject(dc OBJ_BITMAP); if(!bm) ret
BITMAP b
if(!GetObject(bm sizeof(b) &b)) ret
si.cx=b.bmWidth
si.cy=b.bmHeight
ret 1
