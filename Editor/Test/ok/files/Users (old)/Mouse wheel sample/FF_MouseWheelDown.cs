 /
function# iid FILTER&f

if(!wintest(win() "Mouse wheel sample" "#32770")) ret -2
int h=child
str s.getwinclass(h)
if(s~"Edit") mac "MouseWheelUpDown" "" h 1 f.tkey; ret -1
else if(s~"RICHEDIT") mac "MouseWheelUpDown" "" h 2 f.tkey; ret -1
ret -2
