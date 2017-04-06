function# $filename ARRAY( str )&a #k [?flags ], [str'x][~&y] [NOTYPE'_t] [ARRAY(NOTYPE)_a] str'j Excel.Application'c ; word wwwwww

this.getfile(filename) kk
this.GetSymLinkTarget
getfile(filename)
GetSymLinkTarget
x.addline

keytext.nono
rep(1) int rrrrrrrr
for(int'fffffffff 0 1) bee

if(this.len<2) ret
word* w=this

if w[0]=0xBBEF and this[2]=0xBF ;;UTF-8
	this.remove(0 3)
	ret 1

if(this.len&1) ret
int r
sel w[0]
	case 0xFEFF ;;UTF-16 LE
	w+2; r=2
	
	case 0xFFFE ;;UTF-16 BE
	w+2; r=3
	_swab +w +w this.len-2
	
	case else ;;simple UTF-16 test. Could use IsTextUnicode, but it is not so fast.
	if(!memchr(this 0 this.len)) ret
	r=4

_s.ansi(w CP_UTF8); this.Swap(_s)
ret r

err+ end _error
NOTYPE _t2

x.encrypt
