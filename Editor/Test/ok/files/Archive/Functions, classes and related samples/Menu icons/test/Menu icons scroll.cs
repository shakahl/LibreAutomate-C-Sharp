 Does not show icons in scrolled menus.

out

SetThreadMenuIcons "1=10 3=1 55=2" "$qm$\il_qm.bmp" 1

str s; int i
for(i 1 60) s.formata("%i %i[]" i i)

MenuPopup m.AddItems(s)
 int j=m.Show
POINT p.x=700; p.y=500
int j=m.Show(0 p 1)
out j
