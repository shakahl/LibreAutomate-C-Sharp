 /
function what k ;;what: 0 MSB, 1 LSB, 2 PRG

 This function is called when user presses 0-9, Backspace or ~.
 Don't need to edit it, unless you want to display OSD text somehow differently.


 out "Text: %i %c" what char

str s

if(k=192) ;;~
	s="___"
else
	s=g_mlp.text[what]
	s.ltrim(" _0")
	
	if(k=8) s.fix(s.len-1) ;;Backspace
	else s.formata("%c" k)
	
	int i=3-s.len
	if(i<0) s.remove(0 -i)
	else if(i>0) _s.all(i 2 '_'); s-_s

MLP_Osd what s -1
