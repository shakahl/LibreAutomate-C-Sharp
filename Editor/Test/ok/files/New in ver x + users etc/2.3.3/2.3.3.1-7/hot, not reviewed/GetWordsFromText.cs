 /
function $text IStringMap&m

if(!m) m=CreateStringMap(1)

str s ss=text
ss.replacerx("[^a-z]" "[]" 1)
 out s
foreach s ss
	if(s.len<2 or !isalpha(s[0])) continue
	s.stem
	int n
	if(m.IntGet(s n)) m.IntSet(s n+1)
	else m.IntAdd(s 1)

  remove rare words
 str sk sv
 m.EnumBegin
 rep
	 if(!m.EnumNext(sk sv)) break
	 