 /
function~ $untilText [flags] ;;flags: 1 case insensitive, 2 regex, 4 ignore match at start position, 8 don't include untilText, 16 search in single line

 Selects text in the active window, backward from current position, until specified string.
 Returns the selected text.

 untilText - search for this string.

 See also: <SelectTextRightUntil>


spe 1
str s sPrev sQE
int found j L
if(flags&2=0) untilText=sQE.from("\Q" untilText "\E")

rep
	 select word and get selected text
	key CSL
	s.getsel
	if(s.len<=sPrev.len) break; else sPrev=s ;;exit loop if does not select more
	 find untilText, last match
	FINDRX fr.ito=s.len
	 g1
	ARRAY(CHARRANGE) a
	if(!findrx(s untilText fr flags&1|4|16 a)) goto gNext
	j=a[0 a.ubound].cpMin; L=a[0 a.ubound].cpMax-j
	 flag 4
	if(flags&4 and j+L=s.len) flags~4; fr.ito=j; goto g1
	 if selected too far, go back
	if(j>0) ST_GoBack(1 j s); s.get(s j)
	 flag 8
	if(flags&8) ST_GoBack(1 L s); s.get(s L)
	 found
	found=1
	break
	 gNext
	if(flags&16 and findc(s 10)>=0) break

if(found) ret s

if(sPrev.len) key R
