 /
function hwndre ~findthis [$replacetext] [flags] [format] [color] [textcolor] ;;flags: 1 insens, 2 word 4 regex, format: 1 italics, 2 underline, 4 bold, 

 Highlights all occurences of findthis in a rich edit control.
 Works with any application.

 hwndre - rich edit control handle.
 findthis - text or regular expression to find.
 flags:
   1 - case insensitive
   2 - whole word
   4 - findthis is regular expression

 format
   0 - remove underline, bold & italics, 
   1 - italicize
   2 - underline
   4 - make bold
   
 color - highlight color. Not used for version 1 controls.
 textcolor - text color.
  
 replacetext - text that will replace the selection. Can be "" to remove the selected text. If omitted or 0, does not replace.

 EXAMPLE

 SelectReplaceHighlightFormat(TextCid "skin" "skin and bones" 0 2 ColorFromRGB(255 255 255) ColorFromRGB(0 0 0))
   will be underlined replacement



def CFM_ITALIC 0x00000002
def CFM_UNDERLINE 0x00000004
def CFE_BOLD 0x0001
def CFE_ITALIC 0x0002
def CFE_UNDERLINE 0x0004



ARRAY(CHARRANGE) a; int i
str s.getwintext(hwndre); if(!s.len) ret
s.findreplace("[]" "[10]")
CHARRANGE& cr

if(replacetext)
	if(!replacetext[0]) SendMessage hwndre EM_REPLACESEL 0 0 ;;this works in other process too
	else
		GetWindowThreadProcessId hwndre &_i
		if(_i=GetCurrentProcessId)
			SendMessage hwndre EM_REPLACESEL 0 replacetext
		else
			for(_i 0 len(replacetext)) SendMessage hwndre WM_CHAR replacetext[_i] 0
			
	s.getwintext(hwndre); if(!s.len) ret
	s.findreplace("[]" "[10]") 

			
	if(flags&4)
		if(findrx(s replacetext 0 flags&3|4|8 a)<0) ret
	else
		a.create(1 0)
		rep
			if(flags&2) i=findw(s replacetext i 0 flags&1); else i=find(s replacetext i flags&1)
			if(i<0) break
			cr=a[0 a.redim(-1)]
			cr.cpMin=i; i+len(replacetext); cr.cpMax=i
		if(!a.len) ret
else
	if(flags&4)
		if(findrx(s findthis 0 flags&3|4|8 a)<0) ret
	else
		a.create(1 0)
		rep
			if(flags&2) i=findw(s findthis i 0 flags&1); else i=find(s findthis i flags&1)
			if(i<0) break
			cr=a[0 a.redim(-1)]
			cr.cpMin=i; i+findthis.len; cr.cpMax=i
		if(!a.len) ret

__ProcessMemory m.Alloc(hwndre 1000)

if(IsWindowUnicode(hwndre))
	CHARFORMAT2W cfw.cbSize=sizeof(cfw)
	if(color) cfw.dwMask|CFM_BACKCOLOR; cfw.crBackColor=color
	if(textcolor) cfw.dwMask|CFM_COLOR; cfw.crTextColor=textcolor
	sel format
		case 7
			cfw.dwMask|CFM_ITALIC|CFM_UNDERLINE|CFM_BOLD;cfw.dwEffects=CFE_ITALIC|CFE_UNDERLINE|CFM_BOLD;
		case 6
			cfw.dwMask|CFM_ITALIC|CFM_UNDERLINE|CFM_BOLD; cfw.dwEffects=CFE_UNDERLINE|CFM_BOLD
		case 5
			cfw.dwMask|CFM_ITALIC|CFM_UNDERLINE|CFM_BOLD; cfw.dwEffects=CFE_ITALIC|CFM_BOLD
		case 4
			cfw.dwMask|CFM_ITALIC|CFM_UNDERLINE|CFM_BOLD; cfw.dwEffects=CFE_BOLD
		case 3
			cfw.dwMask|CFM_ITALIC|CFM_UNDERLINE|CFM_BOLD;cfw.dwEffects=CFE_ITALIC|CFE_UNDERLINE
		case 2
			cfw.dwMask|CFM_ITALIC|CFM_UNDERLINE|CFM_BOLD ;cfw.dwEffects= CFE_UNDERLINE
		case 1
			cfw.dwMask|CFM_ITALIC|CFM_UNDERLINE|CFM_BOLD; cfw.dwEffects=CFE_ITALIC
		case 0
			cfw.dwMask|CFM_ITALIC|CFM_UNDERLINE|CFM_BOLD;
			
			 Remarks: To turn off a formatting attribute, set the appropriate value in dwMask but do not set the corresponding value in dwEffects. For example, to turn off italics, set CFM_ITALIC but do not set CFE_ITALIC.

			
	m.Write(&cfw sizeof(cfw))
else
	CHARFORMAT2A cfa.cbSize=sizeof(cfa)
	if(color) cfa.dwMask|CFM_BACKCOLOR; cfa.crBackColor=color
	if(textcolor) cfa.dwMask|CFM_COLOR; cfa.crTextColor=textcolor
	sel format
		case 7
			cfa.dwMask|CFM_ITALIC|CFM_UNDERLINE|CFM_BOLD;cfa.dwEffects=CFE_ITALIC|CFE_UNDERLINE|CFM_BOLD;
		case 6
			cfa.dwMask|CFM_UNDERLINE|CFM_BOLD; cfa.dwEffects=CFE_UNDERLINE|CFM_BOLD
		case 5
			cfa.dwMask|CFM_ITALIC|CFM_BOLD; cfa.dwEffects=CFE_ITALIC|CFM_BOLD
		case 4
			cfa.dwMask|CFM_BOLD; cfa.dwEffects=CFE_BOLD
		case 3
			cfa.dwMask|CFM_ITALIC|CFM_UNDERLINE;cfa.dwEffects=CFE_ITALIC|CFE_UNDERLINE
		case 2
			cfa.dwMask|CFM_UNDERLINE ;cfa.dwEffects= CFE_UNDERLINE
		case 1
			cfa.dwMask|CFM_ITALIC; cfa.dwEffects=CFE_ITALIC
			 Remarks: To turn off a formatting attribute, set the appropriate value in dwMask but do not set the corresponding value in dwEffects. For example, to turn off italics, set CFM_ITALIC but do not set CFE_ITALIC.
	m.Write(&cfa sizeof(cfa))


for i 0 a.len
	SendMessage hwndre EM_SETSEL a[0 i].cpMin a[0 i].cpMax
	SendMessage hwndre EM_SETCHARFORMAT SCF_SELECTION m.address 

SendMessage hwndre EM_SETSEL 0 0
ret a.len