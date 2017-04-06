str s.getsel; if(!s.len) ret

 out; str s="int   WINAPI GetClipRgn( __in HDC hdc, __in HRGN hrgn);"

s.CompactFunction

if(findw(s "WINAPI")<0) s.replacerx(" \w+\(" " WINAPI$0" 4)

str ss.get(s findc(s '('))
ss.replacerx("\w+[\*]* " "")
 out ss

str r=
F
 $1 (WINAPI* $2)$3;
 CREATEHOOK($2);
 $1 WINAPI My$2$3
 {{
 	$1 R=_f.$2{ss}
 }

s.replacerx("(.+?) WINAPI (\w+)(.+);" r 4)


outp s
