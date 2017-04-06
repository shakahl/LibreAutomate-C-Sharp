 /
function# hmenu $itemstring [flags] ;;flags: 1 error if not found

int i j; str s
for i 0 GetMenuItemCount(hmenu)
	MenuGetString hmenu -i &s
	j=findc(s 9); if(j>=0) s.fix(j)
	if(s=itemstring) ret GetSubMenu(hmenu i)
if(flags&1) end "menu item '%s' not found" 0 itemstring
