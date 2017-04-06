out
int i n; str s ss
s.all(300)
s.flags=1
for i 0xC000 0xFFFF+1
	n=GetClipboardFormatName(i s 300)
	if(!n) continue
	
	 filter out most window classes an clipboard formats
	 s.fix(n)
	 if(findc(s '_')<0) continue
	 ss=s; ss.ucase; if(s!ss) continue
	
	out "0x%X %s" i s
