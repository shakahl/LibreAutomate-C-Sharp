int+ g_away
g_away+1
str s.format("Im away %i" g_away)
outp s

int+ g_away=0

int away
rep
	away+1
	str s.format("Im away %i" away)
	outp s

int+ g_pauseMacroX
rep
	if(g_pauseMacroX) wait 0 -V g_pauseMacroX
	 ...

int+ g_pauseMacroX
g_pauseMacroX=1
 ...
g_pauseMacroX=0
