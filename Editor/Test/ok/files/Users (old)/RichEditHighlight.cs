 /
function hwndre ~findthis [flags] [color] [textcolor] ;;flags: 1 insens, 2 word, 4 regex, 128 bold

 Highlights all occurences of findthis in a rich edit control.
 Works with any application.

 hwndre - rich edit control handle.
 findthis - text or regular expression to find.
 flags:
   1 - case insensitive
   2 - whole word
   4 - findthis is regular expression
   128 - make bold
 color - highlight color. Not used for version 1 controls. Use -1 to reset to default.
 textcolor - text color. Use -1 to reset to default.

 EXAMPLE
 str rx="\bfind this\b"
 int n=RichEditHighlight(id(59648 "WordPad") rx 1|4|128 ColorFromRGB(255 255 128) ColorFromRGB(255 0 0))
 out "Found %i instances" n


ARRAY(CHARRANGE) a; int i
str s.getwintext(hwndre); if(!s.len) ret
s.findreplace("[]" "[10]")

if(flags&4)
	if(findrx(s findthis 0 flags&3|4|8|16 a)<0) ret
else
	a.create(1 0)
	rep
		if(flags&2) i=findw(s findthis i 0 flags&1); else i=find(s findthis i flags&1)
		if(i<0) break
		CHARRANGE& cr=a[0 a.redim(-1)]
		cr.cpMin=i; i+findthis.len; cr.cpMax=i
	if(!a.len) ret

__ProcessMemory m.Alloc(hwndre 1000)

CHARFORMAT2W cfw.cbSize=sizeof(cfw)
if(color) cfw.dwMask|CFM_BACKCOLOR; if(color=-1) cfw.dwEffects=CFE_AUTOBACKCOLOR; else cfw.crBackColor=color
if(textcolor) cfw.dwMask|CFM_COLOR; if(textcolor=-1) cfw.dwEffects=CFE_AUTOCOLOR; else cfw.crTextColor=textcolor
if(flags&128) cfw.dwMask|CFM_BOLD; cfw.dwEffects=CFE_BOLD
if(IsWindowUnicode(hwndre))
	m.Write(&cfw sizeof(cfw))
else
	CHARFORMAT2A cfa.cbSize=sizeof(cfa)
	cfa.dwMask=cfw.dwMask; cfa.crBackColor=cfw.crBackColor; cfa.crTextColor=cfw.crTextColor; cfa.dwEffects=cfw.dwEffects
	m.Write(&cfa sizeof(cfa))

int selStart selEnd
SendMessage hwndre EM_GETSEL &selStart &selEnd

for i 0 a.len
	SendMessage hwndre EM_SETSEL a[0 i].cpMin a[0 i].cpMax
	SendMessage hwndre EM_SETCHARFORMAT SCF_SELECTION m.address

SendMessage hwndre EM_SETSEL selStart selEnd
ret a.len
