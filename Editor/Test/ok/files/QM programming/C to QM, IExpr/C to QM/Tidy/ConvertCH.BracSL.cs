 /CtoQM
function cl cr

 Joins lines in {} or () blocks.

int i j; lpstr s2
rep
	i=findc(m_s cl i); if(i<0) break
	s2=SkipEnclosed(m_s+i cr); if(!s2) end "%c not found" 1 cr
	j=s2-m_s
	NewLinesToSpaces m_s+i j-i
	i=j
