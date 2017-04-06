
spe 10
act win(mouse)
str+ g_mfst
str s.getsel; s.trim
if !s.len or s=g_mfst
	dou; 0.1
	s.getsel; s.trim
if(!s.len) inp- s "word" "Longman dictionary"
g_mfst=s

MFST_Open F"http://www.ldoceonline.com/search/?q={s.escape(9)}"
