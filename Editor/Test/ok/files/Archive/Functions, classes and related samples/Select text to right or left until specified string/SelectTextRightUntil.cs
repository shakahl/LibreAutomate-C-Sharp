 /
function~ $untilText [flags] ;;flags: 1 case insensitive, 2 regex, 4 ignore match at start position, 8 don't include untilText, 16 search in single line

 Selects text in the active window, forward from current position, until specified string.
 Returns the selected text.

 untilText - search for this string.

 REMARKS
 Works with all text editors that support standard keyboard shortcuts for text selection.
 How it works: Repeatedly selects more and more text using keys Ctrl+Shift+Right, gets selected text and searches for untilText. When found, stops and optionally unselects extra text.
 If untilText not found, presses L to unselect text and returns null. In some editors (Word, QM) it sets text cursor to the initial position, however in others (Notepad, OpenOffice) the behavior is different.

 EXAMPLE
 str s=SelectTextRightUntil("]" 0)
 out s


spe 1
str s sPrev
int found j L(len(untilText)) from

rep
	 select word and get selected text
	key CSR
	s.getsel
	if(s.len<=sPrev.len) break; else sPrev=s ;;exit loop if does not select more
	 find untilText
	 g1
	if(flags&2) j=findrx(s untilText from flags&1 L); else j=find(s untilText from flags&1)
	if(j<0) goto gNext
	 flag 4
	if(flags&4 and j=0) flags~4; from=L; goto g1
	 if selected too far, go back
	j=s.len-j-L
	if(j>0) ST_GoBack(0 j s); s.fix(s.len-j)
	 flag 8
	if(flags&8) ST_GoBack(0 L s); s.fix(s.len-L)
	 found
	found=1
	break
	 gNext
	if(flags&16 and findc(s 10)>=0) break

if(found) ret s

if(sPrev.len) key L
