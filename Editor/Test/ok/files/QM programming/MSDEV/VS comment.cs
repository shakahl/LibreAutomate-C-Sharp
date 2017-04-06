Acc a.FromMouse

 must be the gray margin of breakpoints
str s=a.Name; if(s!="Glyph Margin Grid") ret
Acc ap; a.Navigate("pa" ap); s=ap.Name; if(s.len) ret

 select that line if it is not in a whole-line selection, to avoid unselecting multiline selection.
 To detect it, we get pixel color at the left side of the text area, it is white when no selection.
Acc at; ap.Navigate("child" at); at.Role(_s); if(_s!="TEXT") end "must be TEXT"
int x y=ym; at.Location(x); x+5
int c=pixel(x y) ;;outx c
spe 10
if(c=0xffffff) lef x y; mou

 if line starts with comments, uncomment, else comment
s.getsel; s.ltrim(" [9]"); if(!s.len) ret
 out s
if s.beg("//") and !s.beg("/// ")
	key C{ku}
else
	key C{kc}

