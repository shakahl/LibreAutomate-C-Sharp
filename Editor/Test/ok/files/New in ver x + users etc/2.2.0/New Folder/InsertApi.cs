 \
function# [str'name]

 Retrieves and inserts Windows API declaration.
 If name is empty, uses _command. If _command is empty, gets from editor, at caret position.
 If database does not exist or fails to open, can download it.


int h=id(2210 _hwndqm)
str s ss.getwintext(h)
CHARRANGE c
SendMessage(h EM_EXGETSEL 0 &c)

if(!name.len)
	name=_command
	if(!name.len)
		if(c.cpMax>c.cpMin) name.getsel
		else
			for(c.cpMin c.cpMin 0 -1) if(!__iscsym(ss[c.cpMin-1])) break
			for(c.cpMax c.cpMin ss.len) if(!__iscsym(ss[c.cpMax])) break
			if(c.cpMax=c.cpMin) mes "Click or select an identifier." "" "i"; ret
			TEXTRANGE tr.chrg=c; tr.lpstrText=name.all(c.cpMax-c.cpMin)
			name.fix(SendMessage(h EM_GETTEXTRANGE 0 &tr))

int r=GetApi(name &s)
if r
	if(r=1) out "%s not found in the database" name
	else DownloadComponent("$qm$\winapi.dat" "http://www.quickmacros.com/winapi.zip" "Windows API database" 2539)
	ret

int i=c.cpMin
rep
	i=findcr(ss 10 i)+1
	if(!i or (ss[i]!=9 and ss[i]!=',')) break
	i-2 
c.cpMin=i; c.cpMax=i
SendMessage(h EM_EXSETSEL 0 &c)
s+"[]"
SendMessage(h EM_REPLACESEL 0 s)
ret 1
