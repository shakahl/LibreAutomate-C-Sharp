str pw="thisismyrealpassword"

 act "Notepad"
 act "Firefox"
act "Internet Explorer"

Acc a.FromFocus
int x y; a.Location(x y)
x+2; y+4

spe 10
int i
for i pw.len-1 -1 -1
	key (_s.get(pw i 1))
	 lef x y
	key L
