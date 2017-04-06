function [$wintext]
if(len(wintext)) act win(wintext "" "" findc(wintext '*')>=0)
ifa(_hwndqm) ret

out
spe 100
str s ss; int i

 PASTE

 key CaX
s.getmacro("Macro512")
foreach ss s
	ss+"[]"
	ifa("Word") ss+"[]"
	outp ss

 COPY

key CH
for i 0 15
	key HDSE
	 key Cc
	s.getsel
	out "%i. %s" i s
	