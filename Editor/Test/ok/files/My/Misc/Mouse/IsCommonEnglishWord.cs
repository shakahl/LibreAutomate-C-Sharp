 /
function! $w [&googleCount]

 Returns 1 if word w is in the 6000 most common English words list.

 googleCount - if the word is not common, receives the number of Google results for the word.


if(&googleCount) googleCount=0

str s ss
IStringMap+ g_icew
if(!g_icew)
	g_icew=CreateStringMap(1)
	s.getfile("$documents$\words_stem.txt")
	g_icew.AddList(s)

s=w; s.stem
if(g_icew.Get(s)) ret 1

if(&googleCount) googleCount=GoogleWordCount(w)

err+
