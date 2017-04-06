str pw="thisismyrealpassword"

 act "Notepad"
 act "Firefox"
act " Explorer"


spe 10
int i
for i 0 pw.len/2
	key (_s.get(pw i 1)) H
	