function# tabWidth

 Replaces tabs to spaces.
 Returns the number of found tabs.

 tabWidth - the number of characters that fit in a tab (eg 8 in notepad, 4 in qm).

 EXAMPLE
 str s.getsel ;;copy selected text from eg notepad
 s.TabsToSpaces(8)
 s.setsel


if(tabWidth<1) end ERR_BADARG

str ss.flags=1; ss.all(this.len)
int i j
for i 0 this.len
	sel this[i]
		case 9 ss.set(32 ss.len tabWidth-j); j=0
		case [13,10] ss.geta(this i 1); j=0
		case else ss.geta(this i 1); j+1; if(j=tabWidth) j=0
this=ss
